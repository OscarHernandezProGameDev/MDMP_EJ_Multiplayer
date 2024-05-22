using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text findMatchmakingText;
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text queueStatusTimerText;

    private void Start()
    {
        queueStatusText.text = string.Empty;
        queueStatusTimerText.text = string.Empty;
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}