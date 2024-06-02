using Cinemachine;
using System.Collections;
using System.Collections.Generic;

using StarterAssets;

using Unity.Netcode;
using UnityEngine;


public class ThirdPersonShooterController : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform projectileSpwanPoint;
    [SerializeField] private GameObject projectileBase;
    [SerializeField] private Charger charger;
    [SerializeField] private SetPlayerData playerData;
    private StarterAssetsInputs _input;

    [Header("Settings")]
    private bool isAiming;
    private Vector3 mouseWorldPosition;
    [SerializeField] private float rotationSpeed = 20f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        _input = GetComponent<StarterAssetsInputs>();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        SetCameraActive();
        Fire();
    }

    private void SetCameraActive()
    {
        isAiming = _input.aim;
        aimCamera.gameObject.SetActive(isAiming);
        if (isAiming)
            RotateToAimCamera();
    }

    public Vector3 AimToRayPoint()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        }

        return mouseWorldPosition;
    }

    private void RotateToAimCamera()
    {
        Vector3 aimTarget = AimToRayPoint();

        aimTarget.y = transform.position.y;
        Vector3 aimDirection = (aimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotationSpeed);
    }

    private void Fire()
    {
        if (isAiming)
        {
            mouseWorldPosition = AimToRayPoint();

            Vector3 aimDirection = (mouseWorldPosition - projectileSpwanPoint.position).normalized;

            if (_input.shoot)
            {
                if (charger.TotalAmmo.Value > 0)
                {
                    SpawnProjectileServerRpc(projectileSpwanPoint.position, aimDirection);

                }
                else
                {
                    Debug.Log("No ammo");
                }
                _input.shoot = false;
            }
        }
    }

    [ServerRpc]
    private void SpawnProjectileServerRpc(Vector3 projectileSpwanPoint, Vector3 aimDirection)
    {
        if (charger.TotalAmmo.Value <= 0)
            return;

        charger.SpendAmmo();

        NetworkObject projectileInstance = NetworkObjectPool.Singleton.GetNetworkObject(projectileBase, projectileSpwanPoint, Quaternion.LookRotation(aimDirection, Vector3.up));

        DestroySelfOnContact destroySelf = projectileInstance.GetComponent<DestroySelfOnContact>();

        destroySelf.NetworkObject = projectileInstance;
        destroySelf.Prefab = projectileBase;
        destroySelf.teamIndex = playerData.TeamIndex.Value;

        // Nos aseguramos que se instance en los cientes
        if (!projectileInstance.IsSpawned)
        {
            projectileInstance.Spawn();
        }

        // Este c�digo estaba en BulletProjectile pero hay que hacerlo aqui porque ya no creamos la instancia sino usamos el pool

        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();

        rb.velocity = aimDirection * 10;

        if (projectileInstance.TryGetComponent<DealDamage>(out var damage))
        {
            damage.SetOwner(OwnerClientId);
            damage.SetTeamIndex(playerData.TeamIndex.Value);
        }
    }
}
