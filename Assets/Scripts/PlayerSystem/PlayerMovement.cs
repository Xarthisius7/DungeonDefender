using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{



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


    private float currentMoveSpeed = 2f;
    //current movespeed of the player. can be effected by his actions.

    private float dashCooldownTimer; 
    private bool canDash = true; //is player in dash cooldown
    private bool isDashing = false;

    private float dashTime;   
    private Vector2 dashDirection;


    private float staminaRegenDelayTimer = 0f; // the counter of the stamina regen cooldown after shooting


    private float fireDamageCooldown = 0f;
    private float spikeDamageCooldown = 0f;


    private void Awake()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        Invoke("DelayInitStats", 0.1f);
    }

    private void DelayInitStats()
    {
        currentMoveSpeed = PowerupManager.instance.GetAttributeValue("Speed");
    }
    void Update()
    {
        if (GameController.Instance.IsPaused) return;

        movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        isShooting = InputManager.IsShooting;

        inputDashing = InputManager.IsDashing;

        if (!isDashing)
        {
            // If player is shooting, his movement speed is reduced.
            if (isShooting > 0)
            {
                currentMoveSpeed = PowerupManager.instance.GetAttributeValue("Speed") * shootingSpeedReduction;
            } else
            {
                currentMoveSpeed = PowerupManager.instance.GetAttributeValue("Speed");
            }

            CheckGroundEffect();
            if (fireDamageCooldown > 0) fireDamageCooldown -= Time.deltaTime;
            if (spikeDamageCooldown > 0) spikeDamageCooldown -= Time.deltaTime;

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


    void CheckGroundEffect()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("GroundEffect"));

        if (hit.collider != null)
        {
            string groundTag = hit.collider.tag;

            switch (groundTag)
            {
                case "Mud":
                    currentMoveSpeed *= 0.30f; 
                    break;

                case "Spike":
                    if (spikeDamageCooldown <= 0f)
                    {
                        PlayerController.Instance.PlayerTakesPercentDamage(0.15f);
                        spikeDamageCooldown = 3f; 
                    }

                    Animator spikeAnimator = hit.collider.GetComponent<Animator>();
                    if (spikeAnimator != null)
                    {
                        spikeAnimator.SetTrigger("Activate"); 
                    }

                    break;

                case "Fire":
                    if (fireDamageCooldown <= 0f)
                    {
                        StartCoroutine(ApplyFireDamage(0.01f, 6f)); // 1% damage for 6 second
                        fireDamageCooldown = 1f;
                    }
                    break;

                default:
                    // »Ö¸´Ä¬ÈÏ×´Ì¬
                    break;
            }
        }
    }

    IEnumerator ApplyFireDamage(float damagePerSecond, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            PlayerController.Instance.PlayerTakesPercentDamage(damagePerSecond);
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }
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
