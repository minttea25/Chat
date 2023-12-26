using System;

using ServerCoreTCP;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Diagnostics;
using ChatServer;

namespace Chat
{
    public class MessageHandler
    {
        public static void SSendChatTextMessageHandler(IMessage message, Session session)
        {
            SSendChatText msg = message as SSendChatText;

            // TODO
        }

        public static void SSendChatIconMessageHandler(IMessage message, Session session)
        {
            SSendChatIcon msg = message as SSendChatIcon;

            // TODO
        }

        public static void SCreateRoomReqMessageHandler(IMessage message, Session session)
        {
            SCreateRoomReq msg = message as SCreateRoomReq;
            ClientSession cs = session as ClientSession;

            // TEMP
            Console.WriteLine(msg);
            RoomManager.Instance.HandleCreateRoom(cs, msg);
        }

        public static void SEnterRoomReqMessageHandler(IMessage message, Session session)
        {
            SEnterRoomReq msg = message as SEnterRoomReq;
            ClientSession cs = session as ClientSession;

            // TEMP
            Console.WriteLine(msg);
            RoomManager.Instance.HandleEnterRoom(cs, msg.RoomId);
        }

        public static void SAllRoomListReqMessageHandler(IMessage message, Session session)
        {
            SAllRoomListReq msg = message as SAllRoomListReq;
            ClientSession cs = session as ClientSession;

            // TEMP
            Console.WriteLine(msg);
            CRoomListRes res = new();
            foreach(var roomInfo in RoomManager.Instance.GetRooms())
            {
                res.Rooms.Add(roomInfo);
            }
            res.LoadTime = Timestamp.FromDateTime(DateTime.UtcNow);
            // 바로 전송
            cs.Send(res);
        }

        public static void SRoomListReqMessageHandler(IMessage message, Session session)
        {
            SRoomListReq msg = message as SRoomListReq;
            ClientSession cs = session as ClientSession;

            // TEMP
            Console.WriteLine(msg);
            cs.HandleRoomListReq();
        }

        public static void SLeaveRoomReqMessageHandler(IMessage message, Session session)
        {
            // SLeaveRoom은 단순히 '나 이방 나갈거에요~' 하고 끝나는 패킷으로
            // res 패킷 따로 없음
            // req 접미사 빼기
            SLeaveRoomReq msg = message as SLeaveRoomReq;
            ClientSession cs = session as ClientSession;

            // TEMP
            Console.WriteLine(msg);
            RoomManager.Instance.HandleLeaveRoom(cs, msg.RoomId);
            
        }

        public static void SLoginReqMessageHandler(IMessage message, Session session)
        {
            SLoginReq msg = message as SLoginReq;
            ClientSession cs = session as ClientSession;

            // TEMP
             Console.WriteLine(msg);
            cs.UserInfo = msg.UserInfo;
            CLoginRes res = new()
            {
                LoginRes = LoginRes.LoginSuccess,
            };
            cs.Send(res);
        }

        public static void SPingPacketMessageHandler(IMessage message, Session session)
        {
            SPingPacket msg = message as SPingPacket;
            ClientSession cs = session as ClientSession;

            Console.WriteLine("ping");

            cs?.Send(new CPongPacket());
        }


    }
}
