using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Datas.Extensions.Responses;
using Services.Interfaces.Services;
using Services.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly AprioriService _aprioriService;

        public RecommendationController(AprioriService aprioriService, IProductService productService)
        {
            _aprioriService = aprioriService;
            _productService = productService;
        }

        public IProductService _productService { get; }

        /// <summary>
        /// Lấy danh sách sản phẩm gợi ý dựa trên luật kết hợp
        /// Confidence là độ tin cậy của một luật kết hợp: Nếu người dùng mua A, thì họ sẽ mua B với xác suất bao nhiêu?
        /// CT: Confidence(A ⇒ B) = Support(A ∪ B) / Support(A)
        /// giá trị minConfidence càng cao thì độ tin cậy của luật càng cao, càng ít luật được tạo ra
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("recommendations/product-recomments/{productId}")]
        public async Task<IActionResult> GetProductRecommendations(Guid productId, [FromQuery] int limit = 5)
        {
            try
            {
                var minConfidence = 0.3;

                // Lấy các luật kết hợp (có thể cache kết quả này)
                var rules = await _aprioriService.GetCachedAssociationRulesAsync(minConfidence); // minConfidence = 70%

                // Lọc và sắp xếp các luật liên quan
                var relevantRules = rules
                    .Where(r => r.Antecedent.Contains(productId))
                    .OrderByDescending(r => r.Confidence)
                    .ThenByDescending(r => r.Support)
                    .Take(limit)
                    .ToList();

                //if (!relevantRules.Any())
                //{
                //    // Fallback: trả về sản phẩm bán chạy nếu không có gợi ý
                //    var popularProducts = await _productService.GetPopularProductsAsync(limit);
                //    return Ok(new ResponseMessage
                //    {
                //        Success = true,
                //        Result = new
                //        {
                //            ProductId = productId,
                //            Recommendations = popularProducts,
                //            Rules = Array.Empty<object>()
                //        }
                //    });
                //}

                // Lấy thông tin sản phẩm gợi ý
                var recommendedProductIds = relevantRules
                    .SelectMany(r => r.Consequent)
                    .Distinct()
                    .ToList();

                var recommendedProducts = _productService.GetAllAsync().Result
                    .Where(p => recommendedProductIds.Contains(p.ID))
                    .Select(p => new
                    {
                        p.ID,
                        p.Name,
                        p.Image,
                        p.Price,
                        p.Promotion_Price,
                        p.Rating
                    })
                    .OrderByDescending(p => p.Rating) // Sắp xếp thêm theo rating
                    .ThenByDescending(p => p.Promotion_Price.HasValue); // Ưu tiên sản phẩm có khuyến mãi

                return Ok(new ResponseMessage
                {
                    Success = true,
                    Result = new
                    {
                        ProductId = productId,
                        Recommendations = recommendedProducts,
                        Rules = relevantRules.Select(r => new
                        {
                            Antecedent = r.Antecedent,
                            Consequent = r.Consequent,
                            Confidence = r.Confidence,
                            Support = r.Support
                        })
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseMessage
                {
                    Success = false,
                    Result = "An error occurred while processing your request"
                });
            }
        }

        // Endpoint để cập nhật/cải thiện luật
        [HttpPost("update-rules")]
        public IActionResult UpdateAssociationRules([FromQuery] double minSupport = 0.1, [FromQuery] double minConfidence = 0.7)
        {
            var rules = _aprioriService.GenerateAssociationRules(minConfidence);
            // Có thể lưu rules vào cache hoặc database để sử dụng sau này
            return Ok(new { Count = rules.Count });
        }

        //[HttpGet("recommend/{productName}")]
        //public IActionResult GetRecommendations(string productName)
        //{
        //    double minSupport = 0.2; // Ngưỡng support
        //    double minConfidence = 0.5; // Ngưỡng confidence

        //    // Tạo frequent item sets
        //    var frequentItemSets = _aprioriService.GenerateFrequentItemSets(_transactions, minSupport);

        //    // Tạo luật kết hợp
        //    var rules = _aprioriService.GenerateAssociationRules(frequentItemSets, minConfidence);

        //    // Lọc các luật có productName ở vế trái
        //    var relevantRules = rules
        //        .Where(r => r.Key.TakeWhile(x => x != "=>").Contains(productName))
        //        .OrderByDescending(r => r.Value)
        //        .ToList();

        //    // Lấy các sản phẩm được đề xuất
        //    var recommendations = new List<string>();
        //    foreach (var rule in relevantRules)
        //    {
        //        var consequent = rule.Key.SkipWhile(x => x != "=>").Skip(1).ToArray();
        //        recommendations.AddRange(consequent);
        //    }

        //    // Loại bỏ trùng lặp và sản phẩm đã chọn
        //    recommendations = recommendations
        //        .Distinct()
        //        .Where(r => r != productName)
        //        .ToList();

        //    return Ok(recommendations);
        //}


    }
}
