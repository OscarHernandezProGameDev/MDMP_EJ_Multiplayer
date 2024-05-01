using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class AimController : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CinemachineFreeLook thirdPersonCamera;
    [SerializeField] private CinemachineFreeLook aimCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform fireTransform;

    [Header("Settings")]
    private bool isAimingStatus;
    [SerializeField] private float rotationSpeed = 20f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnAimEvent += HandleAim;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnAimEvent -= HandleAim;
    }

    private void HandleAim(bool isAiming)
    {
        isAimingStatus = isAiming;

        SetCamera(isAiming);
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if (isAimingStatus)
        {
            RotateToAimCamera();
        }
    }

    private void SetCamera(bool isAiming)
    {
        thirdPersonCamera.gameObject.SetActive(!isAiming);
        aimCamera.gameObject.SetActive(isAiming);
    }

    private Vector3 AimToRayPoint()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            fireTransform.position = raycastHit.point;
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
}
