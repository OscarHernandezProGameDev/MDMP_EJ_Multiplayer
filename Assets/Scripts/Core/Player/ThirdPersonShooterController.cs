using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class ThirdPersonShooterController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectileBase;
    [SerializeField] private Charger charger;
    [SerializeField] private SetPlayerData playerData;
    private StarterAssetsInputs _input;
    private int teamIndex;
    private Vector3 mouseWorldPosition;
    private Animator _animator;

    [Header("Settings")]
    private bool isAiming;
    [SerializeField] private float rotationSpeed = 20f;

    // Animation Settings
    private int _animIDAim;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        _input = GetComponent<StarterAssetsInputs>();
        _animator = GetComponent<Animator>();
        _animIDAim = Animator.StringToHash("IsAiming");
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        SetCameraActive();
        Fire();
    }

    private void SetCameraActive()
    {
        isAiming = _input.aim;

        if (isAiming)
        {
            aimCamera.gameObject.SetActive(true);
            _animator.SetBool(_animIDAim, true);
            RotateToAimCamera();
        }
        else if (!isAiming)
        {
            aimCamera.gameObject.SetActive(false);
            _animator.SetBool(_animIDAim, false);
        }
    }

    public Vector3 AimToRayPoint()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            if (raycastHit.transform.gameObject != gameObject)
            {
                mouseWorldPosition = raycastHit.point;
            }
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
        if (!isAiming) { _input.shoot = false; }

        if (isAiming)
        {
            mouseWorldPosition = AimToRayPoint();
            Vector3 aimDirection = (mouseWorldPosition - projectileSpawnPoint.position).normalized;

            if (_input.shoot)
            {
                if (charger.TotalAmmo.Value > 0)
                {
                    SpawnProjectileServerRpc(projectileSpawnPoint.position, aimDirection);
                    _input.shoot = false;
                }
                else
                {
                    Debug.Log("NO AMMO");
                }
            }
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

        rb.velocity = aimDirection * projectileInstance.GetComponent<BulletProjectile>().Speed;

        if (projectileInstance.TryGetComponent<DealDamage>(out DealDamage damage))
        {
            damage.SetOwner(OwnerClientId);
            damage.SetTeamIndex(playerData.TeamIndex.Value);
        }
    }
}
