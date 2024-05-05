using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<NetworkObject>(out var netVar))
        {
            if (netVar.OwnerClientId == ownerClientId)
            {
                return;
            }
        }
        if (other.gameObject.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(damage);
        }
    }
}
