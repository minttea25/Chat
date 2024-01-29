using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServer.DB
{
    [Table("Account")]
    public class AccountDb
    {
        [Key]
        public long AccountDbId { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public ulong UserDbId { get; set; }
    }

    [Table("ChatServerIp")]
    public class ChatServerIpDb
    {
        [Key]
        public long ChatServerIpDbId { get; set; }
        public string? ChatServerName { get; set; }
        public string? ChatServerIp { get; set; }
        public int ? ChatServerPort { get; set; }
        public bool ? IsOnline { get; set; }
        public int ? Status { get; set; }
    }
}
