using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private InputActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void Update()
    {
        Vector2 move = _inputActions.Player.Move.ReadValue<Vector2>();

        if (Mathf.Approximately(move.x, 0.0f) && Mathf.Approximately(move.y, 0.0f))
        {
            // nothing
        }
        else
        {
        }
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
}
