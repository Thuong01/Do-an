using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.Models
{
    [Table("Brands")]
    public class Brand
    {
        [Key]
        [Required]
        public Guid ID { get; set; }

        [Column(TypeName = "character varying")]
        public string Name { get; set; }

        [Column(TypeName = "character varying")]
        public string? Logo { get; set; }

        public string? Logo_Public_Id { get; set; }
    }
}
