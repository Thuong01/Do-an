using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Datas.Extensions.Responses;
using Datas.ViewModels.Errors;
using Datas.ViewModels.Feedback;
using Models.Models;
using Services.Interfaces.Services;

namespace Web.Admin.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<FeedbackController> _logger;
        private readonly IMapper _mapper;

        public FeedbackController(IFeedbackService feedbackService, UserManager<AppUser> userManager, ILogger<FeedbackController> logger, IMapper mapper)
        {
            _feedbackService = feedbackService;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> ReplyFeedback(Guid parentId, Guid productId, string subject, string message)
        {
            try
            {
                var reply = new FeedbackCreateViewModel
                {
                    Parent_ID = parentId,
                    User_ID = _userManager.GetUserId(User),
                    Product_ID = productId,
                    Subject = subject,
                    Message = message
                };

                var res = await _feedbackService.CreateAsync(reply);

                return RedirectToAction("Details", "Product", new { id = productId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Details", "Product", new { id = productId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LikeFeedback(string fbid, string productId)
        {
            try
            {
                var res = await _feedbackService.LikeFeedback(_mapper.Map<Guid>(fbid), _mapper.Map<Guid>(productId));

                return new JsonResult(new ResponseMessage(true, res));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return new JsonResult(new ExceptionResponse(400, ex.Message));
            }
        }

        public async Task<IActionResult> DeleteFeedback(string fbid, string productId)
        {
            try
            {
                var res = await _feedbackService.DeleteFeedback(_mapper.Map<Guid>(fbid), _mapper.Map<Guid>(productId));

                return new JsonResult(new ResponseMessage(true, res));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return new JsonResult(new ExceptionResponse(400, ex.Message));
            }
        }
    }
}
