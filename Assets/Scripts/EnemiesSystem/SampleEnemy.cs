using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    private GameObject HealthBar;
    private float selfDifficulty = 1;

    public Transform target;  // Target/Player location

    public float health = 10;
    private float maxHealth;
    public float moveSpeed = 1.0f;
    public float enemyAttackDamage = 3f;

    public float levelIncrementHP = 10f;
    public float levelIncrementMoveSpeed = 0.2f;
    public float levelIncrementAttackDamage = 3f;
    public bool IsNormalSpawned = true; // if it got 20% chance to drop item when died



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
        maxHealth = health;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        sprite = GetComponent<SpriteRenderer>();

        //StartCoroutine(CustomUpdate());
    }

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

    public void UpdateHealthBar(float percentage, float level)
    {
        if (HealthBar == null)
        {
            GameObject healthBarPrefab = Resources.Load<GameObject>("Prefabs/UI/HealthBar");
            if (healthBarPrefab != null)
            {
                HealthBar = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, transform);

                Transform levelTextTransform = HealthBar.transform.Find("Level Text");
                if (levelTextTransform != null)
                {
                    TextMeshProUGUI levelText = levelTextTransform.GetComponent<TextMeshProUGUI>();
                    if (levelText != null)
                    {
                        levelText.text = "Lv. " + Mathf.FloorToInt(level);
                    }
                }
            }
            else
            {

                return;
            }
        }

        Transform amountDisplayTransform = HealthBar.transform.Find("Frame").Find("AmountDisplay");
        if (amountDisplayTransform != null)
        {
            Image healthImage = amountDisplayTransform.GetComponent<Image>();
            if (healthImage != null)
            {
                healthImage.fillAmount = Mathf.Clamp01(percentage);
            }
            else
            {
                Debug.LogError("Image component not found on AmountDisplay object.");
            }
        }
        else
        {
            Debug.LogError("AmountDisplay object not found in HealthBar.");
        }
    }


    public virtual void SetDifficulty(float difficulty)
    {
        if (agent == null)
            Start();
        //the difficulty is default set to 1 (area 1)
        health += levelIncrementHP * (difficulty - 1);
        maxHealth = health;
        moveSpeed += levelIncrementMoveSpeed * (difficulty - 1);
        agent.speed = moveSpeed;

        enemyAttackDamage += levelIncrementAttackDamage * (difficulty - 1);
        lockOnDistance += difficulty;
        selfDifficulty = difficulty;
        UpdateHealthBar(1, selfDifficulty);
    }


    protected virtual void MoveTowardsTarget()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer <= lockOnDistance)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
            hasLockedOn = true;
        }

        if (hasLockedOn)
        {
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
            PlayerController.Instance.PlayerTakesDamage(enemyAttackDamage);
            EffectsManager.Instance.PlaySFX(AttackSFXID, AttackSFXVolume);
            DeathSlient();
        }
        else if (collision.gameObject.CompareTag("Player") && gameObjectType == "Spider")
        {
            if (attackCooldownTimer <= 0f)
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Attack", true);

                PlayerController.Instance.PlayerTakesDamage(enemyAttackDamage);
                attackCooldownTimer = attackCooldown;
                EffectsManager.Instance.PlaySFX(AttackSFXID, AttackSFXVolume);
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
        if (health <= 0)
        {
            Death();
        }
        else
        {
            // TO DO: PLAY TAKE DAMAGE SOUND


            UpdateHealthBar((health / maxHealth), selfDifficulty);

            EffectsManager.Instance.PlaySFX(HurtSFXID, HurtSFXVolume);

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

        EffectsManager.Instance.PlaySFX(DeathSFXID, DeathSFXVolume);

        //remove the gameobject after finishing playing the animation.
        Invoke("DroppedItem", 1.0f);
        Invoke("DestorySelf", 1.5f);
    }

    public void DroppedItem()
    {
        float prob1 = 82f; // for common item
        float prob2 = 5f;  // for equipment
        float prob3 = 12f; // for key - just a notice. can be ignored.
        float randomValue = Random.Range(0f, 100f);

        // 20% that enemy will drop item. enemy in thow defense will not drop item.
        if (Random.Range(0f, 1f) <= 0.2f && IsNormalSpawned)
        {
            if (randomValue < prob1)
            {
                // common item
                ItemManager.Instance.CreateDroppedItem(
                    ItemManager.Instance.GetRandomConsumeableID(),
                    1,
                    transform
                );
            }
            else if (randomValue < prob1 + prob2)
            {
                // equipment
                ItemManager.Instance.CreateDroppedItem(
                    ItemManager.Instance.GetRandomEquipmentID(),
                    1,
                    transform
                );
            }
            else
            {
                // key
                ItemManager.Instance.CreateDroppedItem(16, 1, transform);
            }
        }

    }

    public void DeathSlient()
    {
        // TO DO: PLAY DEATH SOUND
        animator.SetBool(animatorDead, true);
        isAlive = false;
        gameObject.GetComponent<Collider2D>().enabled = false;

        //remove the gameobject after finishing playing the animation.
        Invoke("DestorySelf", 1.5f);
    }


    protected virtual void flipSprite(Transform currentTargetTransform)
    {
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

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public virtual void SetTower(Transform closestCrystal)
    {
        this.target = closestCrystal;
    }
    public virtual void SetPlayer(Transform player)
    {
        Debug.Log("Used for Dynamic binding and does not contain anything");
    }

    public void DestorySelf()
    {
        Destroy(gameObject);
    }

    protected void ResetTakeDamage()
    {
        animator.SetBool("TakeDamage", false);
    }

    protected void ResetDeath()
    {
        animator.SetBool("Dead", false);
    }
    protected void ResetAttack()
    {
        animator.SetBool("Attack", false);
    }
    protected void ResetWalk()
    {
        animator.SetBool("Walk", false);
    }
}