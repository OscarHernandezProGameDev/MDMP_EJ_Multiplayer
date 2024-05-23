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

    public async void FindMatchButtonPressed()
    {
        if (isMatchmaking)
            return;

        if (isMatchmaking)
        {
            queueStatusText.text = "Searching...";
            isCancelling = true;
            await ClientSingleton.Instance.GameManager.CancelMatchmakingAsync();
            isCancelling = false;
            isMatchmaking = false;
            findMatchmakingText.text = "Find Match";
            queueStatusText.text = string.Empty;

            return;
        }

        await ClientSingleton.Instance.GameManager.MatchmakeAsync(OnMatchmakeResponse);
        findMatchmakingText.text = "cancel";
        queueStatusText.text = "Searching...";
        isMatchmaking = true;
    }

    private void OnMatchmakeResponse(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusText.text = "Connecting ...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusText.text = "There was an error creating your ticket";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatusText.text = "There was an error cancelling your ticket";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusText.text = "There was an error retrieving your ticket";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusText.text = "There was an error assigning to match";
                break;
        }
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