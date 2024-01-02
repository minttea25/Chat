using System;

namespace Chat
{
#if PACKET_TYPE_INT
    public enum PacketType : uint
    {
        P_INVALID = 0,
        P_SSendChatText = 1,
        P_SSendChatIcon = 2,
        P_CSendChat = 3,
        P_CChatText = 4,
        P_CChatIcon = 5,
        P_ChatBase = 6,
        P_ChatText = 7,
        P_ChatIcon = 8,
        P_SPingPacket = 9,
        P_CPongPacket = 10,
        P_SCreateRoomReq = 11,
        P_CCreateRoomRes = 12,
        P_SEnterRoomReq = 13,
        P_CEnterRoomRes = 14,
        P_SRoomListReq = 15,
        P_CRoomListRes = 16,
        P_SLeaveRoomReq = 17,
        P_CUserEnterRoom = 18,
        P_CUserLeftRoom = 19,
        P_SLoginReq = 20,
        P_CLoginRes = 21,
        P_SEditUserNameReq = 22,
        P_CEditUserNameRes = 23,

    }
#else
    public enum PacketType : ushort
    {
        P_INVALID = 0,
        P_SSendChatText = 1,
        P_SSendChatIcon = 2,
        P_CSendChat = 3,
        P_CChatText = 4,
        P_CChatIcon = 5,
        P_ChatBase = 6,
        P_ChatText = 7,
        P_ChatIcon = 8,
        P_SPingPacket = 9,
        P_CPongPacket = 10,
        P_SCreateRoomReq = 11,
        P_CCreateRoomRes = 12,
        P_SEnterRoomReq = 13,
        P_CEnterRoomRes = 14,
        P_SRoomListReq = 15,
        P_CRoomListRes = 16,
        P_SLeaveRoomReq = 17,
        P_CUserEnterRoom = 18,
        P_CUserLeftRoom = 19,
        P_SLoginReq = 20,
        P_CLoginRes = 21,
        P_SEditUserNameReq = 22,
        P_CEditUserNameRes = 23,

    }
#endif
}
