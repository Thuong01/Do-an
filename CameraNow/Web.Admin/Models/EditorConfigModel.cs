namespace Web.Admin.Models
{
    public class EditorConfigModel
    {
        public EditorConfigModel()
        {
        }

        public EditorConfigModel(string selector)
        {
            Selector = selector;
        }

        //public int Height { get; set; }
        public string Selector { get; set; }
    }
}
