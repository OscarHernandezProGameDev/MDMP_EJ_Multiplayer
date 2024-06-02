using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraPriority : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera thirdPersonFollowCamera;
    [SerializeField] private CinemachineVirtualCamera thirdPersonAimCamera;
    [SerializeField] private int priority = 15;
    override public void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            thirdPersonFollowCamera.Priority = priority;
            thirdPersonAimCamera.Priority = priority;
        }
    }
}
