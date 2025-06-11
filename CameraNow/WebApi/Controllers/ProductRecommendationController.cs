using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Datas.ViewModels.ML;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductRecommendationController : ControllerBase
    {
        private static Dictionary<string, List<List<string>>> _seasonalTransactions = new Dictionary<string, List<List<string>>>
        {
            // Dữ liệu giao dịch theo mùa (ví dụ)
            ["Spring"] = new List<List<string>>
        {
            new List<string> { "Áo khoác nhẹ", "Giày thể thao", "Quần jean" },
            new List<string> { "Áo khoác nhẹ", "Mũ len", "Khăn quàng cổ" },
            // Thêm dữ liệu khác...
        },
            ["Summer"] = new List<List<string>>
        {
            new List<string> { "Áo phông", "Quần short", "Kính râm" },
            new List<string> { "Áo phông", "Quần short", "Mũ lưỡi trai" },
            // Thêm dữ liệu khác...
        },
            // Thêm các mùa khác...
        };

        [HttpGet("seasonal/{season}")]
        public IActionResult GetSeasonalRecommendations(string season, [FromQuery] string productId)
        {
            if (!_seasonalTransactions.ContainsKey(season))
                return NotFound($"Không tìm thấy dữ liệu cho mùa {season}");

            var transactions = _seasonalTransactions[season];
            var apriori = new Apriori(minSupport: 0.2, minConfidence: 0.5);

            var frequentItemsets = apriori.GetFrequentItemsets(transactions);
            var rules = apriori.GenerateAssociationRules(frequentItemsets);

            // Tìm các sản phẩm thường đi kèm với sản phẩm được chọn
            var recommendations = new List<RecommendationResult>();

            foreach (var rule in rules)
            {
                if (rule.Key.Contains(productId))
                {
                    foreach (var r in rule.Value)
                    {
                        recommendations.Add(new RecommendationResult
                        {
                            RecommendedProducts = r.Consequent,
                            Confidence = r.Confidence,
                            Support = r.Support
                        });
                    }
                }
            }

            // Sắp xếp theo độ tin cậy giảm dần
            var sortedRecommendations = recommendations
                .OrderByDescending(r => r.Confidence)
                .ThenByDescending(r => r.Support)
                .ToList();

            return Ok(sortedRecommendations);
        }

    }


    public class RecommendationResult
    {
        public List<string> RecommendedProducts { get; set; }
        public double Confidence { get; set; }
        public double Support { get; set; }
    }
}
