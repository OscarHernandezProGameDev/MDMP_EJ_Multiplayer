using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingRespawner : MonoBehaviour
{
    public static HealingRespawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetMedicKitToRespawn(SpawningHealth medicKit)
    {
        StartCoroutine(RespawmMedicKit(medicKit));
    }

    private IEnumerator RespawmMedicKit(SpawningHealth medicKit)
    {
        yield return new WaitForSeconds(3f);
        medicKit.ResetStatus();
    }
}
