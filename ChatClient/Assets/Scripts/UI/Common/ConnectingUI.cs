using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI ConnectingText;


    static ConnectingUI instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        gameObject.SetActive(false);
    }

    public static void Show()
    {
        instance.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        instance.gameObject.SetActive(false);
    }

}
