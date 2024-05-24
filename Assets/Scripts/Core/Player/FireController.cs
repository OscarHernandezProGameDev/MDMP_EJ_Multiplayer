using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class FireController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectileClient;
    [SerializeField] private GameObject projectileServer;
    [SerializeField] private GameObject projectileBase;
    [SerializeField] private Charger charger;
    [SerializeField] private SetPlayerData playerData;
    private int teamIndex;

    private bool isFiring;
    private bool isAiming;
    private Vector3 mouseWorldPosition;

    public override void OnNetworkSpawn()
    {
        inputReader.OnFireEvent += HandleFirePrimary;
        teamIndex = GetComponent<SetPlayerData>().TeamIndex.Value;
    }

    public override void OnNetworkDespawn()
    {
        inputReader.OnFireEvent -= HandleFirePrimary;
    }

    void Update()
    {
        if (!IsOwner) { return; }
        if (!isFiring) { return; }

        Fire();
    }

    private void HandleFirePrimary(bool isFiring)
    {
        this.isFiring = isFiring;
    }

    private void Fire()
    {
        isAiming = AimController.instance.isAimingStatus;

        if (isAiming == true)
        {
            mouseWorldPosition = AimController.instance.AimToRayPoint();
            Vector3 aimDirection = (mouseWorldPosition - projectileSpawnPoint.position).normalized;

            if (charger.TotalAmmo.Value > 0)
            {
                SpawnProjectileServerRpc(projectileSpawnPoint.position, aimDirection);
            }
            else
            {
                Debug.Log("NO AMMO");
            }

            isFiring = false;
        }

    }

    [ServerRpc]
    private void SpawnProjectileServerRpc(Vector3 projectileSpawnPoint, Vector3 aimDirection)
    {
        if (charger.TotalAmmo.Value < 0) { return; }

        charger.SpendAmmo();

        NetworkObject projectileInstance = NetworkObjectPool.Singleton.GetNetworkObject(projectileBase, projectileSpawnPoint, Quaternion.LookRotation(aimDirection, Vector3.up));

        DestroySelfOnContact destroySelf = projectileInstance.GetComponent<DestroySelfOnContact>();
        destroySelf.networkObject = projectileInstance;
        destroySelf.prefab = projectileBase;
        destroySelf.teamIndex = playerData.TeamIndex.Value;

        if (!projectileInstance.IsSpawned)
        {
            projectileInstance.Spawn();
        }

        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();

        rb.velocity = aimDirection * 10;

        if (projectileInstance.TryGetComponent<DealDamage>(out DealDamage damage))
        {
            damage.SetOwner(OwnerClientId);
            damage.SetTeamIndex(playerData.TeamIndex.Value);
        }
    }
}
