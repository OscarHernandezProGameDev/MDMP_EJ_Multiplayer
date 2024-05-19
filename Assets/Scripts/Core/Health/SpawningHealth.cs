using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningHealth : Healing
{
    [SerializeField] private int value = 5;

    public override void Collect(Health health)
    {
        if (isCollected)
            return;

        isCollected = true;

        health.RestoreHealth(value);
    }

    public override void ResetStatus()
    {
        if (IsOwnedByServer)
            Spawn();

        if (!isCollected)
            return;

        isCollected = false;
    }
}
