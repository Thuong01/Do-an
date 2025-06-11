using AutoMapper;
using Datas.Data;
using Datas.Extensions.Responses;
using Datas.Extensions;
using Datas.ViewModels.Errors;
using Datas.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Services;

namespace MyShop.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    public class CouponAPIController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponAPIController> _logger;
        private readonly CameraNowContext _dbContext;

        public CouponAPIController(ICouponService couponService, IMapper mapper,
                ILogger<CouponAPIController> logger,
                CameraNowContext dbContext)
        {
            _couponService = couponService;
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("coupons")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCoupons([FromQuery] PaginatedParams pageParams, [FromQuery] BaseSpecification spec)
        {
            try
            {
                var res = await _couponService.GetListAsync(spec, pageParams);
                return Ok(new ResponseMessage
                {
                    Success = true,
                    Result = res,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new ExceptionResponse(400, ex.Message));
            }
        }

        [HttpGet]
        [Route("coupons/{code}")]
        public async Task<IActionResult> GetById(string code)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                var res = await _couponService.GetByIdAsync(code);

                var isUsed = _dbContext.UsedCoupons.Where(x => x.CouponCode == code && x.UserId == userId).Any();

                if (isUsed)
                {
                    return BadRequest(new ResponseMessage
                    {
                        Success = false,
                        Result = "Mã giảm giá đã được sử dụng"
                    });
                }

                if (res == null)
                {
                    return NotFound();
                }
                ;

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(400, ex.Message));
            }
        }
    }
}
