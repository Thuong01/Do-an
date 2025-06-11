using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels
{
    public class CreateProductCategoryViewModel : AuditableViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        //[Display(Name = "Alias")]
        //[Required]
        //public string? Alias { get; set; }

        [Display(Name = "Parent_ID")]
        public Guid? Parent_ID { get; set; }

        [Display(Name = "Image")]
        public IFormFile? Image { get; set; }

        public string? Image_Public_Id { get; set; }
    }
}
