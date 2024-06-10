using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetPlayerData : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> TeamIndex = new NetworkVariable<int>();

    [field: SerializeField] public Health Health { get; private set; }

    public static event Action<SetPlayerData> OnPlayerSpawned;
    public static event Action<SetPlayerData> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = null;

#if UNITY_SERVER || UNITY_EDITOR
            if (IsHost)
            {
                userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
#else
            userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
#endif

            playerName.Value = userData.userName;
            TeamIndex.Value = userData.teamIndex;

            OnPlayerSpawned?.Invoke(this);
        }

        HandlePlayerNameChanged(string.Empty, playerName.Value);
        playerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes previousName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= HandlePlayerNameChanged;
        OnPlayerDespawned?.Invoke(this);
    }
}