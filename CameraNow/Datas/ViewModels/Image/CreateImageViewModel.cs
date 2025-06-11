using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels.Image
{
    public class CreateImageViewModel
    {
        [Display(Name = "Link")]
        public string Link { get; set; }

        [Display(Name = "Product_Id")]
        public Guid Product_Id { get; set; }
    }
}
