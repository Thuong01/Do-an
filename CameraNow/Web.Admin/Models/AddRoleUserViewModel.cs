namespace Web.Admin.Models
{
    public class AddRoleUserViewModel
    {
        public AddRoleUserViewModel()
        {
        }

        public AddRoleUserViewModel(string userId, string userName, List<string> roleName)
        {
            UserId = userId;
            UserName = userName;
            RoleName = roleName;
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<string> RoleName { get; set; }
    }
}
