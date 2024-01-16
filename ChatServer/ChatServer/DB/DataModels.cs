using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.DB
{
    [Table("Account")]
    public class AccountDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountDbId { get; set; }
        public long LoginDbId { get; set; } // AccountDbId in AccountServer
        public ulong UserDbId { get; set; }

        [ForeignKey("UserDb")]
        public virtual UserDb User { get; set; }

    }

    [Table("User")]
    public class UserDb
    {
        [Key]
        public ulong UserDbId { get; set; }
        public string UserName { get; set; }

        public virtual ICollection<ChatDb> SentChats { get; set; } = new List<ChatDb>();

        public virtual ICollection<ChatRoomDb> Rooms { get; set; } = new List<ChatRoomDb>();

    }
    

    [Table("Chat")]
    public class ChatDb
    {
        [Key]
        public ulong ChatDbId { get; set; }
        public ushort ChatType { get; set; } // enum for ChatType
        public uint IconId { get; set; } = 0;
        public string Message { get; set; }
        public DateTime SendTime { get; set; }

        [ForeignKey("Sender")]
        public ulong SenderDbId { get; set; }
        public virtual UserDb Sender { get; set; }

        [ForeignKey("Room")]
        public ulong RoomDbId { get; set; }
        public virtual ChatRoomDb Room { get; set; }
    }

    [Table("ChatRoom")]
    public class ChatRoomDb
    {
        [Key]
        public ulong ChatRoomDbId { get; set; }
        public uint ChatRoomNumber { get; set; }
        public string ChatRoomName { get; set; }

        public virtual ICollection<UserDb> Users { get; set; } = new List<UserDb>();
        public virtual ICollection<ChatDb> Chats { get; set; } = new List<ChatDb>();
    }

}
