using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels
{
    public class CreateBrandViewModel
    {
        public CreateBrandViewModel()
        {
        }

        public CreateBrandViewModel(string name, IFormFile? logo, string? logo_link)
        {
            Name = name;
            Logo = logo;
            Logo_Link = logo_link;
        }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Logo")]
        public IFormFile? Logo { get; set; }

        [Display(Name = "Logo_Link")]
        public string? Logo_Link { get; set; }

    }
}
