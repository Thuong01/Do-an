namespace Datas.ViewModels.Permissions
{
    public class PermissionViewModel
    {
        public Guid Id { get; set; }

        public string GroupName { get; set; }

        public string Name { get; set; }

        public string? ParentName { get; set; }

        public List<PermissionViewModel> PermissionsChild { get; set; }

        public PermissionViewModel PermissionParent { get; set; }
    }
}
