using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text findMatchmakingText;
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text queueStatusTimerText;
    [SerializeField] private Toggle teamToogle;
    [SerializeField] private Toggle privateToogle;

    private bool isMatchmaking;
    private bool isCancelling;
    private bool isBusy;
    private float timeInQueue;

    private void Start()
    {
        queueStatusText.text = string.Empty;
        queueStatusTimerText.text = string.Empty;
    }

    private void Update()
    {
        if (isMatchmaking)
        {
            timeInQueue += Time.deltaTime;

            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInQueue);

            queueStatusTimerText.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
    }

    public async void FindMatchButtonPressed()
    {
        if (isCancelling)
            return;

        if (isMatchmaking)
        {
            queueStatusText.text = "Cancelling...";
            isCancelling = true;
            await ClientSingleton.Instance.GameManager.CancelMatchmaking();
            isCancelling = false;
            isMatchmaking = false;
            isBusy = false;
            findMatchmakingText.text = "Find Match";
            queueStatusText.text = string.Empty;
            queueStatusTimerText.text = string.Empty;

            return;
        }

        if (isBusy)
            return;

        ClientSingleton.Instance.GameManager.MatchmakeAsync(teamToogle.isOn, OnMatchmakeResponse);
        findMatchmakingText.text = "cancel";
        queueStatusText.text = "Searching...";
        timeInQueue = 0;
        isMatchmaking = true;
        isBusy = true;
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
        if (isBusy)
            return;

        isBusy = true;

        await HostSingleton.Instance.GameManager.StartHostAsync(privateToogle.isOn);

        isBusy = false;
    }

    public async void StartClient()
    {
        if (isBusy)
            return;

        isBusy = true;

        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);

        isBusy = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isBusy)
            return;

        isBusy = true;
        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);

            return;
        }
        finally
        {
            isBusy = false;
        }
    }
}