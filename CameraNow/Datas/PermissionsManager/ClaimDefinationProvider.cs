namespace Datas.PermissionsManager
{
    public class ClaimDefinationProvider
    {
        public const string GroupName = "Permissions";

        public static class ProductClaim
        {
            public const string Default = GroupName + ".ProductCategory";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
            public const string CanView = Default + ".Canview";
        }
    }
}
