using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterController characterController;
    private Transform _mTransform;
    private Transform mainCamera;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;
    private float rotationSmoothVelocity;
    private float rotationSmoothTime = 0.1f;

    private Vector3 previousMovementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnMoveEvent += HandleMovement;
        _mTransform = transform;
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        if (!IsOwner)
            return;

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

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(_mTransform.eulerAngles.y, targetAngle, ref rotationSmoothVelocity, rotationSmoothTime);

            _mTransform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(movementDirection * (movementSpeed * Time.deltaTime));
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnMoveEvent -= HandleMovement;
    }
}
