using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    void OnEnable()
    {
        inputReader.OnMoveEvent += HandlerMovement;
    }

    private void HandlerMovement(Vector3 movement)
    {
        Debug.Log($"Movement: {movement}");
    }
}
