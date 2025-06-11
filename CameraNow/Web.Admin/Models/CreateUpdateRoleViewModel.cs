using Microsoft.AspNetCore.Identity;

namespace Web.Admin.Models
{
    public class CreateUpdateRoleViewModel
    {
        public CreateUpdateRoleViewModel()
        {
            IdentityRole = new IdentityRole();
        }

        public IdentityRole IdentityRole { get; set; }
        public bool IsUpdate { get; set; }
    }
}
