using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    public static DestroySelfOnContact instance;

#if !NO_POOLING
    public NetworkObject NetworkObject;
    public GameObject Prefab;
#endif
    public int teamIndex;
    [SerializeField] private bool destroyOnFriendlyFire;

    private void Awake()
    {
        instance = this;
    }

    void OnTriggerEnter(Collider other)
    {
#if NO_POOLING
        Destroy(gameObject);
#else
        if (other.TryGetComponent(out SetPlayerData playerData))
        {
            var playerTeamIndex = playerData.TeamIndex.Value;

            if (playerTeamIndex >= 0 && playerTeamIndex == teamIndex && !destroyOnFriendlyFire)
            {
                Debug.Log("Mismo equipo atraviesa objeto");
                return;
            }
        }

        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, Prefab);

        Debug.Log("Destruye bala", other.gameObject);

        // Nos aseguramos que se instance en los cientes
        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(false);
        }
#endif
    }
}
