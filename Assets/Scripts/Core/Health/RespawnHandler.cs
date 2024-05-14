using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private float respawnTime = 2f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        SetPlayerData[] players = FindObjectsOfType<SetPlayerData>();

        foreach (SetPlayerData player in players)
            HandlePlayerSpawned(player);

        SetPlayerData.OnPlayerSpawned += HandlePlayerSpawned;
        SetPlayerData.OnPlayerDespawned += HandlePlayerDespawned;
    }

    override public void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        SetPlayerData.OnPlayerSpawned -= HandlePlayerSpawned;
        SetPlayerData.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(SetPlayerData player)
    {
        player.Health.OnDie += _ => HandlePlayerDied(player);
    }

    private void HandlePlayerDespawned(SetPlayerData player)
    {
        player.Health.OnDie -= _ => HandlePlayerDied(player);
    }

    private void HandlePlayerDied(SetPlayerData player)
    {
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        yield return new WaitForSeconds(respawnTime);

        NetworkObject playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPosition(), Quaternion.identity);

        playerInstance.SpawnAsPlayerObject(ownerClientId);
    }
}
