using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private float dashDistance = 0.5f;
    [SerializeField] private float dashDuration = 0.07f;
    [SerializeField] private float dashDefaultCooldown = 1f; // the default cooldown of dash. can be reduced from powerups.

    [SerializeField] private float shootingSpeedReduction = 0.25f;
    //how much player speed is effected when he is shooting

    [SerializeField] public PlayerAction playerAction;

    //Animator const variables.
    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";
    private const string mouseHorizontal = "MouseHorizontal";
    private const string mouseVertical = "MouseVertical";
    //Variables for animator - to show the last IDLE phase of player's movement
    private const string lastHorizontal = "LastHorizontal";
    private const string lastVertical = "LastVertical";
    private const string animatorShooting = "IsShooting";
    private const string animatorIsDashing = "IsDashing";


    private Camera mainCam;
    private Vector3 mousePos;

    //movement - the actuall movement record from Input Manager
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private float isShooting;
    private float inputDashing;


<<<<<<< HEAD
=======
    private float currentMoveSpeed = 2f;
    //current movespeed of the player. can be effected by his actions.

    private float dashCooldownTimer; 
    private bool canDash = true; //is player in dash cooldown
    private bool isDashing = false;

    private float dashTime;   
    private Vector2 dashDirection;


    private float staminaRegenDelayTimer = 0f; // the counter of the stamina regen cooldown after shooting


>>>>>>> master
    private void Awake()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        isShooting = InputManager.IsShooting;

        inputDashing = InputManager.IsDashing;

        if (!isDashing)
        {
            // If player is shooting, his movement speed is reduced.
            if (isShooting > 0)
            {
                currentMoveSpeed = moveSpeed * shootingSpeedReduction;
            } else
            {
                currentMoveSpeed = moveSpeed;
            }
            rb.velocity = movement * currentMoveSpeed;

            // handle the dash cooldown
            if (!canDash)
            {
                dashCooldownTimer -= Time.deltaTime;
                if (dashCooldownTimer <= 0f)
                {
                    canDash = true; 
                }
            }

            // if the dash is ready & input key is down, then dash
            if (inputDashing > 0 && canDash)
            {
                if (PlayerController.Instance.TryUseStamiaToDash())
                {
                    StartDash();
                }
            }
        }
        else
        {
            // update the dash cooldown
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                EndDash();
            }
        }




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

        //seting the animation when player is shooting:
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 playerPos = transform.position;

        Vector3 direction = mousePos - playerPos;
        Vector3 normalizedDirection = direction.normalized;
        // Normalized direction - between player character and mouse position.
        // then update the anomitor.
        animator.SetFloat(mouseHorizontal, normalizedDirection.x);
        animator.SetFloat(mouseVertical, normalizedDirection.y);


        if (isShooting > 0 && !isDashing)
        {
            staminaRegenDelayTimer = PlayerController.Instance.shootingStaminaDelay; 

            playerAction.Shoot(mousePos); 
        }
        else
        {
            //disable the display of CrossHair when player is not shooting
            playerAction.rotationPoint.SetActive(false);
        }

        PassiveStaminaRegen();
    }
    private void PassiveStaminaRegen()
    {
        if ( isShooting <= 0)
        {
            if (staminaRegenDelayTimer <= 0f)
            {
                PlayerController.Instance.RegenTimeFixedStamina();
            }
            else
            {
                staminaRegenDelayTimer -= Time.deltaTime;
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        canDash = false; // update the dash cooldown



        animator.SetFloat(animatorIsDashing, 1f);

        dashTime = dashDuration;
        dashCooldownTimer = dashDefaultCooldown;  // set the dash cooldown
        dashDirection = movement.normalized; // set the direction of dash to the normalized movement
        rb.velocity = dashDirection * dashDistance / dashDuration; // set the dash speed.

        // TODO: dash animation, dash sound effect
        EffectsManager.Instance.PlaySFX(4);
    }

    void EndDash()
    {
        isDashing = false;
        animator.SetFloat(animatorIsDashing, 0f);
        rb.velocity = Vector2.zero; // stop the player when the dash is finished.


    }


}
