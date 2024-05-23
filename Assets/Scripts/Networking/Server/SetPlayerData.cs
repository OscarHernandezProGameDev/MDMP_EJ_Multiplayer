using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class SetPlayerData : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [field: SerializeField] public Health Health { get; private set; }
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<SetPlayerData> OnPlayerSpawned;
    public static event Action<SetPlayerData> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = null;

            if (IsHost)
            {
                userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            playerName.Value = userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }

        HandlePlayerNameChanged(string.Empty, playerName.Value);
        playerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        playerNameText.text = newValue.ToString();
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= HandlePlayerNameChanged;
        OnPlayerDespawned?.Invoke(this);
    }
}
