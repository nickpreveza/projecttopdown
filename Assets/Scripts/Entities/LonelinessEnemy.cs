using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LonelinessEnemy : Entity
{
    //IMPORTANT: Every game object of the player that has collider must be tagged as player";
    public float minDistanceFromPlayer;
    public GameObject gunParent;
    public GameObject bulletPrefab;

    [Header("Projectile Settings")]
    public float projectileSpeed;
    public float projectileRange;
    [SerializeField] GameObject bulletSpawnPosition;

    [Tooltip("White line is range. Red line is minimum range.")]
    public bool visualizeAim = true;
    public bool dontShoot;

    private int originalSpeed;
    private float stopDistance;

    ProjectileWeapon projectileWeapon;
    public bool playerInTarget;

    public Vector3 targetYoffset;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        visuals = GetComponent<EntityVisuals>();
        originalSpeed = speed;
        projectileWeapon = gunParent.transform.GetChild(0).GetComponent<ProjectileWeapon>();
    }

    public override void SetUpAgent()
    {
        base.SetUpAgent();
    }


    void Start()
    {
        if (attackRange < minDistanceFromPlayer)
        {
            Debug.LogWarning("Min distance is set lower than attack range! Loneliness will not function correctly.");
            attackRange = minDistanceFromPlayer;
        }

        Setup();
    }

    public void Setup()
    {
        target = PlayerController.Instance.gameObject;
        MaxHealth();
        SetUpAgent();
        SetUpWeapon();
        direction = EntityDirection.LEFT;
        player = PlayerController.Instance.gameObject;
        stopDistance = minDistanceFromPlayer + Vector3.Distance(bulletSpawnPosition.transform.position, transform.position);
        agent.stoppingDistance = stopDistance;
    }

    public override void Damage(int amount)
    {
        AudioManager.Instance.Play("rangedHit");
        base.Damage(amount);
    }

    void Update()
    {
        float distanceFromPlayer = DistanceFromPlayer();
        internalAttackSpeed += Time.deltaTime;
        internalAttackCooldown += Time.deltaTime;
        HandleFreeze();
        if (isFrozen)
        {
            return;
        }
        RotateGun();
        playerInTarget = PlayerInTarget();
        switch (state)
        {
            case EntityAIState.IDLE:
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                if (target != null)
                {
                    state = EntityAIState.MOVINGTOWARDS;
                }
                break;
            case EntityAIState.MOVINGTOWARDS:

                if (distanceFromPlayer <= attackRange)
                {
                    if (distanceFromPlayer < minDistanceFromPlayer)
                    {
                        state = EntityAIState.MOVINGAWAY;
                        return;
                    }

                    state = EntityAIState.ATTACKING;
                    return;
                }

                agent.isStopped = false;
                agent.stoppingDistance = stopDistance;
                agent.speed = speed;
                agent.SetDestination(target.transform.position);
               
                break;
            case EntityAIState.ATTACKING:

                if (distanceFromPlayer > attackRange)
                {
                    state = EntityAIState.MOVINGTOWARDS;
                    return;
                }

                if (distanceFromPlayer < minDistanceFromPlayer)
                {
                    state = EntityAIState.MOVINGAWAY;
                    return;
                }

                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                TryAttack();
                break;
            case EntityAIState.MOVINGAWAY:
                
                if (!playerInTarget || distanceFromPlayer > attackRange)
                {
                    state = EntityAIState.MOVINGTOWARDS;
                    return;
                }

                TryAttack();

                agent.isStopped = false;
                agent.stoppingDistance = 0;
                agent.speed = originalSpeed * (minDistanceFromPlayer / DistanceFromPlayer());
                try
                {
                    agent.SetDestination((2 * transform.position - target.transform.position));
                }
                catch
                {
                    Debug.Log("Runaway destination out of bounds. Should still work though.");
                }
                  
                break;
        }
    }

    public void TryAttack()
    {
        if (internalAttackCooldown > attackCooldown)
        {
            if (internalAttackCount < attackCount)
            {
                if (internalAttackSpeed > attackSpeed)
                {
                    internalAttackCount++;
                    internalAttackSpeed = 0;
                    AudioManager.Instance.Play("rangedAttack");
                    projectileWeapon.Pistol(Vector3.zero);
                }
            }
            else
            {
                internalAttackCooldown = 0;
                internalAttackCount = 0;
            }
        }
    }
    private void RotateGun()
    {
        Vector2 aimDir = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        gunParent.transform.rotation = Quaternion.Euler(0, 0, angle);
        UpdateDirection(angle);

        if (visualizeAim)
        {
            Debug.DrawRay(bulletSpawnPosition.transform.position, (bulletSpawnPosition.transform.right + targetYoffset) * attackRange);
            Debug.DrawRay(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.right * minDistanceFromPlayer, Color.red);
        }
    }

    private bool PlayerInTarget()
    {
        /*
         RaycastHit2D hit = Physics2D.Raycast(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.right, attackRange);
         if (hit.collider != null && hit.collider.CompareTag("Player"))
         {
             return true;
         }
         */

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.right + targetYoffset, attackRange);
      
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
         
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
           
        }
        return false;
    }

    public void SetUpWeapon()
    {
        projectileWeapon.damage = attackDamage;
        projectileWeapon.attackRate = attackSpeed;
        projectileWeapon.projectileLifetime = projectileRange;
        projectileWeapon.projectileSpeed = projectileSpeed;
    }

    public void UpdateDirection(float angle) //For flipping weapon
    {
        if (angle > 90 || angle < -90)
        {
            projectileWeapon.DirectionUpdate(WeaponDirection.LEFT);
        }
        else
        {
            projectileWeapon.DirectionUpdate(WeaponDirection.RIGHT);
        }
    }
  
    public float DistanceFromPlayer()
    {
        return Vector3.Distance(player.transform.position, this.transform.position);
    }
}
