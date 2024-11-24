using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class RangedEnemy : SampleEnemy
{ 
    public float minDistance = 1f;
    public float maxDistance = 2f;
    public float retreatSpeed = 200f;

    public GameObject projectilePrefab;
    public GameObject bulletSpawnPoint;
    public float projectileSpeed = 2.5f;
    public float shootingCooldown = 3f;

    private float shootingTimer = 0f;
    // SpriteRenderer sprite;
    private float distanceToPlayer;
    // private NavMeshAgent agent;

    void Start(){

        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = this.moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        sprite = GetComponent<SpriteRenderer>();
    }


    void FixedUpdate(){
        MoveAccordingToPlayer();
    }

    public override void SetDifficulty(float difficulty){
        base.SetDifficulty(difficulty);
        retreatSpeed *= difficulty;
        projectileSpeed += 0.5f * difficulty;

        if (difficulty > 1){
            shootingCooldown -= 0.3f;
        }

    }

    void MoveAccordingToPlayer(){
        distanceToPlayer = Vector2.Distance(transform.position, target.position);

        // Player has gotten too close and enemy must back up
        if (distanceToPlayer < minDistance){
            Vector2 directionAwayFromPlayer = (transform.position - target.position).normalized;
            Vector2 newPosition = (Vector2)transform.position + directionAwayFromPlayer * retreatSpeed * Time.deltaTime;
            //Vector2 newPosition = (Vector2)transform.position + directionAwayFromPlayer;
            // Vector2 newPosition = (Vector2)transform.position + directionAwayFromPlayer * retreatSpeed
            agent.SetDestination(newPosition);
            animator.SetBool("Walk", true);
        }
        // Enemy is too far from player, must move closer
        else if (distanceToPlayer > maxDistance){
            agent.SetDestination(target.position);
            animator.SetBool("Walk", true);
        }
        // Enemy is within range so stop them from moving
        else{
            agent.ResetPath();
            animator.SetBool("Walk", false);
            if (shootingTimer <= 0f){
                ShootAtPlayer();
            }
        }
        shootingTimer -= Time.deltaTime;
        flipSprite(target);
    }

    void ShootAtPlayer(){

        animator.SetBool("Idle", false);
        animator.SetBool("Attack", true);
        GameObject projectile = Instantiate(projectilePrefab, bulletSpawnPoint.transform.position, Quaternion.identity);


        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
        // Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), gameObject.transform.GetChild(0).GetComponent<Collider2D>(), true);

        projectile.GetComponent<EnemyBullet>().damage = enemyAttackDamage;
        projectile.transform.Rotate(0,0, -45f, Space.World);

        Vector2 projectileDirection = (target.position - bulletSpawnPoint.transform.position).normalized;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        rb.velocity = projectileDirection * projectileSpeed;
        projectile.transform.LookAt(target.position);

        shootingTimer = shootingCooldown;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy touched player.");
        }
    }

    protected override void flipSprite(Transform currentTargetTransform){
        Vector2 direction = transform.position - currentTargetTransform.position;

        if (Vector2.Dot(direction, Vector2.left) < Vector2.Dot(direction, Vector2.right))
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
    }

    void SetIdle(){
        animator.SetBool("Idle", true);
    }
    void ResetIdle(){
        animator.SetBool("Idle", false);
    }
}

