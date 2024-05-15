using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Stats : MonoBehaviour
{
    private Dictionary<ulong, NetworkVariable<int>> deathsStats = new Dictionary<ulong, NetworkVariable<int>>();
    private Dictionary<ulong, NetworkVariable<int>> killsStats = new Dictionary<ulong, NetworkVariable<int>>();

    public void HandlerPlayerDeath(ulong ownerId)
    {
        if (deathsStats.ContainsKey(ownerId))
        {
            deathsStats[ownerId].Value++;
            Debug.Log($"Owned ID {ownerId}");
            Debug.Log($"Deaths {deathsStats[ownerId].Value}");
        }
        else
        {
            deathsStats.Add(ownerId, new NetworkVariable<int>(1));
            Debug.Log($"First Enthy Owned ID {ownerId}");
            Debug.Log($"First Entry Deaths {deathsStats[ownerId].Value}");
        }
    }

    public void HandlerPlayerKills(ulong killId)
    {
        if (killsStats.ContainsKey(killId))
        {
            killsStats[killId].Value++;
            Debug.Log($"Owned ID {killId}");
            Debug.Log($"Kills {killsStats[killId].Value}");
        }
        else
        {
            killsStats.Add(killId, new NetworkVariable<int>(1));
            Debug.Log($"First Entry Killer ID {killId}");
            Debug.Log($"First Entry Kills {killsStats[killId].Value}");
        }
    }
}
