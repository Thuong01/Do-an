using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Datas.Extensions.Responses;
using Datas.ViewModels.Auth;
using Datas.ViewModels.Errors;
using Models.Models;
using Services.Interfaces.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAPIController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthService _authService;

        public LoginAPIController(UserManager<AppUser> userManager, IAuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Request Example: api/LoginAPI/login-user
        ///     {
        ///         "userName": "Admin",
        ///         "password": "@Abc123456"
        ///     }
        /// </remarks>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var res = await _authService.LoginAsync(login.UserName, login.Password);

            if (res == null)
            {
                return NotFound(new ExceptionResponse(StatusCodes.Status404NotFound, $"UserName {login.UserName} not found!"));
            }

            return Ok(new ResponseMessage(true, res));
        }

        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var res = await _authService.RegisterAsync(model);

            if (res.Success)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }

        [HttpPost]
        [Route("forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var res = await _authService.ForgetPasword(email);
            if (res.Success)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }
    }
}
