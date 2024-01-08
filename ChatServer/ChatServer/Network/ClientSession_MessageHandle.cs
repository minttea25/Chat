using Chat;
using Chat.DB;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using ServerCoreTCP.MessageWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public enum SessionStatus
    {
        NOT_LOGINNED = 0,
        LOGINNED = 1,
        DISCONNECTED = 2,
    }

    public partial class ClientSession : PacketSession
    {
        public long AccountDbId { get; private set; }
        public SessionStatus SessionStatus { get; private set; } = SessionStatus.NOT_LOGINNED;

        #region Authentication
        public void SetLoginned(AccountDb account, UserInfo info)
        {
            UserInfo = info;
            AccountDbId = account.AccountDbId;
            SessionStatus = SessionStatus.LOGINNED;
        }

        public void HandleLoginReq(SLoginReq req)
        {
            if (MessageValidation.Validate_SLoginReq(req) == false) return;

            using (AppDbContext db = new AppDbContext())
            {
                // 나중에 ulong id로 바꾸기 (일단 string 비교)
                AccountDb foundAccount = db.Accounts
                    .FirstOrDefault(a => a.AccountLoginId == req.UserInfo.UserLoginId);

                if (foundAccount != null)
                {
                    // assign the found id for AccountDbId
                    AccountDbId = foundAccount.AccountDbId;

                    // find user db
                    UserDb user = db.Users.FirstOrDefault(u => u.UserDbId == foundAccount.UserDbId);

                    if (user == null) return;

                    // assign the the userinfo
                    UserInfo info = UserInfo.FromUserDb(user, foundAccount.AccountLoginId);
                    SetLoginned(foundAccount, info);

                    CLoginRes res = new CLoginRes()
                    {
                        LoginRes = LoginRes.LoginSuccess,
                        UserInfo = UserInfo,
                    };

                    // TODO : Is anything to do more?

                    Send(res); // send directly OK.
                }
                else
                {
                    // TEST
                    DbProcess.CreateNewAccount(req.UserInfo.UserLoginId, req.UserInfo.UserName, this);
                    return;

                    // no acoount found
                    CLoginRes res = new CLoginRes()
                    {
                        LoginRes = LoginRes.LoginFailed,
                    };
                    Send(res); // send directly OK.
                }
            }
        }
        #endregion

        #region User Data
        public void HandleRoomListReq()
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;

            DbProcess.RoomListReq(this);
        }

        public void HandleEditUserNameReq(SEditUserNameReq req)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (req.UserInfo.UserDbId != UserInfo.UserDbId) return;
            if (MessageValidation.Validate_SEditUserNameReq(req) == false) return;

            DbProcess.EditUserName(this, req.UserInfo.UserDbId, req.NewUserName);
        }
        #endregion

        #region Chat

        public void HandleChatText(SSendChatText chat)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (chat.SenderInfo.UserDbId != UserInfo.UserDbId) return;
            if (MessageValidation.Validate_SSendChatText(chat) == false) return;

            // db 처리
            // 전송 성공 여부는 SaveChatText에 포함
            DbProcess.SaveChatText(UserInfo.UserDbId, chat, this);

            // 빠른 응답을 위해 성공 여부 상관 없이 바로 broad cast
            // db 저장과 동시 진행 가능
            

            // broadcast
            RoomManager.Instance.HandleChatText(chat, this);
        }

        public void HandleChatIcon(SSendChatIcon chat)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (chat.SenderInfo.UserDbId != UserInfo.UserDbId) return;
            if (MessageValidation.Validate_SSendChatIcon(chat) == false) return;

            // db 처리
            // 전송 성공 여부는 SaveChatIcon 포함
            DbProcess.SaveChatIcon(UserInfo.UserDbId, chat, this);

            // 빠른 응답을 위해 성공 여부 상관 없이 바로 broad cast
            // db 저장과 동시 진행 가능

            // broadcast
            RoomManager.Instance.HandleChatIcon(chat, this);
        }
        #endregion

        #region Rooms

        public void HandleCreateRoomReq(SCreateRoomReq req)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (MessageValidation.Validate_SCreateRoomReq(req) == false) return;

            RoomManager.Instance.HandleCreateRoom(this, req);
        }

        public void HandleEnterRoomReq(SEnterRoomReq req)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (MessageValidation.Validate_SEnterRoomReq(req) == false) return;

            RoomManager.Instance.HandleEnterRoom(this, req.RoomNumber);
        }

        public void HandleLeaveRoomReq(SLeaveRoomReq req)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (req.UserInfo.UserDbId != UserInfo.UserDbId) return;
            if (MessageValidation.Validate_SLeaveRoomReq(req) == false) return;

            RoomManager.Instance.HandleLeaveRoom(this, req.RoomNumber);
        }

        #endregion
    }
}
