using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntity leaderboardEntityPrefab;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
        }
        if (IsServer)
        {
            SetPlayerData[] players = FindObjectsByType<SetPlayerData>(FindObjectsSortMode.None);

            foreach (SetPlayerData player in players)
                HandlePlayerSpawned(player);

            SetPlayerData.OnPlayerSpawned += HandlePlayerSpawned;
            SetPlayerData.OnPlayerDespawned += HandlePlayerDespawned;

        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
        }
        if (IsServer)
        {
            SetPlayerData.OnPlayerSpawned -= HandlePlayerSpawned;
            SetPlayerData.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        throw new NotImplementedException();
    }

    public void HandlePlayerSpawned(SetPlayerData player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.playerName.Value,
            Deaths = 0,
            Kills = 0,
        });
    }

    public void HandlePlayerDespawned(SetPlayerData player)
    {
        if (leaderboardEntities == null)
            return;

        foreach (var entity in leaderboardEntities)
        {
            if (entity.ClientId == player.OwnerClientId)
            {
                leaderboardEntities.Remove(entity);

                break;
            }
        }
    }

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }
}
