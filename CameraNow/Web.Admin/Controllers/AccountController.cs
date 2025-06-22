using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Commons.Commons;
using Datas.ViewModels;
using Datas.ViewModels.Auth;
using Models.Enums;
using Models.Models;
using Datas.Extensions;
using Services.Interfaces.Services;
using System.Text;
using Datas.Extensions.Responses;
using System.Net;

namespace Web.Admin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<AppUser> _signinManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly ILogger<AccountController> _logger;
        private readonly IAppUserService _userService;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(SignInManager<AppUser> signinManager,
                                UserManager<AppUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                IUserStore<AppUser> userStore,
                                ILogger<AccountController> logger,
                                IAppUserService userService,
                                IMapper mapper,
                                IImageService imageService,
                                SignInManager<AppUser> signInManager)
        {
            _signinManager = signinManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _imageService = imageService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(string filter_text, [Bind(Prefix = "page")] int page, string sorting = "name")
        {
            ViewData["current_FilterText"] = filter_text;

            if (page <= 0) page = 1;

            var spec = new AppUserSpecification(filter_text, Status.All, sorting);
            var pageParams = new PaginatedParams(page, 25);
            var users = await _userService.GetListAsync(spec, pageParams);

            foreach (var item in users.Data)
            {
                var roles = await _userManager.GetRolesAsync(_mapper.Map<AppUser>(item));
                item.Roles = roles;
            }

            ViewBag.Page = page;
            ViewBag.TotalPages = users.TotalPage;
            ViewBag.TotalCount = users.TotalCount;

            return View(users.Data);
        }

        public IActionResult CreateUser()
        {
            var creating_user = new CreateUserViewModel();
            creating_user.Password = CommonGenerates.GenerateDefaultPassword(_userManager.Options);

            return View(creating_user);
        }

        public async Task<IActionResult> DeleteRange(List<string> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new
                    {
                        success = false,
                        error = new
                        {
                            code = HttpStatusCode.BadRequest,
                            message = "Danh sách người dùng cần xóa đang trống."
                        }
                    });
                }

                int deletedCount = 0;

                foreach (var id in ids)
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (result.Succeeded)
                        {
                            deletedCount++;
                        }
                    }
                }

                if (deletedCount > 0)
                {
                    return Json(new
                    {
                        success = true,
                        error = false,
                        deleted = deletedCount
                    });
                }

                return Json(new
                {
                    success = false,
                    error = true,
                    message = "Không xóa được người dùng nào."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = new
                    {
                        code = HttpStatusCode.BadRequest,
                        message = ex.Message,
                        stacktrace = ex.StackTrace,
                    },
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel input)
        {
            if (ModelState.IsValid)
            {
                input.Birthday = DateTime.SpecifyKind(input.Birthday.Value, DateTimeKind.Utc);
                input.EmailConfirmed = !string.IsNullOrEmpty(input.Email) ? true : false;
                input.PhoneNumberComfirmed = !string.IsNullOrEmpty(input.PhoneNumber) ? true : false;

                var user = new AppUser
                {
                    UserName = input.UserName,
                    Email = input.Email,
                    EmailConfirmed = input.EmailConfirmed,
                    FullName = input.FullName,
                    Address = input.Address,
                    Birthday = input.Birthday,
                    PhoneNumber = input.PhoneNumber,
                    PhoneNumberConfirmed = input.PhoneNumberComfirmed,
                };
                var result = await _userManager.CreateAsync(user, input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");

                    // Start:  Chuyển màu login
                    string GreenColor = "\x1b[32m"; // Màu xanh lá cây
                    string ResetColor = "\x1b[0m"; // Trở về màu mặc định
                    _logger.LogInformation($"{GreenColor}User created a new account with password.{ResetColor}");
                    // End

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        // Nếu cấu hình phải xác thực email mới được đăng nhập thì chuyển hướng đến trang
                        // RegisterConfirmation - chỉ để hiện thông báo cho biết người dùng cần mở email xác nhận
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Không cần xác thực - đăng nhập luôn
                        // await _signinManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction(nameof(Index));
                    }

                }

                // Có lỗi, đưa các lỗi thêm user vào ModelState để hiện thị ở html heleper: asp-validation-summary
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();
        }

        public async Task<IActionResult> Profile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                return View(_mapper.Map<AppUserViewModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<ResponseMessage> EditProfile(EditProfileViewModel input, IFormFile ImageFile)
        {
            try
            {
                if (string.IsNullOrEmpty(input.Id))
                {
                    return new ResponseMessage(false, "Giá trị không được để trống");
                }                

                //if (!ModelState.IsValid)
                //{
                //    return View(nameof(Profile));
                //}

                    var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return new ResponseMessage(false, $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                string imagePath = string.Empty;
                string publicId = string.Empty;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadTask = new List<Task<string>>();
                    uploadTask.Add(_imageService.UploadImage(ImageFile, user.Id, user.Id));
                    var uploadResult = await Task.WhenAll(uploadTask);

                    imagePath = uploadResult[0];
                }


                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (input.PhoneNumber != phoneNumber)
                {
                    var setPhoneNumber = await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);
                    if (!setPhoneNumber.Succeeded)
                    {
                        return new ResponseMessage(false, "Không thể cập nhật số điện thoại");
                    }
                }

                if (input.Delete_yn)
                {
                    if (!string.IsNullOrEmpty(user.PublicId))
                    {
                        _imageService.DeleteImage(user.Avatar);
                    }
                }

                user.Address = input.Address;
                user.Birthday = DateTime.SpecifyKind(DateTime.Parse(input.Birthday), DateTimeKind.Utc);
                user.FullName = input.FullName;
                user.Avatar = input.Image_yn ? imagePath : (input.Delete_yn ? "" : user.Avatar);
                user.PublicId = input.Image_yn ? publicId : (input.Delete_yn ? "" : user.PublicId);

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
                        return new ResponseMessage(false, new
                        {
                            Key = "Password",
                            Msg = string.Join(", ", result.Errors.Select(x => x.Description)),
                        });
                    }
                    var setPasswordResult = await _userManager.RemovePasswordAsync(user);
                    if (!setPasswordResult.Succeeded)
                    {
                        return new ResponseMessage(false, "Không thể cập nhật mật khẩu");
                    }
                    setPasswordResult = await _userManager.AddPasswordAsync(user, input.Password);
                    if (!setPasswordResult.Succeeded)
                    {
                        return new ResponseMessage(false, "Không thể cập nhật mật khẩu");
                    }
                }

                await _userManager.UpdateAsync(user);

                await _signinManager.RefreshSignInAsync(user);

                return new ResponseMessage(true, "Cập nhật thông tin tài khoản thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ResponseMessage(true, ex.Message);
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
