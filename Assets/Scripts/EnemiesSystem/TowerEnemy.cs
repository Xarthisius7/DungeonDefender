using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class TowerEnemy : SampleEnemy
{ 
    [SerializeField] public float enemyAttackRange = 1.5f;
    [SerializeField] public float enemyAttackCooldown = 3f;
    [SerializeField] public Transform player;
    [SerializeField] public float TowerEnemySpeed = 1f;



    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;

    float distanceToPlayer;
    float distanceToCrystal;  

    Transform currentTargetTransform;  
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        this.moveSpeed = TowerEnemySpeed;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking && Vector2.Distance(transform.position, target.position) > enemyAttackRange){
            MoveTowardsTarget();
        }
        if (Vector2.Distance(transform.position, target.position) <= enemyAttackRange)
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

        //if (!isAlive)
        //    agent.SetDestination(transform.position);
    } 

    public override void SetDifficulty(float difficulty){
        base.SetDifficulty(difficulty);
        enemyAttackCooldown /= difficulty;
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
        //gameObject.GetComponent<NavMeshAgent>().isStopped = false;
        //distanceToPlayer = Vector2.Distance(transform.position, player.position);
        //distanceToCrystal = Vector2.Distance(transform.position, target.position);

        //if (distanceToPlayer < distanceToCrystal){
        //    currentTargetTransform = player;
        //    agent.SetDestination(player.position);
        //}
        //else{
        //    currentTargetTransform = target;
        //    agent.SetDestination(target.position);
        //}
        //flipSprite();
        //animator.SetBool("Walk", true);
    }


    void flipSprite(){
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

    public override void SetTower(Transform closestCrystal){
        this.target = closestCrystal;
    }
    public override void SetPlayer(Transform player){
        this.player = player;
    }

}