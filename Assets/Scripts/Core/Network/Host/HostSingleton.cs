using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;

    [SerializeField] private HostGameManager gameManager;

    public static HostSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<HostSingleton>();
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

    public void CreateHost()
    {
        gameManager = new HostGameManager();
    }
}
