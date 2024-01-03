using System;

using ServerCoreTCP;
using Google.Protobuf;
using ServerCoreTCP.Core;
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

            ManagerCore.Network.HandleChatText(msg);
        }


        public static void CChatIconMessageHandler(IMessage message, Session session)
        {
            CChatIcon msg = message as CChatIcon;

            ManagerCore.Network.HandleChatIcon(msg);
        }

        public static void CCreateRoomResMessageHandler(IMessage message, Session session)
        {
            CCreateRoomRes msg = message as CCreateRoomRes;

            ManagerCore.Network.ResCreateRoom(msg);
        }

        public static void CEnterRoomResMessageHandler(IMessage message, Session session)
        {
            CEnterRoomRes msg = message as CEnterRoomRes;

            ManagerCore.Network.ResEnterRoom(msg);
        }

        public static void CRoomListResMessageHandler(IMessage message, Session session)
        {
            CRoomListRes msg = message as CRoomListRes;

            ManagerCore.Network.ResRoomList(msg);
        }

        public static void CUserEnterRoomMessageHandler(IMessage message, Session session)
        {
            CUserEnterRoom msg = message as CUserEnterRoom;

            ManagerCore.Network.HandleUserEnterRoom(msg);
        }

        public static void CUserLeftRoomMessageHandler(IMessage message, Session session)
        {
            CUserLeftRoom msg = message as CUserLeftRoom;

            ManagerCore.Network.HandleUserLeftRoom(msg);
        }

        public static void CLoginResMessageHandler(IMessage message, Session session)
        {
            CLoginRes msg = message as CLoginRes;

            ManagerCore.Network.ResLogin(msg);
        }

        public static void CEditUserNameResMessageHandler(IMessage message, Session session)
        {
            CEditUserNameRes msg = message as CEditUserNameRes;

            // TODO
        }

        public static void CPongPacketMessageHandler(IMessage message, Session session)
        {
            CPongPacket msg = message as CPongPacket;

            // TODO
            var pongTick = Global.G_Stopwatch.ElapsedMilliseconds;
            var pingTick = ManagerCore.Network.PingTick;
            //UnityEngine.Debug.Log($"ping: {pongTick - pingTick} ms");
            ManagerCore.Scene.GetScene<MainScene>().UI.SetPing(pongTick - pingTick);
        }
    }
}
