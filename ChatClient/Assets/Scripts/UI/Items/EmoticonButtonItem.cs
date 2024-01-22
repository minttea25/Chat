using Core;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class EmoticonButtonItemContext : UIContext
{
    public UIObject<Image> LoadingImage = new();
}

public class EmoticonButtonItem : BaseUIItem
{
    [SerializeField]
    EmoticonButtonItemContext Context = new();

    public uint IconId { get; private set; }
    Tweener spriteLoading = null;

    public override void Init()
    {
        base.Init();

        spriteLoading = Context.LoadingImage.Component.rectTransform
            .DORotate(new Vector3(0f, 0f, -360f), 0, RotateMode.FastBeyond360);
    }

    public void SetIcon(uint id, string key)
    {
        IconId = id;
        //var emoticon = ManagerCore.Data.Emoticons?.GetEmoticon(iconId);
        ManagerCore.Resource.LoadImageAsync(key, (sprite) =>
        {
            spriteLoading.Pause();
            Destroy(Context.LoadingImage.BindObject);
            GetComponent<Image>().sprite = sprite;
            spriteLoading = null;
        });

        // ���߿� �����Ͱ� �߰��Ǹ� �񵿱� �ε�����? like īī���� 
        // web���� ���� �����͸� �޾Ƽ� ������ �߰� (json �� �޸�(data manager)
    }

#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(EmoticonButtonItemContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
