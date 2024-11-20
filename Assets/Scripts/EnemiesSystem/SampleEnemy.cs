using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class SampleEnemy : MonoBehaviour
{
    public Transform target;  // Target location
    protected NavMeshAgent agent;

    public float moveSpeed = 3f;
    public float enemyAttackDamage = 3f;

    public float lockOnDistance = 10f;
    bool hasLockedOn = false;

    protected Rigidbody2D rb;
    protected Animator animator;

    protected bool isAlive = true;
    protected const string animatorDead = "Dead";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void FixedUpdate()
    {
        // check if the game is paused.
        if (Time.timeScale == 0)
            return;

        if (isAlive)
        {
            MoveTowardsTarget();
        }
        if (!isAlive)
            agent.SetDestination(transform.position);
    }

    protected virtual void MoveTowardsTarget(){
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer <= lockOnDistance)
            hasLockedOn = true;
        
        if (hasLockedOn)
            agent.SetDestination(target.position);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
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
            PlayerController.Instance.PlayerTakesDamage(enemyAttackDamage);
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
        Invoke("DestorySelf", 1f);
    }

    public void DestorySelf()
    {
        Destroy(gameObject);
    }
}