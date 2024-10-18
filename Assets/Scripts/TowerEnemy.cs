using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEnemy : SampleEnemy
{ 
    public float enemyAttackRange = 1.5f;
    public float enemyAttackCooldown = 2f;
    public float attackCooldownTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive){
                MoveTowardsTarget();
        }
    } 
}
