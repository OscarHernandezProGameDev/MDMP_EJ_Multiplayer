using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboadEntityHolder;
    [SerializeField] private LeaderboardEntity leaderboadEntityPrefab;
    [SerializeField] private Transform titleEntity;

    private NetworkList<leaderboardEntityState> leaderboardEntities;
    private List<LeaderboardEntity> entities = new List<LeaderboardEntity>();

    [field: SerializeField] public Stats Statistics { get; private set; }

    private void Awake()
    {
        leaderboardEntities = new NetworkList<leaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboadEntitiesChanged;
            foreach (leaderboardEntityState entity in leaderboardEntities)
            {
                HandleLeaderboadEntitiesChanged(new NetworkListEvent<leaderboardEntityState>
                {
                    Type = NetworkListEvent<leaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            SetPlayerData[] players = FindObjectsByType<SetPlayerData>(FindObjectsSortMode.None);
            foreach (SetPlayerData player in players)
            {
                HandlePlayerSpawned(player);
            }

            SetPlayerData.OnPlayerSpawned += HandlePlayerSpawned;
            SetPlayerData.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboadEntitiesChanged;
        }

        if (IsServer)
        {
            SetPlayerData.OnPlayerSpawned -= HandlePlayerSpawned;
            SetPlayerData.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboadEntitiesChanged(NetworkListEvent<leaderboardEntityState> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<leaderboardEntityState>.EventType.Add:
                if (!entities.Any(x => x.ClientId == changeEvent.Value.ClientID))
                {
                    LeaderboardEntity leaderboardEntity = Instantiate(leaderboadEntityPrefab, leaderboadEntityHolder);
                    leaderboardEntity.Initialise(changeEvent.Value.ClientID,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Deaths,
                        changeEvent.Value.Kills);
                    entities.Add(leaderboardEntity);
                }
                break;
            case NetworkListEvent<leaderboardEntityState>.EventType.Remove:
                LeaderboardEntity entityToRemove = entities.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientID);
                if (entityToRemove != null)
                {
                    entityToRemove.transform.SetParent(null);
                    Destroy(entityToRemove.gameObject);
                    entities.Remove(entityToRemove);
                }
                break;
            case NetworkListEvent<leaderboardEntityState>.EventType.Value:
                LeaderboardEntity entityToUpdate = entities.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientID);
                if (entityToUpdate != null)
                {
                    entityToUpdate.UpdateDeaths(changeEvent.Value.Deaths);
                    entityToUpdate.UpdateKills(changeEvent.Value.Kills);
                }
                break;
        }

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
        leaderboardEntities.Add(new leaderboardEntityState
        {
            ClientID = player.OwnerClientId,
            PlayerName = player.playerName.Value,
            Deaths = 0,
            Kills = 0
        });

        ulong ownerId = player.OwnerClientId;

        Statistics.AddPlayerToLists(ownerId);

        Statistics.deathsStats[ownerId].OnValueChanged += (oldDeaths, newDeaths) => HandleDeathsChanged(ownerId, newDeaths);

        Statistics.killsStats[ownerId].OnValueChanged += (oldKills, newKills) => HandleKillsChanged(ownerId, newKills);
    }

    private void HandleDeathsChanged(ulong ownerId, int newDeaths)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientID !=  ownerId) { continue; }

            leaderboardEntities[i] = new leaderboardEntityState
            {
                ClientID = leaderboardEntities[i].ClientID,
                PlayerName = leaderboardEntities[i].PlayerName.Value,
                Deaths = newDeaths,
                Kills = leaderboardEntities[i].Kills
            };

            return;
        }
    }

    private void HandleKillsChanged(ulong ownerId, int newKills)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientID != ownerId) { continue; }

            leaderboardEntities[i] = new leaderboardEntityState
            {
                ClientID = leaderboardEntities[i].ClientID,
                PlayerName = leaderboardEntities[i].PlayerName.Value,
                Deaths = leaderboardEntities[i].Deaths,
                Kills = newKills,
            };

            return;
        }
    }

    public void HandlePlayerDespawned(SetPlayerData player)
    {
        if (leaderboardEntities == null) { return; }

        if (IsServer && player.OwnerClientId == OwnerClientId) { return; }

        foreach (leaderboardEntityState entity in leaderboardEntities)
        {
            if (entity.ClientID != player.OwnerClientId) { continue; }

            leaderboardEntities.Remove(entity);
            break;
        }

        Statistics.deathsStats[player.OwnerClientId].OnValueChanged -= (oldDeaths, newDeaths) => HandleDeathsChanged(player.OwnerClientId, newDeaths);

        Statistics.killsStats[player.OwnerClientId].OnValueChanged -= (oldKills, newKills) => HandleKillsChanged(player.OwnerClientId, newKills);
    }
}
