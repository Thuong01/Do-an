using Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Coupons")]
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Value { get; set; }
        public string? Description { get; set; }
        public CouponType Type { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Quantity { get; set; }
        public int UsedCount { get; set; }
        public List<string> ApplyForProductIds { get; set; }
        public List<string> ApplyForCategoryIds { get; set; }
    }
}
