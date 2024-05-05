using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Charger : NetworkBehaviour
{
    public NetworkVariable<int> TotalAmmo = new NetworkVariable<int>();

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Ammo>(out var ammo))
        {
            int ammoValue = ammo.Collect();

            if (!IsServer)
                return;

            TotalAmmo.Value += ammoValue;
        }
    }
}
