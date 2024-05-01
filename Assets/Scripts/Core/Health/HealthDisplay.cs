using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image healthImageUI;

    public override void OnNetworkSpawn()
    {
        if (!IsClient)
            return;

        health.currentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, health.currentHealth.Value);
    }

    override public void OnNetworkDespawn()
    {
        if (!IsClient)
            return;

        health.currentHealth.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(int oldHealth, int newHealth)
    {
        healthImageUI.fillAmount = (float)newHealth / health.maxHealth;
    }
}
