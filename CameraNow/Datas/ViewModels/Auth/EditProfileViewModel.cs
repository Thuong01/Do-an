using System.ComponentModel;

namespace Datas.ViewModels.Auth
{
    public class EditProfileViewModel
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [DisplayName("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [DisplayName("FullName")]
        public string FullName { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Birthday")]
        public string Birthday { get; set; }
        public bool Delete_yn { get; set; } = false;
        public bool Image_yn { get; set; } = false;
    }
}
