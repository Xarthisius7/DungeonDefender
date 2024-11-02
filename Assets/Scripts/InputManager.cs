using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static float IsShooting;
    public static float IsDashing;

    public static float IsUsingItem;
    public static float IsDropingItem;
    public static Vector2 scrollValue;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _shootAction;
    private InputAction _dashAction;
    private InputAction _scrollAction;

    private InputAction _useItemAction;
    private InputAction _dropItemAction;





    //Class that manage the input. 
    //you can modify the input at "Input" floder - Controls
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        _shootAction = _playerInput.actions["Shooting"];

        _dashAction = _playerInput.actions["Dash"];

        _useItemAction = _playerInput.actions["UseItem"];
        _dropItemAction = _playerInput.actions["DropItem"];

        _scrollAction = _playerInput.actions["Scroll"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        IsShooting = _shootAction.ReadValue<float>();
        IsDashing = _dashAction.ReadValue<float>();

        IsUsingItem = _useItemAction.ReadValue<float>();
        IsDropingItem = _dropItemAction.ReadValue<float>();

        scrollValue = _scrollAction.ReadValue<Vector2>();



     
    }
}
