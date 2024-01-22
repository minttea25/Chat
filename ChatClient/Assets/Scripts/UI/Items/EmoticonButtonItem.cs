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

        // 나중에 데이터가 추가되면 비동기 로딩으로? like 카카오톡 
        // web으로 부터 데이터를 받아서 데이터 추가 (json 및 메모리(data manager)
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
