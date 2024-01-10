using Core;
using System;
using TMPro;
using UnityEngine;

[Serializable]
class ChatContentEtcItemUIContext : UIContext
{
    public UIObject<TextMeshProUGUI> Text = new();
}

public class ChatContentEtcItemUI : BaseUIItem
{
    [SerializeField]
    ChatContentEtcItemUIContext Context = new();

    public Property<string, TextMeshProUGUI> Text = new();
    public string time;
    bool entered;

    public override void Init()
    {
        base.Init();

        Text.AddUI(Context.Text);
    }

    public void SetText(string userName, string time, bool enter)
    {
        this.time = time;
        entered = enter;
        Text.Data = ToFormat(userName, time, enter);
    }

    public void SetUserName(string userName)
    {
        Text.Data = ToFormat(userName, time, entered);
    }

    string ToFormat(string userName, string time, bool enter)
    {
        if (enter) return $"[{time}] {userName} entered.";
        else return $"[{time}] {userName} leaved.";

    }

#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(ChatContentEtcItemUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
