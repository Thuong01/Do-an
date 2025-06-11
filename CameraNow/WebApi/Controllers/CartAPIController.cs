using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Datas.Extensions.Responses;
using Datas.ViewModels.Cart;
using Datas.ViewModels.Errors;
using Models.Models;
using Services.Interfaces.Services;

namespace WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ApiController]
    [Authorize]
    public class CartAPIController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<CartAPIController> _logger;

        public CartAPIController(ICartService cartService, UserManager<AppUser> userManager, ILogger<CartAPIController> logger)
        {
            _cartService = cartService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("carts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public async Task<IActionResult> GetCarts()
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                var res = await _cartService.GetCarts(userId);

                return Ok(new ResponseMessage
                {
                    Result = res,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpPost]
        [Route("carts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(CartItemCreateViewModel input)
        {
            try
            {
                if (input.Quantity <= 0)
                {
                    return BadRequest(new ExceptionResponse(StatusCodes.Status400BadRequest, "Số lượng phải lớn hơn 0"));
                }

                var userId = _userManager.GetUserId(User);

                var res = await _cartService.CreateAsync(input, userId);

                if (res == 0)
                {
                    return BadRequest(new ExceptionResponse(StatusCodes.Status400BadRequest, "Có lỗi khi thêm vào giỏ hàng"));
                }

                return Ok(new ResponseMessage
                {
                    Result= res,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpDelete]
        [Route("carts/remove-cart-item")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid productId, Guid cartId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var res = await _cartService.DeleteAsync(userId, cartId, productId);

                if (res > 0)
                {
                    return Ok(new ResponseMessage
                    {
                        Result = res,
                        Success = true
                    });
                }
                else
                {
                    return BadRequest(new ResponseMessage
                    {
                        Success = false
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpPut]
        [Route("carts/item-quantity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuantity(int quantity, Guid productId, Guid cartId)
        {
            try
            {
                var res = await _cartService.UpdateAsync(cartId, productId, quantity);

                if (res > 0)
                {
                    return Ok(new ResponseMessage
                    {
                        Result = res,
                        Success = true
                    });
                }
                else
                {
                    return BadRequest(new ResponseMessage
                    {
                        Success = false
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(400, ex.Message));
            }
        }
    }
}
