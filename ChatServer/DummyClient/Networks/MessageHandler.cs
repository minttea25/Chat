using System;

using ServerCoreTCP;
using Google.Protobuf;
using System.Diagnostics;
using DummyClient;
using ServerCoreTCP.CLogger;

namespace Chat
{
    public class MessageHandler
    {
        public static void CSendChatMessageHandler(IMessage message, Session session)
        {
            CSendChat msg = message as CSendChat;

            //Console.WriteLine(msg);
            //CoreLogger.LogInfo("", msg.ToString());
        }

        public static void CChatTextMessageHandler(IMessage message, Session session)
        {
            CChatText msg = message as CChatText;

            //Console.WriteLine(msg);
            CoreLogger.LogInfo("", msg.ToString());
        }


        public static void CChatIconMessageHandler(IMessage message, Session session)
        {
            CChatIcon msg = message as CChatIcon;

            //Console.WriteLine(msg);
            CoreLogger.LogInfo("", msg.ToString());
        }

        public static void CCreateRoomResMessageHandler(IMessage message, Session session)
        {
            CCreateRoomRes msg = message as CCreateRoomRes;

            //Console.WriteLine(msg);
            CoreLogger.LogInfo("", msg.ToString());
        }

        public static void CEnterRoomResMessageHandler(IMessage message, Session session)
        {
            CEnterRoomRes msg = message as CEnterRoomRes;
            ServerSession ss = session as ServerSession;

            ss?.Rooms.Add(msg.RoomNumber);

            //Console.WriteLine(msg);
            CoreLogger.LogInfo("", msg.ToString());
        }

        public static void CRoomListResMessageHandler(IMessage message, Session session)
        {
            CRoomListRes msg = message as CRoomListRes;

            Console.WriteLine(msg);
        }

        public static void CUserEnterRoomMessageHandler(IMessage message, Session session)
        {
            CUserEnterRoom msg = message as CUserEnterRoom;
            Console.WriteLine(msg);
        }

        public static void CUserLeftRoomMessageHandler(IMessage message, Session session)
        {
            CUserLeftRoom msg = message as CUserLeftRoom;
            Console.WriteLine(msg);
        }

        public static void CLoginResMessageHandler(IMessage message, Session session)
        {
            CLoginRes msg = message as CLoginRes;
            ServerSession ss = session as ServerSession;

            Console.WriteLine(msg);
            ss.IsConnected = true;
            
        }

        public static void CEditUserNameResMessageHandler(IMessage message, Session session)
        {
            CEditUserNameRes msg = message as CEditUserNameRes;
        }

        public static void CPongPacketMessageHandler(IMessage message, Session session)
        {
            // CPongPacket msg = message as CPongPacket;
        }
    }
}
