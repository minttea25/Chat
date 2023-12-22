using Chat;
using ServerCoreTCP.Job;
using ServerCoreTCP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public partial class RoomManager : JobSerializerWithTimer, IUpdate
    {
        public void HandleCreateRoom(ClientSession session, SCreateRoomReq req)
        {
            Add(() =>
            {
                if (Rooms.ContainsKey(req.RoomId) == true)
                {
                    CCreateRoomRes res = new()
                    {
                        Res = CreateRoomRes.CreateRoomDuplicatedRoomId
                    };
                    session.Send(res);
                }
                else
                {
                    // 방 생성 및 입장 한번에 하기 (한번에 안하면 비어있는 방으로 간주될 수 있음)
                    // TODO : 방 생성 요청에 이름추가하기
                    CreateRoom(req.RoomId, req.RoomName);

                    // 로직이 방체크(timer) -> RoomManager.Update -> Room.Update 
                    // 방생성 후에 비어있는 방체크하지 않음
                    HandleEnterRoom(session, req.RoomId);
                }
            });
        }

        public void HandleEnterRoom(ClientSession session, ulong roomId)
        {
            Add(() =>
            {
                if (Rooms.ContainsKey(roomId) == false)
                {
                    CEnterRoomRes res = new()
                    {
                        Res = EnterRoomRes.EnterRoomNoSuchRoom,
                        RoomInfo = null,
                    };
                    session.Send(res);
                }
                else Rooms[roomId].HandleEnterRoom(session);
            });
        }

        public void HandleLeaveRoom(ClientSession session, ulong roomId)
        {
            Add(() =>
            {
                if (Rooms.ContainsKey(roomId) == false)
                {
                    // wrong request

                    // TODO : Error Handling
                    return;
                }
                else
                {
                    Rooms[roomId].HandleLeaveRoom(session.UserInfo);
                }
            });
        }

    }
}
