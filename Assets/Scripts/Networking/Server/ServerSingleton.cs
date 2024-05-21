using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : MonoBehaviour
{
    private static ServerSingleton instance;

    public ServerGameManager GameManager { get; private set; }

    public static ServerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<ServerSingleton>();
                if (instance == null)
                {
                    Debug.LogError("No HostSingleton found in the scene!");
                }
            }

            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateServerAsync()
    {
        await UnityServices.InitializeAsync();
        GameManager = new ServerGameManager(ApplicationData.IP(), ApplicationData.Port(), ApplicationData.QPort(), NetworkManager.Singleton);
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
