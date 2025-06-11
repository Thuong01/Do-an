using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("UsedCoupons")]
    public class UsedCoupon
    {
        [Key]
        public int Id { get; set; }
        public string CouponCode { get; set; }
        public string UserId { get; set; }
    }
}
