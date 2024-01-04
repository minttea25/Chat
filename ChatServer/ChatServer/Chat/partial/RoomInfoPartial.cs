using Chat.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public sealed partial class RoomInfo
    {
        public static RoomInfo FromChatRoomDb(ChatRoomDb room, bool containUser = false)
        {
            var info = new RoomInfo()
            {
                RoomDbId = room.ChatRoomDbId,
                RoomName = room.ChatRoomName,
                RoomNumber = room.ChatRoomNumber,
            };

            if (containUser == true && room.Users != null)
            {
                foreach (var user in room.Users)
                {
                    info.Users.Add(UserInfo.FromUserDb(user));
                }
            }

            return info;
        }
    }

    public sealed partial class UserInfo
    {
        public static UserInfo FromUserDb(UserDb user, string loginId = null)
        {
            return new UserInfo()
            {
                UserDbId = user.UserDbId,
                UserName = user.UserName,
                UserLoginId = loginId ?? string.Empty,
            };
        }
    }
}
