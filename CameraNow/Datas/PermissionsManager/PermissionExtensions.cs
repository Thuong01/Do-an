using Datas.Data;
using Models.Models;

namespace Datas.PermissionsManager
{
    public static class PermissionExtensions
    {
        public static PermissionGroups AddGroup(this CameraNowContext context, string groupName)
        {
            var myGroup = context.PermissionGroups.Where(x => x.Name == groupName).FirstOrDefault();

            if (myGroup == null)
            {
                myGroup = new PermissionGroups
                {
                    Name = ClaimDefinationProvider.GroupName,
                };

                context.PermissionGroups.Add(myGroup);
                context.SaveChanges();
            }

            return myGroup;
        }

        public static Permissions AddPermission (this CameraNowContext context, PermissionGroups group, string permissionName)
        {
            var permission = context.Permissions.Where(x => x.Name == permissionName).FirstOrDefault();

            if (permission == null)
            {
                permission = new Permissions
                {
                    Name = permissionName,
                    GroupName = group.Name
                };

                context.Permissions.Add(permission);
                context.SaveChanges();
            }

            return permission;
        }

        public static Permissions AddChild(this CameraNowContext context, Permissions parent, string permissionName)
        {
            var child = context.Permissions.Where(x => x.Name == permissionName && x.ParentName == parent.Name).FirstOrDefault();

            if (child == null)
            {
                child = new Permissions
                {
                    Name = permissionName,
                    GroupName = parent.GroupName,
                    ParentName = parent.Name
                };

                context.Permissions.Add(child);
                context.SaveChanges();
            }

            return child;
        }
    }
}
