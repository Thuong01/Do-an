using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels
{
    public class UpdateUserViewModel
    {
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "BirthDay")]
        public DateTime BirthDay { get; set; }

        [Display(Name = "FullName")]
        public string FullName { get; set; }
    }
}
