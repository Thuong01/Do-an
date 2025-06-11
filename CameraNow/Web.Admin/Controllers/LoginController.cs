using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Datas.ViewModels;
using Models.Models;
using Services.Interfaces.Services;
using System.Text;

namespace Web.Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginController> _logger;
        private readonly IAuthService _authService;

        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                        ILogger<LoginController> logger, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel input)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(input.UserName, input.Password, input.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(input.UserName);

                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                    if (isAdmin)
                    {
                        _logger.LogInformation($"Admin {input.UserName} logged in system.");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _logger.LogWarning($"User {input.UserName} attempted to log in without admin role.");
                        ModelState.AddModelError(string.Empty, "You do not have permission to access this area.");
                        return View("Index");
                    }
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning($"User account locked out.");
                    ModelState.AddModelError(string.Empty, $"User account locked out.");

                    return View("Index");
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogWarning($"User account is not Allowed.");
                    ModelState.AddModelError(string.Empty, $"User account is not Allowed.");

                    return View("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Invalid login attempt.");
                    return View("Index");
                }
            }

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
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


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
