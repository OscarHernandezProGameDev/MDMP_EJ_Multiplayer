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

    private bool isMatchmaking;
    private bool isCancelling;

    private void Start()
    {
        queueStatusText.text = string.Empty;
        queueStatusTimerText.text = string.Empty;
    }

    public void FindMatchButtonPressed()
    {
        if (isMatchmaking)
            return;

        if (isMatchmaking)
        {
            queueStatusText.text = "Searching...";
            isCancelling = true;
            // cancel matchmaking
            isCancelling = false;
            isMatchmaking = false;
            findMatchmakingText.text = "Find Match";
            queueStatusText.text = string.Empty;

            return;
        }
        findMatchmakingText.text = "cancel";
        queueStatusText.text = "Searching...";
        isMatchmaking = true;
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