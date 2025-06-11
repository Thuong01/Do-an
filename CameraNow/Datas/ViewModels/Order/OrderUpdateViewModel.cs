using Models.Enums;

namespace Datas.ViewModels.Order
{
    public class OrderUpdateViewModel
    {
        public Guid ID { get; set; }

        public string User_Id { get; set; }

        public string OrderNo { get; set; }

        public string FullName { get; set; }

        public string? Message { get; set; }

        public string Payment_Method { get; set; }

        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.DangXuLy;

        public decimal Total_Amount { get; set; }

        public int? Coupon_Id { get; set; }

        public DateTime Order_Date { get; set; }
        public bool IsCommented { get; set; }

        public ICollection<OrderDetailUpdateViewModel> OrderDetails { get; set; }
    }
}
