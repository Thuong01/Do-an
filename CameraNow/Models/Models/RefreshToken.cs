using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string User_Id { get; set; }
        public string Token { get; set; }
        public string Jwt_Id { get; set; }
        public bool Is_Revoked { get; set; }
        public DateTime Date_Added { get; set; }
        public DateTime Date_Expire { get; set; }
        [ForeignKey("User_Id")]
        public AppUser User { get; set; }
    }
}
