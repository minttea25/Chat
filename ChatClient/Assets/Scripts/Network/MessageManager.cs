using System;
using System.Collections.Generic;

using Google.Protobuf;

using ServerCoreTCP;
using ServerCoreTCP.MessageWrapper;
using ServerCoreTCP.Utils;

namespace ChatServer.Chat
{
    public class MessageManager
    {
        #region Singleton
        static MessageManager _instance = null;
        public static MessageManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MessageManager();
                return _instance;
            }
        }
        #endregion

#if PACKET_TYPE_INT
        readonly Dictionary<uint, MessageParser> _parsers = new Dictionary<uint, MessageParser>();
        readonly Dictionary<uint, Action<IMessage, Session>> _handlers = new Dictionary<uint, Action<IMessage, Session>>();
#else
        readonly Dictionary<ushort, MessageParser> _parsers = new Dictionary<ushort, MessageParser>();
        readonly Dictionary<ushort, Action<IMessage, Session>> _handlers = new Dictionary<ushort, Action<IMessage, Session>>();
#endif

        MessageManager()
        {
        }

        /// <summary>
        /// Must be called before use MessageManager in multi-thread environment.
        /// </summary>
        public void Init()
        {
#if PACKET_TYPE_INT
            MessageWrapper.PacketMap.Add(typeof(SSendChatText), (uint)PacketType.P_SSendChatText);
            MessageWrapper.PacketMap.Add(typeof(SSendChatIcon), (uint)PacketType.P_SSendChatIcon);
            MessageWrapper.PacketMap.Add(typeof(CSendChat), (uint)PacketType.P_CSendChat);
            MessageWrapper.PacketMap.Add(typeof(CChatText), (uint)PacketType.P_CChatText);
            MessageWrapper.PacketMap.Add(typeof(CSendIcon), (uint)PacketType.P_CSendIcon);
            MessageWrapper.PacketMap.Add(typeof(ChatBase), (uint)PacketType.P_ChatBase);
            MessageWrapper.PacketMap.Add(typeof(ChatText), (uint)PacketType.P_ChatText);
            MessageWrapper.PacketMap.Add(typeof(ChatIcon), (uint)PacketType.P_ChatIcon);
            MessageWrapper.PacketMap.Add(typeof(SPingPacket), (uint)PacketType.P_SPingPacket);
            MessageWrapper.PacketMap.Add(typeof(CPongPacket), (uint)PacketType.P_CPongPacket);
            MessageWrapper.PacketMap.Add(typeof(SCreateRoomReq), (uint)PacketType.P_SCreateRoomReq);
            MessageWrapper.PacketMap.Add(typeof(CCreateRoomRes), (uint)PacketType.P_CCreateRoomRes);
            MessageWrapper.PacketMap.Add(typeof(SEnterRoomReq), (uint)PacketType.P_SEnterRoomReq);
            MessageWrapper.PacketMap.Add(typeof(CEnterRoomRes), (uint)PacketType.P_CEnterRoomRes);
            MessageWrapper.PacketMap.Add(typeof(SAllRoomListReq), (uint)PacketType.P_SAllRoomListReq);
            MessageWrapper.PacketMap.Add(typeof(SRoomListReq), (uint)PacketType.P_SRoomListReq);
            MessageWrapper.PacketMap.Add(typeof(CRoomListRes), (uint)PacketType.P_CRoomListRes);
            MessageWrapper.PacketMap.Add(typeof(SLeaveRoomReq), (uint)PacketType.P_SLeaveRoomReq);
            MessageWrapper.PacketMap.Add(typeof(CUserLeftRoom), (uint)PacketType.P_CUserLeftRoom);
            MessageWrapper.PacketMap.Add(typeof(SLoginReq), (uint)PacketType.P_SLoginReq);
            MessageWrapper.PacketMap.Add(typeof(CLoginRes), (uint)PacketType.P_CLoginRes);

#else
            MessageWrapper.PacketMap.Add(typeof(SSendChatText), (ushort)PacketType.P_SSendChatText);
            MessageWrapper.PacketMap.Add(typeof(SSendChatIcon), (ushort)PacketType.P_SSendChatIcon);
            MessageWrapper.PacketMap.Add(typeof(CSendChat), (ushort)PacketType.P_CSendChat);
            MessageWrapper.PacketMap.Add(typeof(CChatText), (ushort)PacketType.P_CChatText);
            MessageWrapper.PacketMap.Add(typeof(CSendIcon), (ushort)PacketType.P_CSendIcon);
            MessageWrapper.PacketMap.Add(typeof(ChatBase), (ushort)PacketType.P_ChatBase);
            MessageWrapper.PacketMap.Add(typeof(ChatText), (ushort)PacketType.P_ChatText);
            MessageWrapper.PacketMap.Add(typeof(ChatIcon), (ushort)PacketType.P_ChatIcon);
            MessageWrapper.PacketMap.Add(typeof(SPingPacket), (ushort)PacketType.P_SPingPacket);
            MessageWrapper.PacketMap.Add(typeof(CPongPacket), (ushort)PacketType.P_CPongPacket);
            MessageWrapper.PacketMap.Add(typeof(SCreateRoomReq), (ushort)PacketType.P_SCreateRoomReq);
            MessageWrapper.PacketMap.Add(typeof(CCreateRoomRes), (ushort)PacketType.P_CCreateRoomRes);
            MessageWrapper.PacketMap.Add(typeof(SEnterRoomReq), (ushort)PacketType.P_SEnterRoomReq);
            MessageWrapper.PacketMap.Add(typeof(CEnterRoomRes), (ushort)PacketType.P_CEnterRoomRes);
            MessageWrapper.PacketMap.Add(typeof(SAllRoomListReq), (ushort)PacketType.P_SAllRoomListReq);
            MessageWrapper.PacketMap.Add(typeof(SRoomListReq), (ushort)PacketType.P_SRoomListReq);
            MessageWrapper.PacketMap.Add(typeof(CRoomListRes), (ushort)PacketType.P_CRoomListRes);
            MessageWrapper.PacketMap.Add(typeof(SLeaveRoomReq), (ushort)PacketType.P_SLeaveRoomReq);
            MessageWrapper.PacketMap.Add(typeof(CUserLeftRoom), (ushort)PacketType.P_CUserLeftRoom);
            MessageWrapper.PacketMap.Add(typeof(SLoginReq), (ushort)PacketType.P_SLoginReq);
            MessageWrapper.PacketMap.Add(typeof(CLoginRes), (ushort)PacketType.P_CLoginRes);

#endif

            _parsers.Add(MessageWrapper.PacketMap[typeof(CSendChat)], CSendChat.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CSendChat)], MessageHandler.CSendChatMessageHandler);

            _parsers.Add(MessageWrapper.PacketMap[typeof(CChatText)], CChatText.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CChatText)], MessageHandler.CChatTextMessageHandler);

            _parsers.Add(MessageWrapper.PacketMap[typeof(CSendIcon)], CSendIcon.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CSendIcon)], MessageHandler.CSendIconMessageHandler);

            _parsers.Add(MessageWrapper.PacketMap[typeof(CPongPacket)], CPongPacket.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CPongPacket)], MessageHandler.CPongPacketMessageHandler);

            _parsers.Add(MessageWrapper.PacketMap[typeof(CCreateRoomRes)], CCreateRoomRes.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CCreateRoomRes)], MessageHandler.CCreateRoomResMessageHandler);

            _parsers.Add(MessageWrapper.PacketMap[typeof(CEnterRoomRes)], CEnterRoomRes.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CEnterRoomRes)], MessageHandler.CEnterRoomResMessageHandler);

            _parsers.Add(MessageWrapper.PacketMap[typeof(CRoomListRes)], CRoomListRes.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CRoomListRes)], MessageHandler.CRoomListResMessageHandler);

            _parsers.Add(MessageWrapper.PacketMap[typeof(CUserLeftRoom)], CUserLeftRoom.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CUserLeftRoom)], MessageHandler.CUserLeftRoomMessageHandler);

            _parsers.Add(MessageWrapper.PacketMap[typeof(CLoginRes)], CLoginRes.Parser);
            _handlers.Add(MessageWrapper.PacketMap[typeof(CLoginRes)], MessageHandler.CLoginResMessageHandler);


        }

