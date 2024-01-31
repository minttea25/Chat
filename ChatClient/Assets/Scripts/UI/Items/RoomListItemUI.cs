using Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class RoomListItemUIContext : UIContext
{
    public UIObject<TextMeshProUGUI> RoomNumberText = new();
    public UIObject<TextMeshProUGUI> RoomNameText = new();

    public UIObject NewMessagePanel = new();
    public UIObject<TextMeshProUGUI> NewMessageText = new();

    public UIObject SelectFrame = new();
}

public class RoomListItemUI : BaseUIItem
{
    [SerializeField]
    RoomListItemUIContext Context;

    public bool Selected { get; private set; } = false;

    public readonly Property<string, TextMeshProUGUI> RoomNumber = new();
    public readonly Property<string, TextMeshProUGUI> RoomName = new();

    int newMessageCount = 0;
    public readonly Property<string, TextMeshProUGUI> NewMessageCount = new();

    public override void Init()
    {
        base.Init();

        Context.SelectFrame.BindObject.SetActive(false);

        ResetNewMessageCount();
        RoomNumber.AddUI(Context.RoomNumberText);
        RoomName.AddUI(Context.RoomNameText);
        NewMessageCount.AddUI(Context.NewMessageText);
    }

    public void SetData(ulong roomNumber, string roomName, int newMessageCount = 0)
    {
        RoomNumber.Data = roomNumber.ToString();
        RoomName.Data = roomName;
        if (newMessageCount != 0) AddNewMessageCount(newMessageCount);
    }

    public void SetRoomNumber(ulong roomNumber)
    {
        RoomNumber.Data = roomNumber.ToString();
    }

    public void SetRoomName(string roomName)
    {
        RoomName.Data = roomName;
    }

    public void AddNewMessageCount(int value = 1)
    {
        if (newMessageCount == 0)
        {
            Context.NewMessagePanel.BindObject.SetActive(true);
        }
        newMessageCount += value;
        if (newMessageCount > AppConst.NewMessageVisibleMaxCount)
        {
            NewMessageCount.Data = "99+";
        }
        else
        {
            NewMessageCount.Data = newMessageCount.ToString();
        }
    }

    public void ResetNewMessageCount()
    {
        Context.NewMessagePanel.BindObject.SetActive(false);
        newMessageCount = 0;
    }

    void ShowSelected(bool visible = true)
    {
        Context.SelectFrame.BindObject.SetActive(visible);
    }

    public void OnUnSelected()
    {
        if (Selected == false) return;

        ShowSelected(false);
        Selected = false;
    }

    public void OnSelected()
    {
        if (Selected == true) return;

        ResetNewMessageCount();
        ShowSelected(true);

        Selected = true;
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
