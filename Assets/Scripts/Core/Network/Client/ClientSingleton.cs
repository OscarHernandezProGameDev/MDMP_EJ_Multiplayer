using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

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
}
