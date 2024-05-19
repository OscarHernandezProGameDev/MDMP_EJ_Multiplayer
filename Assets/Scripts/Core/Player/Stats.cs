using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public Dictionary<ulong, NetworkVariable<int>> DeathsStats { get; } = new Dictionary<ulong, NetworkVariable<int>>();
    public Dictionary<ulong, NetworkVariable<int>> KillsStats { get; } = new Dictionary<ulong, NetworkVariable<int>>();

    public void HandlerPlayerDeath(ulong ownerId)
    {
        if (DeathsStats.ContainsKey(ownerId))
        {
            DeathsStats[ownerId].Value++;
        }
    }

    public void HandlerPlayerKills(ulong killId)
    {
        if (KillsStats.ContainsKey(killId))
        {
            KillsStats[killId].Value++;
        }
    }

    public void AddPlayerToLists(ulong ownerId)
    {
        if (!DeathsStats.ContainsKey(ownerId))
        {
            DeathsStats[ownerId] = new NetworkVariable<int>(0);
        }
        if (!KillsStats.ContainsKey(ownerId))
        {
            KillsStats[ownerId] = new NetworkVariable<int>(0);
        }
    }
}
