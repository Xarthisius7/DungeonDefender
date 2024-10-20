using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] public PlayerAction playerAction;

    //Animator const variables.
    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";
    //Variables for animator - to show the last IDLE phase of player's movement
    private const string lastHorizontal = "LastHorizontal";
    private const string lastVertical = "LastVertical";
    private const string animatorShooting = "IsShooting";


    //movement - the actuall movement record from Input Manager
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private float isShooting;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        isShooting = InputManager.IsShooting;
        rb.velocity = movement * moveSpeed;

        //update the animator
        animator.SetFloat(animatorShooting, isShooting);
        animator.SetFloat(horizontal, movement.x);
        animator.SetFloat(vertical, movement.y);

        //when player stopped moving, let the animator show the player's IDLE direction
        if (movement != Vector2.zero)
        {
            animator.SetFloat(lastHorizontal, movement.x);
            animator.SetFloat(lastVertical, movement.y);
        }

        if (isShooting > 0)
        {
            playerAction.Shoot(); 
        }
    }

    //The shooting function. gonna be a huge part of the code later.
    void Shoot()
    {

    }
}
