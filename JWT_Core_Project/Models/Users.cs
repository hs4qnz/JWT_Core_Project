using System.ComponentModel.DataAnnotations;

namespace JWT_Core_Project.Models
{
    public class Users
    {
        [Key]
        public int user_id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public DateTime? create_at { get; set; } = DateTime.Now;  //TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
        public int create_by { get; set; }
        public DateTime? updated_at { get; set; }
        public int? updated_by { get; set; }
        public bool isActive { get; set; }
        public int role_id { get; set; }

    }
}
