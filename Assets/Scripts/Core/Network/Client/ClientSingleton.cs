using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    public ClientGameManager GameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<ClientSingleton>();
                if (instance == null)
                {
                    Debug.LogError("No ClientSingleton found in the scene!");
                }
            }

            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClientAsync()
    {
        GameManager = new ClientGameManager();

        return await GameManager.InitAsync();
    }
}
