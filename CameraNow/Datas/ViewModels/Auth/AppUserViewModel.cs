using System.ComponentModel;

namespace Datas.ViewModels.Auth
{
    public class AppUserViewModel
    {
        public string Id { get; set; }
        [DisplayName("FullName")]
        public string FullName { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        public string Avartar { get; set; }
        public string PublicId { get; set; }
        [DisplayName("UserName")]
        public string UserName { get; set; }
        [DisplayName("Birthday")]
        public DateTime? Birthday { get; set; }
        [DisplayName("Email")]
        public string? Email { get; set; }
        [DisplayName("PhoneNumber")]
        public string? PhoneNumber { get; set; }
        public Guid? CartId { get; set; }
    }
}
