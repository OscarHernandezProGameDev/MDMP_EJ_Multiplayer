using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;

    private const string MainMenu = "MainMenu";

    public NetworkClient(NetworkManager networkManager)
    {
        this._networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId != 0 && clientId != _networkManager.LocalClientId) { return; }

        Disconnect();
    }

    internal void Disconnect()
    {
        if (SceneManager.GetActiveScene().name != MainMenu)
        {
            SceneManager.LoadScene(MainMenu);
        }

        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if (_networkManager == null) { return; }

        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}
