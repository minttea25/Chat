using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatSharedDb
{
    [Table("AutoToken")]
    public class AuthTokenDb
    {
        [Key]
        public long TokenDbId { get; set; }
        public long AccountDbId { get; set; }
        public string? Token { get; set; }
        public DateTime Expired { get; set; }
        public string? RecentIpAddress { get; set; }
    }
}
