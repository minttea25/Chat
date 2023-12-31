using System;

using ServerCoreTCP;
using Google.Protobuf;
using ServerCoreTCP.Core;
using System.Diagnostics;
using Core;

namespace Chat
{
    public class MessageHandler
    {
        public static void CSendChatMessageHandler(IMessage message, Session session)
        {
            CSendChat msg = message as CSendChat;

            // TODO
        }

        public static void CChatTextMessageHandler(IMessage message, Session session)
        {
            CChatText msg = message as CChatText;

            // TODO
        }

        public static void CSendIconMessageHandler(IMessage message, Session session)
        {
            CSendIcon msg = message as CSendIcon;

            // TODO
        }

        public static void CCreateRoomResMessageHandler(IMessage message, Session session)
        {
            CCreateRoomRes msg = message as CCreateRoomRes;

            // TODO
        }

        public static void CEnterRoomResMessageHandler(IMessage message, Session session)
        {
            CEnterRoomRes msg = message as CEnterRoomRes;

            // TODO
        }

        public static void CRoomListResMessageHandler(IMessage message, Session session)
        {
            CRoomListRes msg = message as CRoomListRes;

            // TODO
        }

        public static void CUserLeftRoomMessageHandler(IMessage message, Session session)
        {
            CUserLeftRoom msg = message as CUserLeftRoom;

            // TODO
        }

        public static void CLoginResMessageHandler(IMessage message, Session session)
        {
            CLoginRes msg = message as CLoginRes;

            // TODO
        }

        public static void CPongPacketMessageHandler(IMessage message, Session session)
        {
            CPongPacket msg = message as CPongPacket;
            ServerSession s = session as ServerSession;

            // TODO
            var pongTick = Global.G_Stopwatch.ElapsedMilliseconds;
            var pingTick = ManagerCore.Scene.GetScene<MainScene>().PingTick;
            UnityEngine.Debug.Log($"ping: {pongTick - pingTick} ms");
        }
    }
}
