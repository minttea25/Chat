using DG.Tweening;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup CanvasGroup;

    [SerializeField]
    TextMeshProUGUI Content;

    static NotificationUI instance;

    const float FadeOutDuration = 1.5f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        instance.CanvasGroup.alpha = 0f;
    }

    public static void Show(string text, float timeAFter = 1.5f)
    {
        instance.Content.text = text;
        instance.CanvasGroup.alpha = 1f;

        instance.Hide(timeAFter);
    }

    void Hide(float timeAfter)
    {
        Invoke(nameof(FadeOut), timeAfter);
    }

    void FadeOut()
    {
        CanvasGroup.DOFade(0, FadeOutDuration);
    }
}
