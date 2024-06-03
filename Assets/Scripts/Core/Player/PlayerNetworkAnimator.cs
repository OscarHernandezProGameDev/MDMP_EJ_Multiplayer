using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerNetworkAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Component networkAnimatorComponent;
    [SerializeField] private NetworkAnimator networkAnimator;
    [SerializeField] private OwnerNetworkAnimator ownerNetworkAnimator;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        networkAnimator.enabled = false;
        ownerNetworkAnimator.enabled = false;
        if (HostSingleton.Instance == null)
        {
            ownerNetworkAnimator.enabled = true;
            networkAnimatorComponent = ownerNetworkAnimator;
        }
        else
        {
            networkAnimator.enabled = true;
            networkAnimatorComponent = networkAnimator;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        networkAnimator.enabled = false;
        ownerNetworkAnimator.enabled = false;
    }

    //private void Start()
    //{
    //    if (HostSingleton.Instance == null)
    //    {
    //        var networkAnimatorNoOwner = gameObject.AddComponent<NetworkAnimator>();

    //        networkAnimatorNoOwner.Animator = animator;

    //        networkAnimatorComponent = networkAnimatorNoOwner;
    //    }
    //    else
    //    {
    //        var networkAnimatorOwner = gameObject.AddComponent<OwnerNetworkAnimator>();

    //        networkAnimatorOwner.Animator = animator;

    //        networkAnimatorComponent = networkAnimatorOwner;
    //    }

    //    //networkAnimatorComponent.transform.parent = gameObject.transform;
    //}

    //private void OnDestroy()
    //{
    //    if (networkAnimatorComponent != null)
    //        Destroy(networkAnimatorComponent);
    //}

    //public override void OnNetworkSpawn()
    //{
    //    if (!IsOwner)
    //        return;

    //    if (HostSingleton.Instance == null)
    //    {
    //        var networkAnimatorNoOwner = gameObject.AddComponent<NetworkAnimator>();

    //        networkAnimatorNoOwner.Animator = animator;

    //        networkAnimatorComponent = networkAnimatorNoOwner;
    //    }
    //    else
    //    {
    //        var networkAnimatorOwner = gameObject.AddComponent<OwnerNetworkAnimator>();

    //        networkAnimatorOwner.Animator = animator;

    //        networkAnimatorComponent = networkAnimatorOwner;
    //    }

    //    //networkAnimatorComponent.transform.parent = gameObject.transform;
    //}

    //public override void OnNetworkDespawn()
    //{
    //    if (!IsOwner)
    //        return;

    //    if (networkAnimatorComponent != null)
    //        Destroy(networkAnimatorComponent);
    //}
}
