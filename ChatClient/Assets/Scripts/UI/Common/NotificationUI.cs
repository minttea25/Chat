using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] GameObject NotiPrefab;
    [SerializeField] Transform NotiPanel;

    static NotificationUI instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach (Transform item in NotiPanel)
        {
            Destroy(item.gameObject);
        }
    }

    public static void Show(string text, float timeAfter = 1.5f)
    {
        var noti = Instantiate(instance.NotiPrefab, instance.NotiPanel);
        noti.GetComponent<NotiItem>().Init(text, timeAfter);
    }
}
