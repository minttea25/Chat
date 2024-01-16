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
}
