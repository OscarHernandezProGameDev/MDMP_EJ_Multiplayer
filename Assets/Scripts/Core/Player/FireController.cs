using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FireController : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpwanPoint;
    [SerializeField] private GameObject projectileClient;
    [SerializeField] private GameObject projectileServer;
    [SerializeField] private GameObject projectileBase;
    private bool isAiming;
    private Vector3 mouseWorldPosition;
    private bool isFiring;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnFireEvent += HandleFirePrimary;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnAimEvent -= HandleFirePrimary;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if (!isFiring)
            return;

        Fire();
    }

    private void HandleFirePrimary(bool isFiring)
    {
        this.isFiring = isFiring;
    }

    private void Fire()
    {
        isAiming = AimController.instance.isAimingStatus;
        if (isAiming)
        {
            mouseWorldPosition = AimController.instance.AimToRayPoint();

            Vector3 aimDirection = (mouseWorldPosition - projectileSpwanPoint.position).normalized;

            SpawnProjectileServerRpc(projectileSpwanPoint.position, aimDirection);
            //SpawnDummyProjectile(projectileSpwanPoint.position, aimDirection);
        }
    }

    private void SpawnDummyProjectile(Vector3 projectileSpwanPoint, Vector3 aimDirection)
    {
        Instantiate(projectileClient, projectileSpwanPoint, Quaternion.LookRotation(aimDirection, Vector3.up));
        isFiring = false;
    }

    [ServerRpc]
    private void SpawnProjectileServerRpc(Vector3 projectileSpwanPoint, Vector3 aimDirection)
    {
        //GameObject projectileInstance = Instantiate
        //(
        //    projectileServer, projectileSpwanPoint, Quaternion.LookRotation(aimDirection, Vector3.up)
        //);
        NetworkObject projectileInstance = NetworkObjectPool.Singleton.GetNetworkObject(projectileBase, projectileSpwanPoint, Quaternion.LookRotation(aimDirection, Vector3.up));

        DestroySelfOnContact destroySelf = projectileInstance.GetComponent<DestroySelfOnContact>();

        destroySelf.NetworkObject = projectileInstance;
        destroySelf.Prefab = projectileBase;

        // Nos aseguramos que se instance en los cientes
        if (!projectileInstance.IsSpawned)
        {
            projectileInstance.Spawn();
        }

        // Este código estaba en BulletProjectile pero hay que hacerlo aqui porque ya no creamos la instancia sino usamos el pool

        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();

        rb.velocity = aimDirection * 10;

        if (projectileInstance.TryGetComponent<DealDamage>(out var damage))
        {
            damage.SetOwner(OwnerClientId);
        }

        isFiring=false;

        //SpawnProjectileClientRpc(projectileSpwanPoint, aimDirection);
    }

    [ClientRpc]
    private void SpawnProjectileClientRpc(Vector3 projectileSpwanPoint, Vector3 aimDirection)
    {
        if (IsOwner)
            return;

        SpawnDummyProjectile(projectileSpwanPoint, aimDirection);
    }
}
