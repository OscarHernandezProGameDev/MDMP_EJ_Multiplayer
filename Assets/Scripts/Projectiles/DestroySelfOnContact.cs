using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    public static DestroySelfOnContact instance;

    public NetworkObject NetworkObject;
    public GameObject Prefab;

    private void Awake()
    {
        instance = this;
    }

    void OnTriggerEnter(Collider other)
    {
        //Destroy(gameObject);
        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, Prefab);
    }
}
