using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Datas.ViewModels.Errors;
using Datas.ViewModels.Order;
using Models.Models;
using Datas.Extensions;
using Services.Interfaces.Services;
using Models.Enums;

namespace WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [Authorize]
    public class OrderAPIController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderAPIController> _logger;

        public OrderAPIController(IOrderService orderService, UserManager<AppUser> userManager, IMapper mapper, ILogger<OrderAPIController> logger)
        {
            _orderService = orderService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders of all user
        /// </summary>
        /// <param name="pageParams"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllOrder([FromQuery] PaginatedParams pageParams)
        {
            try
            {
                var res = await _orderService.GetAllOrdersAsync(pageParams, new[] { "OrderDetails", "User", "OrderDetails.Product" });
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        /// <summary>
        /// Get orders of user
        /// </summary>
        /// <param name="paegParams"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("orders/user-orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrdersUser([FromQuery] PaginatedParams paegParams, OrderStatusEnum status, string keyword = null)
        {
            try
            {
                var res = await _orderService.GetOrdersUserAsync(_userManager.GetUserId(User), paegParams, status, new[] { "OrderDetails", "User", "OrderDetails.Product" }, keyword);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] OrderCreateViewModel value)
        {
            try
            {
                value.User_Id = _userManager.GetUserId(User);
                return Ok(await _orderService.CreateAsync(value));
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpDelete]
        [Route("orders/cancelled-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelledOrder(Guid id)
        {
            try
            {
                return Ok(await _orderService.CancelledOrder(id, _userManager.GetUserId(User)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpGet]
        [Route("orders/order-user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid id, string userId)
        {
            try
            {
                return Ok(await _orderService.GetOrderById(id, userId, new[] { "OrderDetails", "User", "OrderDetails.Product" }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }
    }
}
