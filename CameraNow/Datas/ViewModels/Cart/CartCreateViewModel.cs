namespace Datas.ViewModels.Cart
{
    public class CartCreateViewModel
    {
        public string User_Id { get; set; }
        public ICollection<CartItemCreateViewModel> CartItems { get; set; }
    }

    public class CartItemCreateViewModel
    {
        public Guid Product_Id { get; set; }
        public Guid Cart_Id { get; set; }
        public int Quantity { get; set; }
    }
}
