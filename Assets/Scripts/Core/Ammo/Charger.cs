using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Charger : NetworkBehaviour
{
    public NetworkVariable<int> TotalAmmo = new NetworkVariable<int>();

    public void SpendAmmo()
    {
        TotalAmmo.Value--;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ammo>(out var ammo))
        {
            int ammoValue = ammo.Collect();

            if (!IsServer)
                return;

            other.gameObject.SetActive(false);

            TotalAmmo.Value += ammoValue;
        }
    }
}
