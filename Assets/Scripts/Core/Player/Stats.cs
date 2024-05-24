using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public Dictionary<ulong, NetworkVariable<int>> deathsStats = new Dictionary<ulong, NetworkVariable<int>>();
    public Dictionary<ulong, NetworkVariable<int>> killsStats = new Dictionary<ulong, NetworkVariable<int>>();

    public void HandlePlayerDeath(ulong ownerId)
    {
        if (deathsStats.ContainsKey(ownerId))
        {
            deathsStats[ownerId].Value++;
        }
    }

    public void HandlePlayerKills(ulong killerId)
    {
        if (killsStats.ContainsKey(killerId))
        {
            killsStats[killerId].Value++;
        }
    }

    public void AddPlayerToLists(ulong ownerId)
    {
        if (!deathsStats.ContainsKey(ownerId))
        {
            deathsStats[ownerId] = new NetworkVariable<int>(0);
        }
        if (!killsStats.ContainsKey(ownerId))
        {
            killsStats[ownerId] = new NetworkVariable<int>(0);
        }
    }
}
