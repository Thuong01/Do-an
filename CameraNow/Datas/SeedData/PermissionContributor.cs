using Datas.Data;
using Datas.PermissionsManager;

namespace Datas.SeedData
{
    public static class PermissionContributor
    {
        public static void Define (CameraNowContext context)
        {
			try
			{
                var myGroup = context.AddGroup(ClaimDefinationProvider.GroupName);

                var productPermission = context.AddPermission(myGroup, ClaimDefinationProvider.ProductClaim.Default);
                context.AddChild(productPermission, ClaimDefinationProvider.ProductClaim.Create);
                context.AddChild(productPermission, ClaimDefinationProvider.ProductClaim.Delete);
                context.AddChild(productPermission, ClaimDefinationProvider.ProductClaim.Update);
                context.AddChild(productPermission, ClaimDefinationProvider.ProductClaim.CanView);
            }
			catch (Exception ex)
			{
                Console.WriteLine(ex.ToString());
			}
        }

    }
}
