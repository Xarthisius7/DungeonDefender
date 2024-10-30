using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEnemy : SampleEnemy
{ 
    [SerializeField] public float enemyAttackRange = 1.5f;
    [SerializeField] public float enemyAttackCooldown = 3f;
    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
}
