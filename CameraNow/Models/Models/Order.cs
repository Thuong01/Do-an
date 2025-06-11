using Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [Required]
        public Guid ID { get; set; }
        [Required]
        public string OrderNo { get; set; }
        public string FullName { get; set; }
        [Required]
        public string User_Id { get; set; }
        [ForeignKey("User_Id")]
        public AppUser User { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? Message { get; set; }
        [Required]
        [MaxLength(256)]
        public string Payment_Method { get; set; }
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.DangXuLy;
        public decimal Total_Amount { get; set; }
        public int? Coupon_Id { get; set; }
        public bool IsCommented { get; set; }
        public DateTime Order_Date { get; set; }
        public string? TransactionNo { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
