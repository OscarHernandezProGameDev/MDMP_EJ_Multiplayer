using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private const int MaxConnections = 10;
    private const string gameServerName = "Game";

    private Allocation allocation;
    private string JoinCode;
    private string lobbyId;

    public async Task<bool> StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
            return false;
        }

        try
        {
            JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join Code: {JoinCode}");
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);

            return false;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    ["JoinCode"] = new DataObject(visibility: DataObject.VisibilityOptions.Member, value: JoinCode)
                }
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("Sample Lobby name", MaxConnections, lobbyOptions);

            lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartbeartLobby(15));
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogWarning(ex);

            return false;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameServerName, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            await Task.Delay(10);
        }

        NetworkManager.Singleton.StartHost();

        return true;
    }

    private IEnumerator HeartbeartLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);

            yield return delay;
        }
    }
}
