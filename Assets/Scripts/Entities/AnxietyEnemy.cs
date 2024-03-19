using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy that explodes when near player. Can be diffused by dog.
/// </summary>
public class AnxietyEnemy : Entity
{
    public DamageType damageType;

    GameObject companion;

    [Header("Behaviour Settings")]
    public float distanceToTrigger; //disatnce to trigger exploision
    public float companionEffectRange; //range where the companion applies it's effect;
    public float diffusionTime; //times it takes for explosion to apply effect //you also need to update the animation if you want to change this
   
    public float healthDownDelay; //time needed for health to go down by 1
    public float healthUpDelay; // time needed for health to recover by 1
    int maxAnxietyHealth = 9; //starting health;
    int anxietyHealth;
    
    [SerializeField] float distanceFromCompanion;
    [SerializeField] float distanceFromPlayer;

    float upTimer;
    float downTimer;

    bool explodingInProcess;

    public List<GameObject> healthEyes;
    public bool hasDoneSound;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        visuals = GetComponent<EntityVisuals>();
    }

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        target = PlayerController.Instance.gameObject;
        MaxHealth();
        MaxAnxietyHealth();
        SetUpAgent();
        direction = EntityDirection.LEFT;
        player = PlayerController.Instance.gameObject;
        companion = CompanionController.Instance.gameObject;
    }

    void MaxAnxietyHealth()
    {
        anxietyHealth = maxAnxietyHealth;
        foreach(GameObject eye in healthEyes)
        {
            eye.SetActive(true);
        }
    }

    public override void SetUpAgent()
    {
        base.SetUpAgent();
    }

    public float DistanceFromPlayer()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    public float DistanceFromCompanion()
    {
        return Vector3.Distance(companion.transform.position, transform.position);
    }

    public override void Damage(int amount)
    {
        visuals.EffectShield();
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (DistanceFromCompanion() < companionEffectRange)
        {
            if (!hasDoneSound)
            {
                AudioManager.Instance.Play("growl");
                hasDoneSound = true;
            }
            upTimer = 0;
            downTimer += Time.deltaTime;
            if (downTimer >= healthDownDelay)
            {
                downTimer = 0f;
                anxietyHealth = Mathf.Clamp(anxietyHealth - 1, 0, maxAnxietyHealth);
                healthEyes[(anxietyHealth)].SetActive(false);
                if (anxietyHealth == 0)
                {
                    CheckDeath();
                    return;
                }

            }
        }
        else
        {
            hasDoneSound = false;
            downTimer = 0;
            if (anxietyHealth < maxAnxietyHealth)
            {
                upTimer += Time.deltaTime;

                if (upTimer >= healthUpDelay)
                {
                    upTimer = 0f;
                    anxietyHealth = Mathf.Clamp(anxietyHealth + 1, 0, maxAnxietyHealth);
                    healthEyes[(anxietyHealth - 1)].SetActive(true);

                }
            }

        }

        HandleFreeze();
        if (isFrozen)
        {
            return;
        }

        switch (state)
        {
            case EntityAIState.IDLE:

                if (target != null)
                {
                    state = EntityAIState.MOVINGTOWARDS;
                    visuals.EntityAnimatorSetTrigger("Walk");
                }

                break;
            case EntityAIState.ATTACKING:
                if (!explodingInProcess)
                {
                    StartCoroutine(ExplodeCoroutine());
                    explodingInProcess = true;
                }
                break;
            case EntityAIState.MOVINGAWAY:
            case EntityAIState.MOVINGTOWARDS:
             
                if (target == null)
                {
                    state = EntityAIState.IDLE;
                    visuals.EntityAnimatorSetTrigger("Idle");
                    return;
                }

                if (DistanceFromPlayer() <= distanceToTrigger)
                {
                    state = EntityAIState.ATTACKING;
                    return;
                }
                else
                {

                }

                agent.SetDestination(target.transform.position);
                break;
        }

       

    }

    void CheckDeath()
    {
        if (explodingInProcess)
        {
            return;
        }
        if (anxietyHealth <= 0)
        {
            AudioManager.Instance.Play("anxietyDeath");
            KillThis();
        }

        StatusChange(EntityStatus.HIT, true);
    }

    void KillThis()
    {
       
        if (GameManager.Instance.spawner != null && !hasDied)
        {
            Debug.Log("Enemy Died");
            GameManager.Instance.spawner.enemiesRemaining--;
            hasDied = true;
        }
        Instantiate(deathParticleEffect, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void Explode()
    {
        AudioManager.Instance.Play("anxietyExplode");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        for (int i = 0; i < colliders.Length-1; i++)
        {
            if (colliders[i].gameObject == this.gameObject)
            {
                continue;
            }
            else
            {
                if (colliders[i].gameObject.GetComponent<IDamagable>() != null)
                {
                    colliders[i].gameObject.GetComponent<IDamagable>().Damage(attackDamage);
                }
            }
           
        }
    }

    IEnumerator ExplodeCoroutine()
    {
        explodingInProcess = true;
        visuals.EntityAnimatorSetTrigger("Attack");
        yield return new WaitForSeconds(diffusionTime);
        Explode();
        yield return new WaitForSeconds(0.2f);
        KillThis();
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
        yield return new WaitForSeconds(0.2f);
        if (GameManager.Instance.spawner != null)
        {
            GameManager.Instance.spawner.enemiesRemaining--;
        }
        Destroy(this.gameObject);
    }
}
