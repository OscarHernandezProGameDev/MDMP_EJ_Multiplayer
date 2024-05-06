using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    [SerializeField] private ClientGameManager gameManager;

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

    public async Task CreateClientAsync()
    {
        gameManager = new ClientGameManager();

        await gameManager.InitAsync();
    }
}
