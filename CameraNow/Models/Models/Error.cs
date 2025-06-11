
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    [Table("Errors")]
    public class Error
    {
        [Key]
        public Guid ID { set; get; }
        public string Message { set; get; }
        public string Stack_Trace { set; get; }
        public DateTime Creation_Date { set; get; }
    }
}
