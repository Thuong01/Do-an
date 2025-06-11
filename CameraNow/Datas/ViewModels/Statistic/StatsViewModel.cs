namespace Datas.ViewModels.Statistic
{
    public class StatsViewModel
    {
        // Tổng quan
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; }

        // Theo thời gian
        public Dictionary<string, decimal> RevenueByDay { get; set; }
        public Dictionary<string, decimal> RevenueByMonth { get; set; }

        // Sản phẩm
        public List<TopProduct> TopSellingProducts { get; set; }
        public List<ProductCombo> FrequentProductCombos { get; set; }

        // Khách hàng
        public List<TopCustomer> TopCustomers { get; set; }
        public List<RecentOrderDto> RecentOrders { get; set; }
    }

    public class RecentOrderDto
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }

    public class TopProduct
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImage { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ProductCombo
    {
        public string ProductPair { get; set; }
        public int ComboCount { get; set; }
    }

    public class TopCustomer
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
