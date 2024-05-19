using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private float respawnTime = 2f;
    [SerializeField] private Stats Statistics;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        SetPlayerData[] players = FindObjectsByType<SetPlayerData>(FindObjectsSortMode.None);

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
        Statistics.HandlerPlayerDeath(player.OwnerClientId);
        player.gameObject.SetActive(false);

        StartCoroutine(RespawnPlayer(player));
    }

    IEnumerator RespawnPlayer(SetPlayerData player)
    {
        NetworkObject playerInstance = player.gameObject.GetComponent<NetworkObject>();
        ulong previousOwnerClientId = playerInstance.OwnerClientId;

        playerInstance.GetComponent<ClientNetwordTransform>().Interpolate = false;

        // Le sacamos la propia identidad a nuestro jugador. Ahora es el servidor el dueño de este objeto
        playerInstance.RemoveOwnership();

        playerInstance.transform.position = SpawnPoint.GetRandomSpawnPosition();

        yield return new WaitForSeconds(respawnTime);

        playerInstance.gameObject.SetActive(true);
        playerInstance.GetComponent<ClientNetwordTransform>().Interpolate = true;

        StartCoroutine(ChangeOwershipBack(playerInstance, previousOwnerClientId));
    }

    private IEnumerator ChangeOwershipBack(NetworkObject playerInstance, ulong previousOwnerClientId)
    {
        ResetHealth(playerInstance);

        yield return new WaitForSeconds(.5f);

        playerInstance.ChangeOwnership(previousOwnerClientId); // Le volvemos a dar la propieda
    }

    private static void ResetHealth(NetworkObject playerInstance)
    {
        Health health = playerInstance.GetComponent<Health>();

        health.currentHealth.Value = health.maxHealth;
        health.isDead = false;
    }
}
