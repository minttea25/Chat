using System;

using ServerCoreTCP;
using Google.Protobuf;

namespace Chat
{
    public class MessageHandler
    {
        public static void SSendChatTextMessageHandler(IMessage message, Session session)
        {
            SSendChatText msg = message as SSendChatText;
            ClientSession cs = session as ClientSession;

            // TODO
            Console.WriteLine(msg);
            cs?.HandleChatText(msg);
        }

        public static void SSendChatIconMessageHandler(IMessage message, Session session)
        {
            SSendChatIcon msg = message as SSendChatIcon;
            ClientSession cs = session as ClientSession;

            // TODO
            Console.WriteLine(msg);
            cs?.HandleChatIcon(msg);
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

            RoomManager.Instance.HandleEnterRoom(cs, msg!.RoomNumber);
        }

        public static void SRoomListReqMessageHandler(IMessage message, Session session)
        {
            SRoomListReq msg = message as SRoomListReq;
            ClientSession cs = session as ClientSession;

            // TEMP
            Console.WriteLine(msg);
            cs?.HandleRoomListReq();
        }

        public static void SLeaveRoomReqMessageHandler(IMessage message, Session session)
        {
            // SLeaveRoom은 단순히 '나 이방 나갈거에요~' 하고 끝나는 패킷으로
            // res 패킷 따로 없음
            // TODO : req 접미사 빼기
            SLeaveRoomReq msg = message as SLeaveRoomReq;
            ClientSession cs = session as ClientSession;

            // TEMP
            Console.WriteLine(msg);
            RoomManager.Instance.HandleLeaveRoom(cs, msg!.RoomNumber);
            
        }

        public static void SLoginReqMessageHandler(IMessage message, Session session)
        {
            SLoginReq msg = message as SLoginReq;
            ClientSession cs = session as ClientSession;

            // TEMP
            Console.WriteLine(msg);

            cs?.HandleLoginReq(msg);
        }

        public static void SEditUserNameReqMessageHandler(IMessage message, Session session)
        {
            SEditUserNameReq msg = message as SEditUserNameReq;
            ClientSession cs = session as ClientSession;

            // TODO
        }

        public static void SPingPacketMessageHandler(IMessage message, Session session)
        {
            SPingPacket msg = message as SPingPacket;
            ClientSession cs = session as ClientSession;

            cs?.Send(new CPongPacket());
        }
    }
}
