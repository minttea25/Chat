using Core;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class EmoticonPanelUIContext : UIContext
{
    public UIObject EmoticonList = new();
    public UIObject<Button> EmoticonPanelCloseButton = new();
}

public class EmoticonPanel : BaseUI
{
    [SerializeField]
    EmoticonPanelUIContext Context = new();

    Transform EmoticonButtons => Context.EmoticonList.BindObject.transform;

    public override void Init()
    {
        base.Init();

        EmoticonButtons.DestroyAllItems();
        Context.EmoticonPanelCloseButton.Component.onClick.AddListener(() => gameObject.SetActive(false));

        LoadItems();
    }

    public void LoadItems()
    {
        var emoticons = ManagerCore.Data.Emoticons.Emoticons;
        foreach(var kv in emoticons)
        {
            var emoticonButton = ManagerCore.UI.AddItemUI<EmoticonButtonItem>(AddrKeys.EmoticonButtonItemUI, EmoticonButtons);
            emoticonButton.SetIcon(kv.Key, kv.Value);
            emoticonButton.BindEventUnityAction(() =>
            {
                var chatPanel = ManagerCore.Scene.GetScene<MainScene>().UI.SelectedChatPanel;
                if (chatPanel != null )
                {
                    chatPanel.SendEmoticon(emoticonButton.IconId);
                }
            });
        }
    }



#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(EmoticonPanelUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
