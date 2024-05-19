using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Healing : NetworkBehaviour
{
    protected bool isCollected;

    public void Spawn()
    {
        if (IsServer && !IsSpawned)
        {
            gameObject.SetActive(true);
            NetworkObject.Spawn();
        }
    }

    public override void OnNetworkDespawn()
    {
        gameObject.SetActive(false);
        base.OnNetworkDespawn();
    }

    public abstract void Collect(Health health);
    public abstract void ResetStatus();
}
