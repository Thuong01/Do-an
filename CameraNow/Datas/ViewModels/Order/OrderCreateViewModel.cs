using Models.Enums;

namespace Datas.ViewModels.Order
{
    public class OrderCreateViewModel
    {
        public string User_Id { get; set; }
        public string OrderNo { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? Message { get; set; }
        public string Payment_Method { get; set; }
        public decimal Total_Amount { get; set; }
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.DangXuLy;
        public int? Coupon_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public bool IsCommented { get; set; }
        public bool FromCart_YN { get; set; }
        public Guid CartID { get; set; }
        public ICollection<OrderDetailCreateViewModel> OrderDetails { get; set; }
    }
}
