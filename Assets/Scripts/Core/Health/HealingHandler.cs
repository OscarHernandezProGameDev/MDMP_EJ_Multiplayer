using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingHandler : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Healing>(out var healing))
        {
            SpawningHealth spawningHealth = other.GetComponent<SpawningHealth>();
            Health health = GetComponent<Health>();

            if (health.currentHealth.Value < health.maxHealth)
            {
                healing.Collect(health);

                if (!IsServer)
                    return;

                other.GetComponent<NetworkObject>().Despawn(false);

                HealingRespawner.Instance.SetMedicKitToRespawn(spawningHealth);
            }
        }
    }
}
