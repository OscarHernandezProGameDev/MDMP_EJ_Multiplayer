using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ServerGameManager : IDisposable
{
    private const string gameServerName = "Game";

    private string serverIP;
    private int serverPort;
    private int serverQueryPort;
    private NetworkServer networkServer;
    private MultiplayAllocationService multiplayAllocationService;

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

    public void Dispose()
    {
    }
}
