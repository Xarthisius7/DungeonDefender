using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class SampleEnemy : MonoBehaviour
{
    [SerializeField]
    public int HurtSFXID;
    public int DeathSFXID;
    public int AttackSFXID;


    [SerializeField]
    public int HurtSFXVolume = 1;
    public int DeathSFXVolume = 1;
    public int AttackSFXVolume = 1;


    public Transform target;  // Target/Player location

    public float health = 10;
    public float moveSpeed = 1.5f;
    public float enemyAttackDamage = 3f;

    [SerializeField] public float enemyAttackRange = 1f;
    [SerializeField] public float attackCooldown = 2f;
    protected float attackCooldownTimer = 0f;

    public float lockOnDistance = 10f;
    protected bool hasLockedOn = false;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer sprite;
    protected NavMeshAgent agent;

    protected bool isAlive = true;
    protected const string animatorDead = "Dead";


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        sprite = GetComponent<SpriteRenderer>();

        //StartCoroutine(CustomUpdate());
    }

    // IEnumerator CustomUpdate()
    // {
    //     yield return new WaitForSeconds(1);
    //     // check if the game is paused.
    //     // if (Time.timeScale == 0)
    //     //     return;

    //     if (isAlive)
    //     {
    //         MoveTowardsTarget();
    //     }
    //     //
    //     //
    //     if (!isAlive)
    //         agent.SetDestination(transform.position);
        
    //     yield return new WaitForSeconds(2);
    // }

    void FixedUpdate()
    {
        // check if the game is paused.
        if (Time.timeScale == 0)
            return;

        // If we want to optimize runtime and limit function in update 
        // we could maybe get rid of the lock on distance and set 
        // the agent destination in the start method
        if (isAlive)
        {
            MoveTowardsTarget();
            attackCooldownTimer -= Time.deltaTime;
        }
        //
        //
        if (!isAlive)
            agent.SetDestination(transform.position);
    }

    public virtual void SetDifficulty(float difficulty){
        if (agent == null)
            Start();
            
        health *= difficulty;
        if (difficulty != 1f)
            moveSpeed  += 0.2f;
        agent.speed = moveSpeed;

        enemyAttackDamage *= difficulty;
        lockOnDistance += difficulty;
    }

    protected virtual void MoveTowardsTarget(){
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer <= lockOnDistance){
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
            hasLockedOn = true;
        }
        
        if (hasLockedOn){
            agent.SetDestination(target.position);
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            flipSprite(target);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Weird approach but works for now (checks to see if gameObject is 
        // sample/suicide enemy or spider enemy since both have this class as a component)
        string gameObjectType = gameObject.name.Substring(0, 6);
        if (collision.gameObject.CompareTag("Player") && gameObjectType == "Sample" && isAlive)
        {
            Debug.Log("Suicide enemy attacked the player.");
            PlayerController.Instance.PlayerTakesDamage(enemyAttackDamage);
            Death();
        }
        else if (collision.gameObject.CompareTag("Player") && gameObjectType == "Spider"){
            if (attackCooldownTimer <= 0f)
            {
                Debug.Log("PLAY ATTACK ANIMATION");
                animator.SetBool("Walk", false);
                animator.SetBool("Attack", true);

                PlayerController.Instance.PlayerTakesDamage(enemyAttackDamage);
                attackCooldownTimer = attackCooldown;
            }
            gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }

    //public IEnumerator takeBulletDamage(float damage)
    public void takeBulletDamage(float damage)
    {
        health -= damage;
        // Pop up damage dealt to enemy
        // UIManager.instance.popsUpDamage(float damage, Transform tf);
        if (health <= 0){
            Death();
        }
        else{
            // TO DO: PLAY TAKE DAMAGE SOUND
            Debug.Log("TODO : ENemyTake DAMAGE!");
            animator.SetBool("TakeDamage", true);
            //animator.SetTrigger("TrTakeDamage");
            //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            //animator.SetBool("TakeDamage", false);
        }
        // if (isAlive) { Death(); }
    }
    public void Death()
    {
        // TO DO: PLAY DEATH SOUND
        animator.SetBool(animatorDead, true);
        isAlive = false;
        gameObject.GetComponent<Collider2D>().enabled = false;

        EffectsManager.Instance.PlaySFX(2);

        //remove the gameobject after finishing playing the animation.
        Invoke("DestorySelf", 1.5f);
    }

    protected virtual void flipSprite(Transform currentTargetTransform){
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

    public void SetTarget(Transform target){
        this.target = target;
    }
    public virtual void SetTower(Transform closestCrystal){
        this.target = closestCrystal;
    }
    public virtual void SetPlayer(Transform player){
        Debug.Log("Used for Dynamic binding and does not contain anything");
    }

    public void DestorySelf()
    {
        Destroy(gameObject);
    }

    protected void ResetTakeDamage(){
        animator.SetBool("TakeDamage", false);
    }

    protected void ResetDeath(){
        animator.SetBool("Dead", false);
    }
    protected void ResetAttack(){
        animator.SetBool("Attack", false);
    }
    protected void ResetWalk(){
        animator.SetBool("Walk", false);
    }
}