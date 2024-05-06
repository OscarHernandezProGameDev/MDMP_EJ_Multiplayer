using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
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
        RelayServerData relayServerData = new RelayServerData(allocation, "udp");

        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(gameServerName, LoadSceneMode.Single);

        return true;
    }
}
