using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Permissions")]
    public class Permissions
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string GroupName { get; set; }
        public string Name { get; set; }
        public string? ParentName { get; set; }
    }
}
