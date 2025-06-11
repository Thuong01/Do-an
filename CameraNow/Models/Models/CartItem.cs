using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public Guid Product_Id { get; set; }
        [ForeignKey(nameof(Product_Id))]
        public Product Product { get; set; }
        public Guid Cart_Id { get; set; }
        [ForeignKey(nameof(Cart_Id))]
        public Cart Cart { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
