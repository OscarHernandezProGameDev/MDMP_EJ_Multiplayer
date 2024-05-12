using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraPriority : NetworkBehaviour
{
    [SerializeField] private CinemachineFreeLook thirdPersonCamera;
    [SerializeField] private int priority = 15;
    override public void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            thirdPersonCamera.Priority = priority;
        }
    }
}
