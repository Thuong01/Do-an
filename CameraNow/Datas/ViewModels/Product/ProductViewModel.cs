using System.ComponentModel.DataAnnotations;
using Datas.ViewModels.Image;

namespace Datas.ViewModels.Product
{
    public class ProductViewModel : AuditableViewModel
    {
        public Guid ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Contents")]
        public string? Contents { get; set; }

        [Display(Name = "Alias")]
        public string? Alias { get; set; }

        [Display(Name = "Image")]
        public string? Image { get; set; }

        public string? Image_Public_Id { get; set; }

        public IEnumerable<ImageViewModel> Images { get; set; }

        [Display(Name = "Buy_Count")]
        public int? Buy_Count { get; set; }

        [Display(Name = "Quantity")]
        public int? Quantity { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Promotion_Price")]
        public decimal? Promotion_Price { get; set; }

        public int Remaining { get; set; }

        public Guid? Category_ID { get; set; }
        public string Category_Name { get; set; }
        public string Category_Alias { get; set; }

        public double Rating { get; set; }

        //public Guid? Brand_ID { get; set; }

        //public string Brand_Name { get; set; }
    }
}
