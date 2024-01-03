using Chat.DB;
using ServerCoreTCP.Job;
using ServerCoreTCP.Utils;

namespace Chat
{
    public partial class RoomManager : JobSerializerWithTimer, IUpdate
    {
        public void HandleCreateRoom(ClientSession session, SCreateRoomReq req)
        {
            DbProcess.CreateRoom(req.RoomNumber, req.RoomName, session);
        }

        public void HandleEnterRoom(ClientSession session, ulong roomNumber)
        {
            DbProcess.EnterRoom(roomNumber, session);

            // 방에 있는 다른 유저 broadcast
            Add(() =>
            {
                if (Rooms.ContainsKey(roomNumber) == false)
                {
                    // wrong request
                    // TODO : error
                    return;
                }
                else
                {
                    Rooms[roomNumber].HandleEnterRoom(session);
                }

            });
        }

        public void HandleLeaveRoom(ClientSession session, ulong roomNumber)
        {
            // TODO : RoomManager 에서 Room 캐싱 추가
            // 현재 Db에서 일일히 확인중...

            // db 처리 (room에서 user 삭제)
            DbProcess.LeaveRoom(session.UserInfo.UserDbId, roomNumber);

            // 방에 있는 다른 유저 broadcast
            Add(() =>
            {
                if (Rooms.ContainsKey(roomNumber) == false)
                {
                    // wrong request

                    // TODO : Error Handling
                    return;
                }
                else
                {
                    Rooms[roomNumber].HandleLeaveRoom(session.UserInfo);
                }
            });
        }

        public void HandleChatText(SSendChatText chat, ClientSession session)
        {
            Add(() =>
            {
                if (Rooms.ContainsKey(chat.RoomNumber) == false)
                {
                    // wrong request

                    // TODO : Error Handling
                    return;
                }
                else
                {
                    Rooms[chat.RoomNumber].HandleSendChatText(chat, session);
                }
            });
        }

        public void HandleChatIcon(SSendChatIcon chat, ClientSession session)
        {
            Add(() =>
            {
                if (Rooms.ContainsKey(chat.RoomNumber) == false)
                {
                    // wrong request

                    // TODO : Error Handling
                    return;
                }
                else
                {
                    // 전송 확인 패킷 전송


                    Rooms[chat.RoomNumber].HandleSendChatIcon(chat, session);
                }
            });
        }
    }
}
