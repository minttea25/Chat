using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WebUrls", menuName = "Network/WebUrls")]
public class WebUrls : ScriptableObject
{
    [SerializeField]
    string m_accountWebServerBaseUrl;

    [SerializeField]
    string m_loginUrl;
    [SerializeField]
    string m_registerUrl;

    public string AccountWebServerBaseUrl => m_accountWebServerBaseUrl;
    public string LoginUrl => m_loginUrl;
    public string RegisterUrl => m_registerUrl;

    public override string ToString()
    {
        return $"BaseUrl: {AccountWebServerBaseUrl}, Login: {LoginUrl}, Register: {RegisterUrl}";
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(AccountWebServerBaseUrl))
        {
            Debug.LogWarning($"{nameof(AccountWebServerBaseUrl)} is not valid");
        }
    }
#endif
}
