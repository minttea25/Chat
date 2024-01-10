using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotiItem : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI text;

    const float DefaultFadeOutDuration = 1.5f;

    public void Init(string text, float timeAfter)
    {
        this.text.text = text;
        Hide(timeAfter);
    }

    void Hide(float timeAfter)
    {
        Invoke(nameof(FadeOut), timeAfter);
    }

    void FadeOut()
    {
        var t = canvasGroup.DOFade(0, DefaultFadeOutDuration);
        t.onComplete += () => Destroy(gameObject, DefaultFadeOutDuration);
    }
}
