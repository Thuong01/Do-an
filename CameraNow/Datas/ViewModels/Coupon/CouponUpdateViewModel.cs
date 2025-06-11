using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels.Coupon
{
    public class CouponUpdateViewModel
    {
        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Giá trị không được để trống")]
        [Display(Name = "Value")]
        public decimal Value { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Type")]
        public CouponType Type { get; set; }

        [Display(Name = "MinOrderAmount")]
        public decimal MinOrderAmount { get; set; }

        [Display(Name = "MaxDiscountAmount")]
        public decimal MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [Display(Name = "StartDate")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [Display(Name = "EndDate")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "UsedCount")]
        public int UsedCount { get; set; }

        [Display(Name = "ApplyForProductIds")]
        public List<string>? ApplyForProductIds { get; set; }

        [Display(Name = "ApplyForCategoryIds")]
        public List<string>? ApplyForCategoryIds { get; set; }
    }
}
