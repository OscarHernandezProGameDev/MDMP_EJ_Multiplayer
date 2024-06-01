using Cinemachine;
using System.Collections;
using System.Collections.Generic;

using StarterAssets;

using Unity.Netcode;
using UnityEngine;


public class ThirdPersonShjooterController : MonoBehaviour
//: NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    private StarterAssetsInputs _input;

    [Header("Settings")]
    private bool isAiming;
    [SerializeField] private float rotationSpeed = 20f;

    //public override void OnNetworkSpawn()
    //{
    //    if (!IsOwner)
    //        return;

    //    _input.OnAimEvent += HandleAim;

    //    instance = this;

    //    rootHeadInitalPosition = rootHead.transform.localPosition;
    //}

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

    }

    //public override void OnNetworkDespawn()
    //{
    //    if (!IsOwner)
    //        return;

    //    _input.OnAimEvent -= HandleAim;
    //}

    void Update()
    {
        //if (!IsOwner)
        //    return;

        //if (isAimingStatus)
        //{
        //    RotateToAimCamera();
        //}
        SetCameraActive();
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
}
