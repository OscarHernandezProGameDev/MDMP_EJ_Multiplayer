using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualJoysticks : MonoBehaviour
//: NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private bool enabledIfEditorMode;

    [Header("References")]
    public UICanvasControllerInput joystickCanvas;

    public void ActiveVirtualJoystick(StarterAssetsInputs starterAssetsInputs, PlayerInput playerInput)
    {
        this.starterAssetsInputs = joystickCanvas.starterAssetsInputs = starterAssetsInputs;
        starterAssetsInputs.cursorLocked = false;
        starterAssetsInputs.cursorInputForLook = false;
        this.playerInput = playerInput;
        joystickCanvas.gameObject.SetActive(true);
        DisableAutoSwitchControls();
    }

    //public override void OnNetworkSpawn()
    //{
    //    if (IsServer)
    //    {
    //        SetPlayerData.OnPlayerSpawned += HandlePlayerSpawned;
    //        SetPlayerData.OnPlayerDespawned += HandlePlayerDespawned;
    //    }
    //}

    //public override void OnNetworkDespawn()
    //{
    //    if (IsServer)
    //    {
    //        SetPlayerData.OnPlayerSpawned -= HandlePlayerSpawned;
    //        SetPlayerData.OnPlayerDespawned -= HandlePlayerDespawned;
    //    }
    //}

    //public void HandlePlayerSpawned(SetPlayerData player)
    //{
    //    if (player.OwnerClientId == NetworkManager.LocalClientId)
    //        ActiveVirtualJoystickIfNecesary(player);
    //}

    //public void HandlePlayerDespawned(SetPlayerData player)
    //{
    //    if (player.OwnerClientId == NetworkManager.LocalClientId)
    //        DesActiveVirtualJoystickIfNecesary(player);
    //}

    //private void ActiveVirtualJoystickIfNecesary(SetPlayerData player)
    //{
    //    starterAssetsInputs = joystickCanvas.starterAssetsInputs = player.StarterAssetsInputs;
    //    starterAssetsInputs.cursorLocked = false;
    //    starterAssetsInputs.cursorInputForLook = false;
    //    playerInput = player.PlayerInput;
    //    joystickCanvas.gameObject.SetActive(true);
    //    DisableAutoSwitchControls();
    //}

    //private void DesActiveVirtualJoystickIfNecesary(SetPlayerData player)
    //{
    //    starterAssetsInputs = null;
    //    playerInput = null;
    //    joystickCanvas.gameObject.SetActive(false);
    //}

    void DisableAutoSwitchControls()
    {
        if (playerInput == null)
            return;

        playerInput.neverAutoSwitchControlSchemes = true;
    }
}
