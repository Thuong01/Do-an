using Microsoft.AspNetCore.Http;
using Commons.AttributesCustom;
using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels.Product
{
    public class CreateProductViewModel : AuditableViewModel
    {
        public Guid ID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Alias")]
        public string? Alias { get; set; }

        [Required]
        [Display(Name = "Image")]
        public IFormFile Image { get; set; }

        [Display(Name = "MoreImage")]
        public List<IFormFile>? MoreImage { get; set; }

        [Display(Name = "Category_ID")]
        public Guid? Category_ID { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [LessThan("Price")]
        [Display(Name = "Promotion_Price")]
        public decimal? Promotion_Price { get; set; }
        [Display(Name = "Rating")]
        public double Rating { get; set; }

        //public Guid? Brand_ID { get; set; }
    }
}
