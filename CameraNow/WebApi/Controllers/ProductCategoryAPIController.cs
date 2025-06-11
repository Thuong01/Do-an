using Microsoft.AspNetCore.Mvc;
using Datas.ViewModels;
using Datas.ViewModels.Errors;
using Datas.Extensions;
using Services.Interfaces.Services;

namespace WebApi.Controllers
{
    public class ProductCategoryAPIController : BaseAPIController<ProductCategoryViewModel, CreateProductCategoryViewModel, UpdateProductCategoryViewModel>
    {
        private readonly ICategoryService _service;
        private readonly ILogger<ProductCategoryAPIController> _logger;

        public ProductCategoryAPIController(ICategoryService service, ILogger<ProductCategoryAPIController> logger) 
            : base(service, logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get List Product Categories
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="pageParams"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("product-categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetList([FromQuery] BaseSpecification spec)
        {
            try
            {
                var res = await _service.GetListParentAsync(spec);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ExceptionResponse(400, ex.Message));
            }
        }
    }
}
