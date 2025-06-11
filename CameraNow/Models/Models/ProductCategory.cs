
using Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Product_Categories")]
    public class ProductCategory : Auditable
    {
        [Key]
        [Required]
        public Guid ID { get; set; }

        [Required]
        [MaxLength(501)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Alias { get; set; }

        public Guid? Parent_ID { get; set; }

        public string? Image { get; set; }

        public string? Image_Public_Id { get; set; }

        public virtual IEnumerable<Product> Products { get; set; }
    }
}
