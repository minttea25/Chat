using Core;
using System;
using TMPro;
using UnityEngine;

[Serializable]
class RoomListItemUIContext : UIContext
{
    public UIObject<TextMeshProUGUI> RoomNumberText = new();
    public UIObject<TextMeshProUGUI> RoomNameText = new();
}

public class RoomListItemUI : BaseUIItem
{
    [SerializeField]
    RoomListItemUIContext Context;

    public readonly Property<string, TextMeshProUGUI> RoomNumber = new();
    public readonly Property<string, TextMeshProUGUI> RoomName = new();

    public override void Init()
    {
        base.Init();

        RoomNumber.AddUI(Context.RoomNumberText);
        RoomName.AddUI(Context.RoomNameText);
    }

    public void SetRoomNumber(ulong roomNumber)
    {
        RoomNumber.Data = $"No. {roomNumber}";
    }

    public void SetRoomName(string roomName)
    {
        RoomName.Data = roomName;
    }

    void Clear()
    {
        RoomNumber.Data = string.Empty;
        RoomName.Data = string.Empty;
    }


#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(RoomListItemUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
