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

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;

    private Vector3 previousMovementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnMoveEvent += HandleMovement;
    }

    void Update()
    {
        characterController.Move(previousMovementInput * (movementSpeed * Time.deltaTime));
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputReader.OnMoveEvent -= HandleMovement;
    }

    private void HandleMovement(Vector3 movementInput)
    {
        previousMovementInput = movementInput;
    }
}
