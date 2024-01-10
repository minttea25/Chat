using Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class MessageValidation
    {
        public static bool Validate_SLoginReq(SLoginReq req)
        {
            if (string.IsNullOrEmpty(req.UserInfo.UserLoginId)) return false;
            // TODO : check securities
            // TODO : Check auth token

            return true;
        }

        public static bool Validate_SSendChatText(SSendChatText msg)
        {
            if (string.IsNullOrEmpty(msg.Chat.Msg)) return false;
            return true;
        }

        public static bool Validate_SSendChatIcon(SSendChatIcon msg)
        {
            if (msg.Chat.IconId == 0) return false;
            return true;
        }

        public static bool Validate_SCreateRoomReq(SCreateRoomReq req)
        {
            if (req.RoomNumber == 0) return false;
            if (string.IsNullOrEmpty(req.RoomName)) return false;
            return true;
        }

        public static bool Validate_SLeaveRoomReq(SLeaveRoomReq req)
        {
            if (req.RoomNumber == 0) return false;
            return true;
        }

        public static bool Validate_SEnterRoomReq(SEnterRoomReq req)
        {
            if (req.RoomNumber == 0) return false;
            return true;
        }

        public static bool Validate_SEditUserNameReq(SEditUserNameReq req)
        {
            if (string.IsNullOrEmpty(req.UserInfo.UserName)) return false;
            // TODO : check regex

            return true;
        }
    }
}
