using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    public static DestroySelfOnContact instance;

    public NetworkObject networkObject;
    public GameObject prefab;

    public int teamIndex;
    [SerializeField] private bool destroyOnFriendlyFire;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SetPlayerData>(out SetPlayerData player))
        {
            if (player.TeamIndex.Value >= 0 && player.TeamIndex.Value == teamIndex && !destroyOnFriendlyFire)
            {
                return;
            }
            else
            {
                NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);
                if (networkObject.IsSpawned)
                {
                    networkObject.Despawn(false);
                }
            }
        }
        else
        {
            NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);
            if (networkObject.IsSpawned)
            {
                networkObject.Despawn(false);
            }
        }
    }
}
