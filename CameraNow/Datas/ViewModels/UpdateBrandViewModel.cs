using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels
{
    public class UpdateBrandViewModel
    {
        public UpdateBrandViewModel()
        {
        }

        public UpdateBrandViewModel(Guid iD, string name, string? logo, string? logo_Public_Id)
        {
            ID = iD;
            Name = name;
            Logo = logo;
            Logo_Public_Id = logo_Public_Id;
        }

        public Guid ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Logo")]
        public string? Logo { get; set; }

        [Display(Name = "Logo_Public_Id")]
        public string? Logo_Public_Id { get; set; }
    }
}
