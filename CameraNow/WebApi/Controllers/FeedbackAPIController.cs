using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Datas.Extensions;
using Datas.Extensions.Responses;
using Datas.ViewModels.Errors;
using Datas.ViewModels.Feedback;
using Models.Models;
using Services.Interfaces.Services;

namespace WebApi.Controllers
{
    /// <summary>
    /// Feedbacks of the product
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ApiController]
    [Authorize]
    public class FeedbackAPIController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<FeedbackAPIController> _logger;

        public FeedbackAPIController(IFeedbackService feedbackService, UserManager<AppUser> userManager, 
            ILogger<FeedbackAPIController> logger)
        {
            _feedbackService = feedbackService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get product feedbacks by product id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="feedbackPageParams"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Request Example: api/FeedbackAPI/feedbacks/product-feedbacks
        ///     
        ///     
        /// </remarks>
        [HttpGet]
        [Route("feedbacks/product-feedbacks")]
        [AllowAnonymous]        
        public async Task<IActionResult> GetProductFeedbacks(Guid productId, [FromQuery] PaginatedParams feedbackPageParams)
        {
            try
            {
                var res = await _feedbackService.GetFeedbacks(productId, feedbackPageParams);

                return Ok(new ResponseMessage(true, res));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpGet]
        [Route("feedbacks")]
        public async Task<IActionResult> GetUserFeedback(Guid productId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                var res = await _feedbackService.GetUserFeedback(userId, productId);

                return Ok(new ResponseMessage(true, res));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpPost]
        [Route("feedbacks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(FeedbackCreateViewModel input)
        {
            try
            {
                input.User_ID = _userManager.GetUserId(User);

                var res = await _feedbackService.CreateAsync(input);

                return Ok(new ResponseMessage(true, res));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpPost]
        [Route("feedbacks/like-feedback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LikeFeedback (Guid fbid, Guid productId)
        {
            try
            {
                var res = await _feedbackService.LikeFeedback(fbid, productId);

                return Ok(new ResponseMessage(true, res));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest(new ExceptionResponse(400, ex.Message));
            }
        }
    }
}
