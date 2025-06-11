using Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datas.ViewModels
{
    public class AuditableViewModel
    {
        [Display(Name = "Creation_Date")]
        public DateTime? Creation_Date { get; set; }

        [Display(Name = "Creation_By")]
        public string? Creation_By { get; set; }

        [Display(Name = "Last_Modify_Date")]
        public DateTime? Last_Modify_Date { get; set; }

        [Display(Name = "Last_Modify_By")]
        public string? Last_Modify_By { get; set; }

        [Display(Name = "Meta_KeyWord")]
        public string? Meta_KeyWord { get; set; }

        [Display(Name = "Meta_Description")]
        public string? Meta_Description { get; set; }

        [Display(Name = "Status")]
        public Status Status { get; set; }

        [NotMapped]
        public DateTime? Local_Creation_Date => Creation_Date?.ToLocalTime();

        [NotMapped]
        public DateTime? Local_Last_Modify_Date => Last_Modify_Date?.ToLocalTime();
    }
}
