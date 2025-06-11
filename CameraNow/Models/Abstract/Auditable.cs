using Models.Enums;

namespace Models.Abstract
{
    public abstract class Auditable : IAuditable
    {
        public DateTime? Creation_Date { get; set; }
        public string? Creation_By { get; set; }
        public DateTime? Last_Modify_Date { get; set; }
        public string? Last_Modify_By { get; set; }
        public string? Meta_KeyWord { get; set; }
        public string? Meta_Description { get; set; }
        public Status Status { get; set; }
    }
}
