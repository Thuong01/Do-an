using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Datas.Extensions.Responses;
using Datas.ViewModels.Auth;
using Datas.ViewModels.Errors;
using Models.Models;
using Services.Interfaces.Services;
using Services.Services;

namespace WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]

    public class UserAPIController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAPIController> _logger;
        private readonly SignInManager<AppUser> _signinManager;

        public UserAPIController(UserManager<AppUser> userManager, ICartService cartService,
                    IMapper mapper, ILogger<UserAPIController> logger,
                    SignInManager<AppUser> signinManager)
        {
            _userManager = userManager;
            _cartService = cartService;
            _mapper = mapper;
            _logger = logger;
            _signinManager = signinManager;
        }

        [HttpGet]
        [Route("user/infor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserInfor()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var uservm = _mapper.Map<AppUserViewModel>(user);
                uservm.CartId = await _cartService.GetCartId(user.Id);

                return Ok(new ResponseMessage()
                {
                    Success = true,
                    Result = uservm
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new ExceptionResponse(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        [Route("user/edit-profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> EditProfile(EditProfileViewModel input)
        {
            try
            {
                if (string.IsNullOrEmpty(input.Id))
                {
                    return BadRequest(new ResponseMessage(false, "Giá trị không được để trống"));
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new ResponseMessage(false, $"Unable to load user with ID '{_userManager.GetUserId(User)}'."));
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (input.PhoneNumber != phoneNumber)
                {
                    var setPhoneNumber = await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);
                    if (!setPhoneNumber.Succeeded)
                    {
                        return BadRequest(new ResponseMessage(false, "Không thể cập nhật số điện thoại"));
                    }
                }
                user.Address = input.Address;
                if (user.Email != input.Email)
                {
                    user.Email = input.Email;
                }
                user.Birthday = DateTime.SpecifyKind(DateTime.Parse(input.Birthday), DateTimeKind.Utc);
                user.FullName = input.FullName;
                if (!string.IsNullOrEmpty(input.Password))
                {
                    var passwordValidator = _userManager.PasswordValidators.FirstOrDefault();
                    var result = await passwordValidator.ValidateAsync(_userManager, user, input.Password);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("Password", error.Description);
                        }
                        return BadRequest(new ResponseMessage(false, new
                        {
                            Key = "Password",
                            Msg = string.Join(", ", result.Errors.Select(x => x.Description)),
                        }));
                    }
                    var setPasswordResult = await _userManager.RemovePasswordAsync(user);
                    if (!setPasswordResult.Succeeded)
                    {
                        return BadRequest(new ResponseMessage(false, "Không thể cập nhật mật khẩu"));
                    }
                    setPasswordResult = await _userManager.AddPasswordAsync(user, input.Password);
                    if (!setPasswordResult.Succeeded)
                    {
                        return BadRequest(new ResponseMessage(false, "Không thể cập nhật mật khẩu"));
                    }
                }

                var update_res = await _userManager.UpdateAsync(user);
                if (update_res.Succeeded)
                {
                    await _signinManager.RefreshSignInAsync(user);
                    return Ok(new ResponseMessage(true, "Cập nhật thông tin tài khoản thành công"));
                }
                else
                {
                    return BadRequest(new ResponseMessage(false, update_res.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ResponseMessage(true, ex.Message));
            }
        }
    }
}
