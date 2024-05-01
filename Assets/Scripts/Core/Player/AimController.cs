using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class AimController : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CinemachineFreeLook thirdPersonCamera;
    [SerializeField] private CinemachineFreeLook aimCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnAimEvent += HandleAim;
    }

    private void HandleAim(bool isAiming)
    {
        thirdPersonCamera.gameObject.SetActive(!isAiming);
        aimCamera.gameObject.SetActive(isAiming);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnAimEvent -= HandleAim;
    }
}
