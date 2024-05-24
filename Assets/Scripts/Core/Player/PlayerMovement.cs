using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Rigidbody rigidBody;
    private Transform _mTranform;
    private Transform mainCamera;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;
    private float rotationSmoothVelocity;
    private float rotationSmoothTime = .1f;

    private Vector3 previousMovementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        inputReader.OnMoveEvent += HandleMovement;
        _mTranform = transform;
        mainCamera = Camera.main.transform;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        inputReader.OnMoveEvent -= HandleMovement;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        Movement();
    }

    private void HandleMovement(Vector3 movementInput)
    {
        previousMovementInput = movementInput;
    }

    private void Movement()
    {
        float x = previousMovementInput.x;
        float z = previousMovementInput.z;

        Vector3 direction = new Vector3(x, 0f, z).normalized;

        if (direction.magnitude >= .1f)
        {
            float targeAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(_mTranform.eulerAngles.y, targeAngle, ref rotationSmoothVelocity, rotationSmoothTime);

            if (AimController.instance.isAimingStatus == false)
            {
                _mTranform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            Vector3 moveDirection = Quaternion.Euler(0f, targeAngle, 9f) * Vector3.forward;
            //characterController.Move(moveDirection * (movementSpeed * Time.deltaTime));
            rigidBody.position += moveDirection * (movementSpeed * Time.deltaTime);
        }
    }
}
