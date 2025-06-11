using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Carts")]
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        public string User_Id { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

    }
}
