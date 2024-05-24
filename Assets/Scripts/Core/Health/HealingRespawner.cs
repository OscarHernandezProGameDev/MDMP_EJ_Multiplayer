using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingRespawner : MonoBehaviour
{
    public static HealingRespawner Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetMedicKitToRespawn(SpawningHealth medicKit)
    {
        StartCoroutine(RespawnMedicKit(medicKit));
    }

    private IEnumerator RespawnMedicKit(SpawningHealth medicKit)
    {
        yield return new WaitForSeconds(3);

        medicKit.ResetStatus();
    }
}
