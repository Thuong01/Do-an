using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Feedback_Images")]
    public class FeedbackImage
    {
        [Key]
        public int Id { get; set; }

        public string Link { get; set; }

        public string? Public_Id { get; set; }

        public Guid Feedback_Id { get; set; }

        [ForeignKey(nameof(Feedback_Id))]
        public Feedback Feedback { get; set; }

    }
}
