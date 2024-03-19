using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EntityVisuals))]
public abstract class Entity : MonoBehaviour, IDamagable
{
    public NavMeshAgent agent;
    public GameObject player; //The AI should be aware of the player at all times. 

    [Header("Entity Settings")]
    public int startingHealth; //health
    public int speed; //entity speed
    public int attackDamage; //entity damage per attack
    public float attackSpeed; //internval in time between attacks
    public float attackRange;  //attack range
    public int attackCount; //attacks that happen before cooldown occurs
    public int attackCooldown; //cooldown between attacks;
    public float hitFeedbackTime; //visual related 

    // [HideInInspector] public float internalAttackSpeed;
    //[HideInInspector] public float internalAttackCooldown;
    //[HideInInspector] public float internalAttackCount;

    public float internalAttackSpeed;
    public float internalAttackCooldown;
    public float internalAttackCount;

    [Header("Knockback Settings")]
    [Tooltip("The force that will knock the entity away.")]
    public float getKnockedBackDuration;
    [Tooltip("The duration of the knock back.")]
    public float getKnockedBackStrength;
    [Tooltip("This is for when enemy gets hit by bullet, so they are pushed away from the player and not the bullet.")]
    //It looks weird when knockback is based on knockback direction
    private bool gettingKnockedBack = false;
    [HideInInspector] public bool canGetKnockedBack = true;

    [Header("Behaviour")]
    [HideInInspector] public  GameObject target; //Target is usually player, but we keeping a dif var just in case. 
    [HideInInspector] public bool targetInRange;
    [HideInInspector] public EntityDirection direction;
    public EntityAIState state;
    public EntityStatus status = EntityStatus.DEFAULT;
    [HideInInspector] public EntityStatus previousStatus;
    [HideInInspector] public bool inViewOfPlayer;
    public int Health { get; set; }

    public EntityVisuals visuals;
    private Vector3 previousPos;
    private EntityDirection futureDir;
    private float previousX;

    [HideInInspector] public bool hasDied;
    [HideInInspector] public float freezeTimer;
    public bool isFrozen;
    [HideInInspector] public bool hasAppliedFrozenEffect;
    public GameObject deathParticleEffect;
    public virtual void SetUpAgent()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
    }

    public virtual void Freeze(float amount)
    {
        freezeTimer = amount;
        isFrozen = true;
    }

    public void SetAsCompanionTarget()
    {
        CompanionController.Instance.SetEntityTarget(this);
        UIManager.Instance.ShowPetTarget();
    }

    public void HandleFreeze()
    {
        if (isFrozen)
        {
           
            if (!hasAppliedFrozenEffect)
            {
                if (agent != null)
                {
                    agent.velocity = Vector3.zero;
                    agent.isStopped = true;
                }
               
                
                visuals.EffectFreeze();
                hasAppliedFrozenEffect = true;
            }

            freezeTimer -= Time.deltaTime;

            if (freezeTimer <= 0)
            {
                visuals.EffectUnfreeze();
                isFrozen = false;
                hasAppliedFrozenEffect = false;

                if (agent != null)
                {
                    agent.isStopped = false;
                }
                
            }

            return;
        }
    }

    public virtual void Damage(int amount)
    {
        visuals.VisualFeedbackHit(isFrozen);


        Health -= amount;
        if (Health <= 0)
        {
            if (GameManager.Instance.spawner != null && !hasDied)
            {
                Debug.Log("Enemy Died");
                GameManager.Instance.spawner.enemiesRemaining--;
                hasDied = true;
            }
            Instantiate(deathParticleEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            return;
        }
        StatusChange(EntityStatus.HIT, true);
       
    }



    public virtual void StatusChange(EntityStatus newStatus, bool revert)
    {
        if (newStatus == EntityStatus.HIT)
        {
            visuals.EntityAnimatorSetTrigger("Hit");
        }
        previousStatus = EntityStatus.DEFAULT;
        status = newStatus;

        if (revert)
        {
            visuals.UpdateStatus(this, previousStatus);
        }
        else
        {
            visuals.UpdateStatus(this);
        }

    }

    public virtual void DirectionChanged()
    {
        visuals.UpdateVisuals(this);
    }

    public virtual void SetSpeed(int amount)
    {
        speed = amount;
        SetUpAgent();
    }

    public virtual void SetHealth(int amount, bool fillHealth)
    {
        startingHealth = amount;
        if (fillHealth)
        {
            MaxHealth();
        }

    }

    public virtual void MaxHealth()
    {
        Health = startingHealth;
    }

    public virtual void UpdateDirection()
    {
        float xAxis = (transform.position - previousPos).x;

        if (xAxis < previousX)
        {
            futureDir = EntityDirection.LEFT;
        }
        else if (xAxis > previousX)
        {
            futureDir = EntityDirection.RIGHT;
        }

        if (futureDir != direction)
        {
            direction = futureDir;
            DirectionChanged();
        }

        previousX = xAxis;
    }



    #region Getting knocked back by an object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Entity") || collision.gameObject.CompareTag("Enemy"))
        {
            GetKnockedBack(collision.gameObject, getKnockedBackStrength);
        }
        else if (collision.gameObject.CompareTag("Bullet") && player != null)
        {
            if (collision.gameObject.GetComponent<Projectile>().nonPlayerOrigin != gameObject)
            {
                GetKnockedBack(player, getKnockedBackStrength);
            }
        }
        if (player == null) { Debug.Log("Player game object not assigned, knock back won't work with bullets!"); }
    }

    public virtual void GetKnockedBack(GameObject collidedGO, float amount)
    {
        if (amount != 0)
        {
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            Vector3 enemyDir = (collidedGO.transform.position - gameObject.transform.position).normalized;
            Vector3 knockBackDir = -enemyDir * amount;

            if (!gettingKnockedBack)
            {
                rb.velocity = Vector2.zero;
                if (gameObject.GetComponent<NavMeshAgent>() != null)
                {
                    gameObject.GetComponent<NavMeshAgent>().velocity = Vector2.zero;
                    gameObject.GetComponent<NavMeshAgent>().Move(knockBackDir);
                }
                else
                {
                    rb.AddForce(knockBackDir);
                }
                StartCoroutine(KnockBackDurationCo(getKnockedBackDuration));
                gettingKnockedBack = true;
            }
        }
    }
    //Waits certain duration before setting the velocity of the GO to zero, so the following movement is smooth
    public virtual IEnumerator KnockBackDurationCo(float duration)
    {
        yield return new WaitForSeconds(duration);
        gettingKnockedBack = false;
    }
    #endregion
}

public enum EntityAIState
{
    IDLE,
    MOVINGTOWARDS,
    MOVINGAWAY,
    ATTACKING,
}

public enum EnemyType
{
    Anger,
    Loneliness,
    Fear,
    Anxiety,
    Resentment,
    Hopelessness
}
