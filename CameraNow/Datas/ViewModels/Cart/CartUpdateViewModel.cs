namespace Datas.ViewModels.Cart
{
    public class CartUpdateViewModel
    {
        public Guid Id { get; set; }
        public string User_Id { get; set; }
        public ICollection<CartItemUpdateViewModel> CartItems { get; set; }
    }
    public class CartItemUpdateViewModel
    {
        public int Id { get; set; }
        public Guid Product_Id { get; set; }
        public Guid Cart_Id { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
    }
}
