
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Order_Details")]
    public class OrderDetail
    {
        [Key]
        [Required]
        [Column(Order = 1)]
        public Guid Order_ID { get; set; }
        [ForeignKey("Order_ID")]
        public virtual Order Order { get; set; }
        [Key]
        [Required]
        [Column(Order = 2)]
        public Guid Product_ID { get; set; }
        [ForeignKey("Product_ID")]
        public virtual Product Product { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
