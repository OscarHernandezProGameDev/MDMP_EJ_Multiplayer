using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class AimController : NetworkBehaviour
{
    public static AimController instance;

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CinemachineFreeLook thirdPersonCamera;
    [SerializeField] private CinemachineFreeLook aimCamera;
    [SerializeField] private GameObject rootHead;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform fireTransform;

    [Header("Settings")]
    public bool isAimingStatus;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private Vector3 rootHeadInitialPosition;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        inputReader.OnAimEvent += HandleAim;

        instance = this;

        rootHeadInitialPosition = rootHead.transform.localPosition;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        inputReader.OnAimEvent -= HandleAim;
    }

    private void HandleAim(bool isAiming)
    {
        SetCamera(isAiming);
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        if (isAimingStatus == true)
        {
            RotateToAimCamera();
        }
    }

    private void SetCamera(bool isAiming)
    {
        isAimingStatus = isAiming;

        if (isAiming == true)
        {
            thirdPersonCamera.gameObject.SetActive(false);
            aimCamera.gameObject.SetActive(true);
            rootHead.transform.localPosition = new Vector3(.5f, 1.5f, 0);
        }
        else if (isAiming == false)
        {
            aimCamera.gameObject.SetActive(false);
            thirdPersonCamera.gameObject.SetActive(true);
            rootHead.transform.localPosition = rootHeadInitialPosition;
        }
    }

    public Vector3 AimToRayPoint()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            if (raycastHit.transform.gameObject != gameObject)
            {
                fireTransform.position = raycastHit.point;
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
}
