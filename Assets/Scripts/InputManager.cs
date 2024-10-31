using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static float IsShooting;
    public static float IsDashing;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _shootAction;
    private InputAction _dashAction;


    //Class that manage the input. 
    //you can modify the input at "Input" floder - Controls
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        _shootAction = _playerInput.actions["Shooting"];
        _dashAction = _playerInput.actions["Dash"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        IsShooting = _shootAction.ReadValue<float>();
        IsDashing = _dashAction.ReadValue<float>();
    }
}
