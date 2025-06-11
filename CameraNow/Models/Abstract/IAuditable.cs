using Models.Enums;

namespace Models.Abstract
{
    public interface IAuditable
    {
        DateTime? Creation_Date { get; set; }
        string? Creation_By { get; set; }
        DateTime? Last_Modify_Date { get; set; }
        string? Last_Modify_By { get; set; }
        string? Meta_KeyWord { get; set; }
        string? Meta_Description { get; set; }
        Status Status { get; set; }
    }
}
