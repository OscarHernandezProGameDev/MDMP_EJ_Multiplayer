using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntity leaderboardEntityPrefab;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;
    private List<LeaderboardEntity> entities = new List<LeaderboardEntity>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
            foreach (var entity in leaderboardEntities)
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState> { Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add, Value = entity });
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
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                if (!entities.Any(e => e.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderboardEntity leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);

                    leaderboardEntity.Initialise(changeEvent.Value.ClientId, changeEvent.Value.PlayerName, changeEvent.Value.Kills, changeEvent.Value.Deaths);

                    entities.Add(leaderboardEntity);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntity entityToRemove = entities.FirstOrDefault(e => e.ClientId == changeEvent.Value.ClientId);

                if (entityToRemove != null)
                {
                    entityToRemove.transform.SetParent(null);
                    Destroy(entityToRemove.gameObject);
                    entities.Remove(entityToRemove);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntity entityToUpdate = entities.FirstOrDefault(e => e.ClientId == changeEvent.Value.ClientId);

                if (entityToUpdate != null)
                {
                    entityToUpdate.UpdateDeaths(changeEvent.Value.Deaths);
                    entityToUpdate.UpdateKills(changeEvent.Value.Kills);
                }
                break;
        }
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
