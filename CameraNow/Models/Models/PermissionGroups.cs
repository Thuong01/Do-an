using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Permission_Groups")]
    public class PermissionGroups
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
