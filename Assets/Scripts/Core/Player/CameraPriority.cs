using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Cinemachine;
using UnityEngine;

public class CameraPriority : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera thirdPersonFollowCamera;
    [SerializeField] private CinemachineVirtualCamera thirdPersonAimCamera;
    [SerializeField] private int priority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            thirdPersonFollowCamera.Priority = priority;
            thirdPersonAimCamera.Priority = priority;
        }
    }
}
