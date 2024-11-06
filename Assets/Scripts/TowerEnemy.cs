using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class TowerEnemy : SampleEnemy
{ 
    [SerializeField] public float enemyAttackRange = 1.5f;
    [SerializeField] public float enemyAttackCooldown = 3f;

    [SerializeField] public Transform player;


    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;

    float distanceToPlayer;
    float distanceToCrystal;    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking){
                MoveTowardsTarget();
        }
        if (Vector2.Distance(transform.position, target.position) <= enemyAttackRange)
        {
            if (attackCooldownTimer <= 0f)
            {
                Debug.Log("ATTACK");
                Attack();
            }
        }
        attackCooldownTimer -= Time.deltaTime;

        if (!isAlive)
            agent.SetDestination(transform.position);
    } 

    // Attack the tower/crystal
    void Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;  // Stop moving

        // Trigger attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Delay before applying damage to simulate attack hit timing
        Invoke("ApplyDamage", 0.5f);  // Adjust the delay to match your attack animation
        
        // Reset cooldown
        attackCooldownTimer = enemyAttackCooldown;
    }

    // Apply damage to the tower after the attack animation
    void ApplyDamage()
    {
        TowerScript tower = target.GetComponent<TowerScript>();
        if (tower != null)
        {
            tower.TakeDamage(enemyAttackDamage);
        }
        isAttacking = false;  // Reset attack state
    }

    // Override of MoveTowardsTarget from SampleEnemy
    protected override void MoveTowardsTarget(){
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        distanceToCrystal = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer < distanceToCrystal)
            agent.SetDestination(player.position);
        else
            agent.SetDestination(target.position);
    }
}