#if PACKET_TYPE_INT
        /// <summary>
        /// Assemble the data to message and handles the result according to the Paceket Type.
        /// </summary>
        /// <param name="session">The session that received the data.</param>
        /// <param name="buffer">The buffer that contains the packet type and serialized message.</param>
        /// <param name="callback">The another callback function, not PacketHandler.</param>
        public void OnRecvPacket(Session session, ReadOnlySpan<byte> buffer, Action<uint, Session, IMessage> callback = null)
        {
            // Note: buffer contains the type (uint or ushort) and serialized message.
            uint packetType = ReadPacketType(buffer);

            if (_parsers.TryGetValue(packetType, out var parser))
            {
                var msg = parser.ParseFrom(buffer.Slice(Defines.PACKET_DATATYPE_SIZE));

                if (callback != null) callback.Invoke(packetType, session, msg);
                else HandlePacket(packetType, msg, session);
            }
        }
#else
        /// <summary>
        /// Assemble the data to message and handles the result according to the Paceket Type.
        /// </summary>
        /// <param name="session">The session that received the data.</param>
        /// <param name="buffer">The buffer that contains the packet type and serialized message.</param>
        /// <param name="callback">The another callback function, not PacketHandler.</param>
        public void OnRecvPacket(Session session, ReadOnlySpan<byte> buffer, Action<ushort, Session, IMessage> callback = null)
        {
            // Note: buffer contains the type (uint or ushort) and serialized message.
            ushort packetType = ReadPacketType(buffer);

            if (_parsers.TryGetValue(packetType, out var parser))
            {
                var msg = parser.ParseFrom(buffer.Slice(Defines.PACKET_DATATYPE_SIZE));

                if (callback != null) callback.Invoke(packetType, session, msg);
                else HandlePacket(packetType, msg, session);
            }
        }
#endif

#if PACKET_TYPE_INT
        static ushort ReadPacketType(ReadOnlySpan<byte> buffer)
        {
            return buffer.ToUInt16();
        }

        void HandlePacket(uint packetType, IMessage msg, Session session)
        {
            if (_handlers.TryGetValue(packetType, out var handler))
            {
                handler.Invoke(msg, session);
            }
        }
#else
        static ushort ReadPacketType(ReadOnlySpan<byte> buffer)
        {
            return buffer.ToUInt16();
        }

        public void HandlePacket(ushort packetType, IMessage msg, Session session)
        {
            if (_handlers.TryGetValue(packetType, out var handler))
            {
                handler.Invoke(msg, session);
            }
        }
#endif
    }
}
