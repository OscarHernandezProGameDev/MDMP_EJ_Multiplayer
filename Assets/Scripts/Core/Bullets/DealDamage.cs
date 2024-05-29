using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private bool isFriendlyFireActive;

    private ulong ownerClientId;
    private Stats stats;
    private int teamIndex;

    private void OnEnable()
    {
        stats = FindAnyObjectByType<Stats>();
    }

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    public void SetTeamIndex(int teamIndex)
    {
        this.teamIndex = teamIndex;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkObject>(out var netVar))
        {
            if (netVar.OwnerClientId == ownerClientId)
            {
                return;
            }
        }

        if (other.TryGetComponent(out SetPlayerData playerData))
        {
            var playerTeamIndex = playerData.TeamIndex.Value;

            if (playerTeamIndex < 0 || playerTeamIndex != teamIndex || isFriendlyFireActive)
            {
                other.TryGetComponent<Health>(out var health);
                health.TakeDamage(damage);
                if (health.currentHealth.Value == 0)
                {
                    stats.HandlerPlayerKills(ownerClientId);
                }
            }
        }
    }
}
