using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class TowerEnemy : SampleEnemy
{ 
    [SerializeField] public Transform player;
    // [SerializeField] public float TowerEnemySpeed = 1f;

    // private float attackCooldownTimer = 0f;
    private bool isAttacking = false;
    float distanceToPlayer;
    float distanceToCrystal;  

    Transform currentTargetTransform;  
    // protected SpriteRenderer sprite;


    // Start is called before the first frame update
    void Start()
    {
        // this.moveSpeed = TowerEnemySpeed;
        currentTargetTransform = target;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        sprite = GetComponent<SpriteRenderer>();

        //StartCoroutine(CustomUpdate());
    }

    // Update is called once per frame
    // IEnumerator CustomUpdate()
    // {
    //     yield return new WaitForSeconds(1);
    //     if (!isAttacking && Vector2.Distance(transform.position, target.position) > enemyAttackRange){
    //         MoveTowardsTarget();
    //     }
    //     if (Vector2.Distance(transform.position, target.position) <= enemyAttackRange)
    //     {
    //         if (attackCooldownTimer <= 0f)
    //         {
    //             //Debug.Log("ATTACK");
    //             Attack();
    //             //StartCoroutine(Attack());
    //             //StopCoroutine(Attack());
    //         }
    //     }
    //     attackCooldownTimer -= Time.deltaTime;

    //     yield return new WaitForSeconds(2);

    //     //if (!isAlive)
    //     //    agent.SetDestination(transform.position);
    // } 

    // // Update is called once per frame
    void FixedUpdate()
    {
        if (!isAttacking){
            MoveTowardsTarget();
        }
        if (Vector2.Distance(transform.position, currentTargetTransform.position) <= enemyAttackRange)
        {
            if (attackCooldownTimer <= 0f)
            {
                //Debug.Log("ATTACK");
                Attack();
                //StartCoroutine(Attack());
                //StopCoroutine(Attack());
            }
        }
        attackCooldownTimer -= Time.deltaTime;

        if (!isAlive)
           agent.SetDestination(transform.position);
    } 

    public override void SetDifficulty(float difficulty){
        base.SetDifficulty(difficulty);
        attackCooldown /= difficulty;
    }

    // Attack the tower/crystal
    //IEnumerator Attack()
    void Attack()
    {
        isAttacking = true;
        //rb.velocity = Vector2.zero;  // Stop moving
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;

        // Trigger attack animation
        if (animator != null)
        {
            //animator.SetTrigger("Attack");
            Debug.Log("PLAY ATTACK ANIMATION");
            animator.SetBool("Walk", false);
            animator.SetBool("Attack", true);
            //animator.SetTrigger("TrAttack");
            //yield return new WaitForSeconds(1.5f);
            //Debug.Log("Play attack animation");
            //animator.SetBool("Attack", false);
        }

        // Delay before applying damage to simulate attack hit timing
        Invoke("ApplyDamage", 1f);  // Adjust the delay to match your attack animation
        // Reset cooldown
        attackCooldownTimer = attackCooldown;
    }

    // Apply damage to the tower after the attack animation
    void ApplyDamage()
    {
        // Apply damage to player or tower
        // TowerScript tower = target.GetComponent<TowerScript>();
        // if (tower != null)
        // {
        //     tower.TakeDamage(enemyAttackDamage);
        // }
        isAttacking = false;  // Reset attack state

        // Apply damage to tower/crystal
        if (currentTargetTransform.gameObject.name == target.gameObject.name){
            target.gameObject.GetComponent<TowerScript>().TakeDamage(enemyAttackDamage);
        }
        // apply damage to Player
        else if (currentTargetTransform.gameObject.name == player.gameObject.name){
            PlayerController.Instance.PlayerTakesDamage(enemyAttackDamage);
        }
    }

    // Override of MoveTowardsTarget from SampleEnemy
    protected override void MoveTowardsTarget(){
        gameObject.GetComponent<NavMeshAgent>().isStopped = false;
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        distanceToCrystal = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer < distanceToCrystal){
            currentTargetTransform = player;
            agent.SetDestination(player.position);
        }
        else{
            currentTargetTransform = target;
            agent.SetDestination(target.position);
        }
        flipSprite(currentTargetTransform);
        animator.SetBool("Walk", true);
    }


    // protected void flipSprite(){
    //     Vector2 direction = transform.position - currentTargetTransform.position;

    //     if (Vector2.Dot(direction, Vector2.left) < Vector2.Dot(direction, Vector2.right))
    //     {
    //         sprite.flipX = true;
    //     }
    //     else
    //     {
    //         sprite.flipX = false;
    //     }
    // }

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



    // Simple override (for some reason if the parent method is set to private this is still inherited)
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Deal damage to player");
        }
    }

    public override void SetTower(Transform closestCrystal){
        this.target = closestCrystal;
    }
    public override void SetPlayer(Transform player){
        this.player = player;
    }

    private void wait(){
    }

}