using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private Allocation allocation;
    private NetworkObject playerPrefab;

    public string JoinCode;
    private string lobbyId;

    public NetworkServer NetworkServer { get; private set; }

    private const int MaxConnections = 10;
    private const string GameScene = "Game";

    public HostGameManager(NetworkObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
    }

    public async Task StartHostAsync(bool isPrivate)
    {
		try
		{
	        allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex);
			return;
		}

        try
        {
            JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join code: { JoinCode }");
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOption = new CreateLobbyOptions();
            lobbyOption.IsPrivate = isPrivate;
            lobbyOption.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: JoinCode
                    )
                }
            };

            string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unkown");

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MaxConnections, lobbyOption);

            lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartbeartLobby(15));
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            return;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GameScene, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            await Task.Delay(10);
        }

        NetworkServer = new NetworkServer(NetworkManager.Singleton, playerPrefab);

        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadByte = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadByte;

        NetworkManager.Singleton.StartHost();

        NetworkServer.OnClientLeft += HandleClientLeft;
    }

    private IEnumerator HeartbeartLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while(true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    public async void Shutdown()
    {
        if (string.IsNullOrEmpty(lobbyId)) { return; }

        HostSingleton.Instance.StopCoroutine(nameof(HeartbeartLobby));

        try
        {
            await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }

        lobbyId = string.Empty;

        NetworkServer.OnClientLeft -= HandleClientLeft;

        NetworkServer?.Dispose();
    }

    private async void HandleClientLeft(string authId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, authId);
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
        }
    }
}
