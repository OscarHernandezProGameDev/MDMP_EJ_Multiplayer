using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int maxHealth { get; private set; } = 100;

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public bool isDead;

    public event Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        currentHealth.Value = maxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healthValue)
    {
        ModifyHealth(healthValue);
    }

    private void ModifyHealth(int value)
    {
        if (isDead)
            return;

        int newHealth = currentHealth.Value + value;

        currentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);

        if (currentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            isDead = true;
        }
    }
}
