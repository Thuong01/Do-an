using Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Products")]
    public class Product : Auditable
    {
        [Key]
        [Required]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Alias { get; set; }
        public string? Image { get; set; }
        public string? Image_Public_Id { get; set; }
        public int? Buy_Count { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? Promotion_Price { get; set; }
        public int Remaining { get; set; }
        public Guid? Category_ID { get; set; }
        [ForeignKey("Category_ID")]
        public virtual ProductCategory ProductCategory { get; set; }
        public double Rating { get; set; }
        //public Guid? Brand_ID { get; set; }
        //[ForeignKey("Brand_ID")]
        //public virtual Brand Brand { get; set; }
        public IEnumerable<ProductImage> Images { get; set; }
        public IEnumerable<Feedback> Feedbacks { get; set; }
    }
}
