using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Commons.Commons;
using Datas.Data;
using Datas.Extensions.Responses;
using Datas.ViewModels.Auth;
using Datas.ViewModels.Errors;
using Models.Models;
using Services.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICartService _cartService;
        private readonly IConfiguration _configuration;
        private readonly CameraNowContext _dbContext;

        public AuthService(
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ICartService cartService,
            IConfiguration configuration, 
            CameraNowContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _cartService = cartService;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<ResponseMessage> ForgetPasword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Result =  $"Người dùng với email {email} không tồn tại"
                };
            }

            // Tạo mật khẩu mới
            var newPassword = GenerateRandomPassword();

            // Reset mật khẩu
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!result.Succeeded)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Result = result.Errors
                };
            }

            // Gửi email
            CommonExtensions.SendMail(CommonConstant.AppName, "Mật khẩu mới", $"Mật khẩu mới của bạn là: {newPassword}", email);

            return new ResponseMessage
            {
                Success = true,
                Result = "Mật khẩu mới đã được gửi qua email."
            };
        }

        public async Task<AuthResultViewModel> LoginAsync(string UserName, string Password)
        {
            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
            {
                throw new Exception($"User named '{UserName}' not found");
            }

            if (!user.EmailConfirmed)
            {
                throw new Exception("Email not confirmed");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, Password);
            if (!passwordValid)
            {
                throw new Exception("Invalid password.");
            }

            var token = GenerateJwtToken(user);

            return new AuthResultViewModel()
            {
                Token = token.Item1,
                RefreshToken = token.Item2,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };
        }

        public async Task<ResponseMessage> RegisterAsync(RegisterViewModel model)
        {
            var userIsExist = await _userManager.FindByNameAsync(model.UserName);

            if (userIsExist != null)
                return new ResponseMessage(false, new
                {
                    Message = $"UserName {model.UserName} already exists!",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            var newUser = new AppUser
            {
                EmailConfirmed = true,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                Address = model.Address,
                SecurityStamp = new Guid().ToString()
            };

            var userResult = await _userManager.CreateAsync(newUser, model.Password);

            if (!userResult.Succeeded)
            {
                //throw new Exception(new ExceptionResponse(StatusCodes.Status400BadRequest, userResult.Errors).ToString());
                return new ResponseMessage(false, new
                {
                    Message = userResult.Errors,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var role = await _userManager.AddToRoleAsync(newUser, "User");

            await _cartService.CreateCartAsync(newUser.Id);

            return new ResponseMessage(true, 1);
        }

        private (string, string) GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles to claims
            var userRoles = _userManager.GetRolesAsync(user).Result; // Assuming you're using ASP.NET Core Identity
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var oldRefreshTokens = _dbContext.RefreshTokens
                                    .Where(rt => rt.User_Id == user.Id && !rt.Is_Revoked && rt.Date_Expire > DateTime.UtcNow)
                                    .ToList();

            foreach (var oldToken in oldRefreshTokens)
            {
                oldToken.Is_Revoked = true;
            }
            _dbContext.RefreshTokens.UpdateRange(oldRefreshTokens);

            var refreshToken = new RefreshToken()
            {
                Jwt_Id = token.Id,
                Is_Revoked = false,
                User_Id = user.Id,
                Date_Added = DateTime.UtcNow,
                Date_Expire = DateTime.UtcNow.AddDays(1),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            _dbContext.SaveChanges();

            return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken.Token);
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+";
            var res = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var uintBuffer = new byte[sizeof(uint)];

                while (res.Length < length)
                {
                    rng.GetBytes(uintBuffer);
                    var num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }
    }
}

