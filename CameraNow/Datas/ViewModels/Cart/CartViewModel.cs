namespace Datas.ViewModels.Cart
{
    public class CartViewModel
    {
        public Guid Id { get; set; }

        public string User_Id { get; set; }

        public decimal TotalPrice { get; set; }

        public ICollection<CartItemViewModel> Items { get; set; }
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }

        public Guid Product_Id { get; set; }

        public string Product_Name { get; set; }

        public decimal Product_Price { get; set; }

        public string Product_Image { get; set; }

        public Guid Cart_Id { get; set; }

        public int Quantity { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }

        public decimal TotalItemPrice { get; set; }
    }
}
