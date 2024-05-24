using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Unity.Networking.Transport.Utilities.ReliableUtility;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private float respawningTime = 2f;
    [SerializeField] private Stats Statistics;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        SetPlayerData[] players = FindObjectsByType<SetPlayerData>(FindObjectsSortMode.None);
        foreach (SetPlayerData player in players)
        {
            HandlePlayerSpawned(player);
        }

        SetPlayerData.OnPlayerSpawned += HandlePlayerSpawned;
        SetPlayerData.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        SetPlayerData.OnPlayerSpawned -= HandlePlayerSpawned;
        SetPlayerData.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(SetPlayerData player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(SetPlayerData player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(SetPlayerData player)
    {
        Statistics.HandlePlayerDeath(player.OwnerClientId);

        player.gameObject.SetActive(false);

        StartCoroutine(RespawnPlayer(player));
    }

    private IEnumerator RespawnPlayer(SetPlayerData player)
    {
        NetworkObject playerInstance = player.GetComponent<NetworkObject>();

        ulong previousOwnerId = player.OwnerClientId;

        playerInstance.GetComponent<ClientNetworkTransform>().Interpolate = false;

        playerInstance.RemoveOwnership();

        playerInstance.transform.position = SpawnPoint.GetRandomSpawnPosition();

        yield return new WaitForSeconds(respawningTime);

        player.gameObject.SetActive(true);

        playerInstance.GetComponent<ClientNetworkTransform>().Interpolate = true;

        StartCoroutine(ChangeOwnershipBack(playerInstance, previousOwnerId));
    }

    private IEnumerator ChangeOwnershipBack(NetworkObject playerInstance, ulong previousOwnerId)
    {
        ResetHealth(playerInstance);

        yield return new WaitForSeconds(.5f);

        playerInstance.ChangeOwnership(previousOwnerId);
    }

    private void ResetHealth(NetworkObject playerInstance)
    {
        Health health = playerInstance.GetComponent<Health>();
        health.currentHealth.Value = health.maxHealth;
        health.isDead = false;
    }
}
