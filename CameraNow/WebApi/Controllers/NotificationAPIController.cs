using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Datas.Extensions;
using Datas.Extensions.Responses;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels.Notification;
using Models.Models;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [Authorize]
    public class NotificationAPIController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationAPIController(INotificationRepository notificationRepository, UserManager<AppUser> userManager, IUnitOfWork _unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            this._unitOfWork = _unitOfWork;
        }

        [HttpPost]
        [Route("notifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationViewModel input)
        {
            try
            {
                var notification = new Notification
                {
                    Title = input.Title,
                    Content = input.Content,
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false,
                    User_ID = input.User_ID,
                    Role = input.Role
                };
                _notificationRepository.Add(notification);
                _unitOfWork.Commit();
                return Ok(new ResponseMessage
                {
                    Success = true,
                    Result = notification
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("notifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId); // lấy AppUser

                if (user == null)
                    return NotFound();

                var roles = await _userManager.GetRolesAsync(user); // truyền AppUser vào đây
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                var res = (await _notificationRepository.GetAllAsync()).Where(x => x.User_ID == userId && x.Role == role);
                return Ok(new ResponseMessage
                {
                    Success = true,
                    Result = res.Select(x => new NotificationViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Content = x.Content,
                        CreatedDate = x.CreatedDate,
                        IsRead = x.IsRead,
                        User_ID = x.User_ID,
                        Role = x.Role
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
