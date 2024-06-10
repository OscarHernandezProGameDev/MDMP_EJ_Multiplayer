using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class OwnerNetworkAnimator : NetworkAnimator
{
    //protected override bool OnIsServerAuthoritative()
    //{
    //    return IsHost ? false : base.OnIsServerAuthoritative();
    //}
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
