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

            // TODO
        }

        public static void SSendChatIconMessageHandler(IMessage message, Session session)
        {
            SSendChatIcon msg = message as SSendChatIcon;

            // TODO
        }

        public static void SPingPacketMessageHandler(IMessage message, Session session)
        {
            SPingPacket msg = message as SPingPacket;

            // TODO
        }

        public static void SCreateRoomReqMessageHandler(IMessage message, Session session)
        {
            SCreateRoomReq msg = message as SCreateRoomReq;

            // TODO
        }

        public static void SEnterRoomReqMessageHandler(IMessage message, Session session)
        {
            SEnterRoomReq msg = message as SEnterRoomReq;

            // TODO
        }

        public static void SRoomListReqMessageHandler(IMessage message, Session session)
        {
            SRoomListReq msg = message as SRoomListReq;

            // TODO
        }

        public static void SLeaveRoomReqMessageHandler(IMessage message, Session session)
        {
            SLeaveRoomReq msg = message as SLeaveRoomReq;

            // TODO
        }

        public static void SLoginReqMessageHandler(IMessage message, Session session)
        {
            SLoginReq msg = message as SLoginReq;

            // TODO
        }


    }
}
