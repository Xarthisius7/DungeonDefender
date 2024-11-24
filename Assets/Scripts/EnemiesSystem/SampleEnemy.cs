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
    protected NavMeshAgent agent;

    public float health = 10;
    public float moveSpeed = 1.5f;
    public float enemyAttackDamage = 3f;


    public float lockOnDistance = 10f;
    bool hasLockedOn = false;

    protected Rigidbody2D rb;
    protected Animator animator;

    protected bool isAlive = true;
    protected const string animatorDead = "Dead";

    private SpriteRenderer sprite;


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

        if (isAlive)
        {
            MoveTowardsTarget();
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
        if (distanceToPlayer <= lockOnDistance)
            hasLockedOn = true;
        
        if (hasLockedOn)
            agent.SetDestination(target.position);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy Attacked player.");
            PlayerController.Instance.PlayerTakesDamage(enemyAttackDamage);
            Death();
        }
    }

    //public IEnumerator takeBulletDamage(float damage)
    public void takeBulletDamage(float damage)
    {
        health -= damage;
        if (health <= 0){
            Death();
        }
        else{
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

        animator.SetBool(animatorDead, true);
        isAlive = false;

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