using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Feedbacks")]
    public class Feedback
    {
        [Key]
        [Required]
        public Guid ID { get; set; }

        public Guid? Parent_ID { get; set; }

        [Required]
        public Guid Product_ID { get; set; }

        [ForeignKey(nameof(Product_ID))]
        public Product Product { get; set; }

        [Required]
        public string User_ID { get; set; }

        [ForeignKey(nameof(User_ID))]
        public AppUser User { get; set; }

        public string Subject { get; set; }

        [Required]
        public string? Message { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public int Rating { get; set; } = 0;

        public int Likes { get; set; } = 0;

        public IEnumerable<FeedbackImage>? Images { get; set; }

    }
}
