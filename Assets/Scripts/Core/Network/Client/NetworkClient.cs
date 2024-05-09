using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient
{
    private readonly NetworkManager _networkManager;

    private const string MainMenu = "MainMenu";

    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // ClientId 0 => Host

        if (clientId != 0 && clientId != _networkManager.LocalClientId)
            return;

        if (SceneManager.GetActiveScene().name != MainMenu)
            SceneManager.LoadScene(MainMenu);

        if (_networkManager.IsConnectedClient)
            _networkManager.Shutdown();
    }
}
