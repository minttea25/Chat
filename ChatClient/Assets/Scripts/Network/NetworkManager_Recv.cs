using Chat;
using Core;
using ServerCoreTCP.Utils;

public partial class NetworkManager : IManager, IUpdate
{
    public void HandleChatText(CChatText chat)
    {
        if (chat.SenderInfo.UserDbId == UserInfo.UserDbId) return;

        ManagerCore.Room.AddChat(chat);
    }

    public void HandleChatIcon(CChatIcon chat)
    {
        ManagerCore.Room.AddChat(chat);
    }

    public void HandleUserEnterRoom(CUserEnterRoom msg)
    {
        ManagerCore.Room.UserEnter(msg);
    }

    public void HandleUserLeftRoom(CUserLeftRoom msg) 
    {
        ManagerCore.Room.UserLeft(msg);
    }
}
