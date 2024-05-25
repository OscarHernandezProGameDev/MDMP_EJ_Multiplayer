using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Matchmaker.Models;

public class ServerGameManager : IDisposable
{
    private const string gameServerName = "Game";

    private string serverIP;
    private int serverPort;
    private int serverQueryPort;
    public NetworkServer NetworkServer;
    private MultiplayAllocationService multiplayAllocationService;
    private MatchplayBackfiller backfiller;

    public ServerGameManager(string serverIP, int serverPort, int serverQueryPort, NetworkManager networkManager)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.serverQueryPort = serverQueryPort;
        NetworkServer = new NetworkServer(networkManager);
        multiplayAllocationService = new MultiplayAllocationService();
    }

    public async Task StartGameServerAsync()
    {
        Debug.Log("BeginServerCheck .....");

        await multiplayAllocationService.BeginServerCheck();

        try
        {
            Debug.Log("GetMatchmakerPayloadAsync .....");
            MatchmakingResults matchmakerPayload = await GetMatchmakerPayloadAsync();

            if (matchmakerPayload != null)
            {
                Debug.Log("StartBackFililAsync .....");
                await StartBackFililAsync(matchmakerPayload);

                NetworkServer.OnUserJoined += UserJoined;
                NetworkServer.OnUserLeft += UserLeft;
            }
            else
                Debug.LogWarning("Getting the matchmaker payload timed out");
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }

        Debug.Log($"Open Connection {serverIP}:{serverPort}");
        if (!NetworkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogWarning("NetworkServer did not start as expected");

            return;
        }

        Debug.Log("Loading scene .....");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameServerName, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            await Task.Delay(10);
        }
    }

    private async Task StartBackFililAsync(MatchmakingResults payload)
    {
        backfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}", payload.QueueName, payload.MatchProperties, 20);

        if (backfiller.NeedsPlayers())
            await backfiller.BeginBackfilling();
    }

    private void UserJoined(UserData user)
    {
        Debug.Log("User Joining .....");
        backfiller.AddPlayerToMatch(user);
        multiplayAllocationService.AddPlayer();

        if (!backfiller.NeedsPlayers() && backfiller.IsBackfilling)
        {
            Debug.Log("Stop BackFill .....");
            _ = backfiller.StopBackfill();
        }
    }

    private void UserLeft(UserData user)
    {
        int playerCount = backfiller.RemovePlayerFromMatch(user.userAuthId);
        //multiplayAllocationService.RemovePlayer();
        if (playerCount <= 0)
        {
            CloseServerAsync();

            return;
        }

        if (backfiller.NeedsPlayers() && !backfiller.IsBackfilling)
        {
            Debug.Log("Begin BackFill .....");
            _ = backfiller.BeginBackfilling();
        }
    }

    private async void CloseServerAsync()
    {
        await backfiller.StopBackfill();
        Dispose();
        Application.Quit();
    }

    private async Task<MatchmakingResults> GetMatchmakerPayloadAsync()
    {
        Task<MatchmakingResults> matchmakerPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
            return matchmakerPayloadTask.Result;

        return null;
    }

    public void Dispose()
    {
        if (NetworkServer != null)
        {
            NetworkServer.OnUserJoined -= UserJoined;
            NetworkServer.OnUserLeft -= UserLeft;
        }
        backfiller?.Dispose();
        multiplayAllocationService?.Dispose();
        NetworkServer?.Dispose();
    }
}
