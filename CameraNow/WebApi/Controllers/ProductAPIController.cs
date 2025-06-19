using Microsoft.AspNetCore.Mvc;
using Datas.Extensions.Responses;
using Datas.ViewModels.Errors;
using Datas.ViewModels.Product;
using Datas.Extensions;
using Services.Interfaces.Services;

namespace WebApi.Controllers
{
    public class ProductAPIController : BaseAPIController<ProductViewModel, CreateProductViewModel, UpdateProductViewModel>
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductAPIController> _logger;

        public ProductAPIController(IProductService productService, ICategoryService categoryService, ILogger<ProductAPIController> logger) : base(productService, logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        [Route("products")]
        public virtual async Task<IActionResult> GetList([FromQuery] ProductSpecification spec, [FromQuery] PaginatedParams pageParams)
        {
            try
            {
                var res = await _productService.GetListAsync(spec, pageParams);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ProductController ==> {ex.Message}");
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpGet]
        [Route("products/{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var res = await _productService.GetByIdAsync(id);

                if (res == null)
                    return NotFound();

                if (res.Images.Count() > 0)
                {
                    foreach (var item in res.Images)
                    {
                        item.Link = "https://localhost:7110/" + item.Link;
                    }
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ProductController ==> {ex.Message}");
                return BadRequest(new ExceptionResponse (400, ex.Message));
            }
        }

        [HttpGet]
        [Route("products/breadcrumbs/{productId}")]
        public async Task<IActionResult> GetBreadcrumb(Guid productId)
        {
            try
            {
                var product = await _productService.GetByIdAsync(productId);

                if (product.Category_ID == null)
                {
                    return NotFound(new ExceptionResponse(404, "Product does not belong to any category."));
                }

                var breadcrumb = new List<object>();
                var parentId = product.Category_ID;

                while (parentId != null)
                {
                    var category = await _categoryService.GetByIdAsync(parentId.Value);

                    string link = $"{category.Alias}";
                    string title = category.Name;

                    breadcrumb.Add(new
                    {
                        link = link,
                        title = title,
                    });

                    parentId = category.Parent_ID;
                }

                return Ok(new ResponseMessage
                {
                    Result = breadcrumb,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ProductController ==> {ex.Message}");
                return BadRequest(new ExceptionResponse(400, ex.Message));
            }
        }

    }
}