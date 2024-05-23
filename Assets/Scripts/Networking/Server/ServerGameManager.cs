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
    private NetworkServer networkServer;
    private MultiplayAllocationService multiplayAllocationService;
    private MatchplayBackfiller backfiller;

    public ServerGameManager(string serverIP, int serverPort, int serverQueryPort, NetworkManager networkManager)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.serverQueryPort = serverQueryPort;
        this.networkServer = new NetworkServer(networkManager);
    }

    public async Task StartGameServerAsync()
    {
        await multiplayAllocationService.BeginServerCheck();

        try
        {
            MatchmakingResults matchmakerPayload = await GetMatchmakerPayloadAsync();

            if (matchmakerPayload != null)
            {
                await StartBackFililAsync(matchmakerPayload);
            }
            else
                Debug.LogWarning("Getting the matchmaker payload timed out");
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }

        if (!networkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogWarning("NetworkServer did not start as expected");

            return;
        }

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

    private async Task<MatchmakingResults> GetMatchmakerPayloadAsync()
    {
        Task<MatchmakingResults> matchmakerPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
            return matchmakerPayloadTask.Result;

        return null;
    }

    public void Dispose()
    {
    }
}
