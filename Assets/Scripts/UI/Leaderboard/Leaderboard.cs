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
    [SerializeField] private Transform titleEntity;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;
    private List<LeaderboardEntity> entities = new List<LeaderboardEntity>();

    [field: SerializeField] public Stats Statistics { get; private set; }

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

        //if (IsOwner)
        //    gameObject.SetActive(false);
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

        // ordenando de mayor a menor por kills
        entities.Sort((x, y) => y.Kills.CompareTo(x.Kills));

        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].transform.SetSiblingIndex(i);
            entities[i].UpdateText();
        }
        titleEntity.transform.SetSiblingIndex(0);
    }

    public void HandlePlayerSpawned(SetPlayerData player)
    {
        ulong ownerId = player.OwnerClientId;

        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = ownerId,
            PlayerName = player.playerName.Value,
            Deaths = 0,
            Kills = 0,
        });

        Statistics.AddPlayerToLists(ownerId);

        Statistics.DeathsStats[ownerId].OnValueChanged += (oldDeaths, newDeaths) => HandleDeathsChanged(ownerId, newDeaths);
        Statistics.KillsStats[ownerId].OnValueChanged += (oldKills, newKills) => HandleKillsChanged(ownerId, newKills);
    }

    private void HandleDeathsChanged(ulong ownerClientId, int newDeaths)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            var entity = leaderboardEntities[i];

            if (entity.ClientId == ownerClientId)
            {
                leaderboardEntities[i] = new LeaderboardEntityState
                {
                    ClientId = entity.ClientId,
                    PlayerName = entity.PlayerName.Value,
                    Deaths = newDeaths,
                    Kills = entity.Kills,
                };

                return;
            }
        }
    }

    private void HandleKillsChanged(ulong ownerClientId, int newKills)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            var entity = leaderboardEntities[i];

            if (entity.ClientId == ownerClientId)
            {
                leaderboardEntities[i] = new LeaderboardEntityState
                {
                    ClientId = entity.ClientId,
                    PlayerName = entity.PlayerName.Value,
                    Deaths = entity.Deaths,
                    Kills = newKills,
                };

                return;
            }
        }
    }

    public void HandlePlayerDespawned(SetPlayerData player)
    {
        if (leaderboardEntities == null)
            return;

        if (IsServer && player.OwnerClientId == OwnerClientId)
            return;

        foreach (var entity in leaderboardEntities)
        {
            if (entity.ClientId == player.OwnerClientId)
            {
                leaderboardEntities.Remove(entity);

                break;
            }
        }

        Statistics.DeathsStats[player.OwnerClientId].OnValueChanged -= (oldDeaths, newDeaths) => HandleDeathsChanged(player.OwnerClientId, newDeaths);
        Statistics.KillsStats[player.OwnerClientId].OnValueChanged -= (oldKills, newKills) => HandleKillsChanged(player.OwnerClientId, newKills);
    }

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }
}
