namespace Datas.ViewModels.Order
{
    public class OrderDetailUpdateViewModel
    {
        public Guid Order_ID { get; set; }
        public Guid Product_ID { get; set; }
        public int Quantity { get; set; }
    }
}
