using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Charger : NetworkBehaviour
{
    public NetworkVariable<int> TotalAmmo = new NetworkVariable<int>();

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.TryGetComponent<Ammo>(out Ammo ammo)) { return;  }

        int ammoValue = ammo.Collect();

        if (!IsServer) { return; }

        collider.gameObject.SetActive(false);

        TotalAmmo.Value += ammoValue;
    }

    public void SpendAmmo()
    {
        TotalAmmo.Value -= 1;
    }
}
