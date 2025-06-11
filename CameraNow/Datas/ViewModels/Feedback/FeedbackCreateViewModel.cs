using Datas.ViewModels.Image;
using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels.Feedback
{
    public class FeedbackCreateViewModel
    {
        public Guid? Parent_ID { get; set; }

        [Required]
        public Guid Product_ID { get; set; }
        public Guid Order_ID { get; set; }
        public string? User_ID { get; set; }

        public string Subject { get; set; }

        public string? Message { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, 5, ErrorMessage = "Đánh giá sao giá trị chỉ được phép từ 1 đến 5")]
        public int Rating { get; set; } = 0;

        public IEnumerable<FeedbackImageCreateViewModel>? Images { get; set; }
    }
}
