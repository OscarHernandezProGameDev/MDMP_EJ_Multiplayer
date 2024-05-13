using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    public static DestroySelfOnContact instance;

#if !NO_POOLING
    public NetworkObject NetworkObject;
    public GameObject Prefab;
#endif

    private void Awake()
    {
        instance = this;
    }

    void OnTriggerEnter(Collider other)
    {
#if NO_POOLING
        Destroy(gameObject);
#else
        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, Prefab);

        // Nos aseguramos que se instance en los cientes
        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(false);
        }
#endif
    }
}
