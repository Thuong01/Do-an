namespace Web.Admin.Models
{
    public class MenuItem
    {
        public MenuItem()
        {
            SubMenus = new List<MenuItem>();
        }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public List<MenuItem> SubMenus { get; set; }
    }
}
