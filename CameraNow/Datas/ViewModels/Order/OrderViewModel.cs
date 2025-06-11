using Models.Enums;
namespace Datas.ViewModels.Order
{
    public class OrderViewModel
    {
        public Guid ID { get; set; }
        public string UserId { get; set; }
        public string OrderNo { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? Message { get; set; }
        public string Email { get; set; }
        public string Payment_Method { get; set; }
        public OrderStatusEnum Status { get; set; }
        public decimal Total_Amount { get; set; }
        public int? Coupon_Id { get; set; }
        public decimal? CouponPercent { get; set; }
        public DateTime Order_Date { get; set; }
        public bool IsCommented { get; set; }
        public string? TransactionNo { get; set; }
        public IEnumerable<OrderDetailViewModel> OrderDetails { get; set; }
    }
    public class OrderDetailViewModel
    {
        public Guid Order_ID { get; set; }
        public Guid Product_ID { get; set; }
        public string Product_Name { get; set; }
        public decimal Product_Price { get; set; }
        public string Product_Image { get; set; }
        public int Quantity { get; set; }
    }
}
