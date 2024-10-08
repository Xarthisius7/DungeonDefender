using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEnemy : MonoBehaviour
{
    public Transform target;  // Target location
    public float moveSpeed = 3f;
    public float enemyAttackDamage = 3f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isAlive = true;

    private const string animatorDead = "Dead";

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // check if the game is paused.
        if (Time.timeScale == 0)
            return;


        if (isAlive)
        {
            Vector2 direction = (target.position - transform.position).normalized;

            //rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            //Directly set the enemy's position - but won't collde with environment obsticles. can be used later.

            rb.velocity = direction * moveSpeed;

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle Collision Detected");
            rb.velocity = Vector2.zero;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy Attacked player.");
            rb.velocity = Vector2.zero;
            PlayerController.Instance.takesDamage(enemyAttackDamage);

            Death();

        }
    }

    public void takeBulletDamage()
    {
        if (isAlive) { Death(); }
    }
    public void Death()
    {
        animator.SetBool(animatorDead, true);
        isAlive = false;
        rb.velocity = Vector2.zero;
        EffectsManager.Instance.PlaySFX(2);

        //remove the gameobject after finishing playing the animation.
        Invoke("DestorySelf", 2f);
    }

    void DestorySelf()
    {
        Destroy(gameObject);
    }
}
