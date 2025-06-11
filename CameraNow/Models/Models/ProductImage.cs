using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Images")]
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string Link { get; set; }
        public string? Public_Id { get; set; }
        public Guid Product_Id { get; set; }
        public Product Product { get; set; }
    }
}
