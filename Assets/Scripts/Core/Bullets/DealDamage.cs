using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private bool isFriendlyFireActive;

    private ulong ownerClientId;
    private Stats playerStats;
    private int teamIndex;

    private void OnEnable()
    {
        playerStats = FindObjectOfType<Stats>();
    }

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }
    public void SetTeamIndex(int teamIndex)
    {
        this.teamIndex = teamIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
		}

        if (other.TryGetComponent<SetPlayerData>(out SetPlayerData player))
        {
            if (player.TeamIndex.Value < 0 || player.TeamIndex.Value != teamIndex || isFriendlyFireActive)
            {
                other.TryGetComponent<Health>(out Health health);
                health.TakeDamage(damage);
                if (health.currentHealth.Value == 0)
                {
                    playerStats.HandlePlayerKills(ownerClientId);
                }
            }
        }
    }
}
