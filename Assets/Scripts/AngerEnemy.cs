using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AngerEnemy : Entity
{    
    [SerializeField] float heightOffset;
    Vector3 gizmosPosition;

    float distanceFromPlayer;

    [SerializeField] EntityWeaponMelee weapon;
    bool changedState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        visuals = GetComponent<EntityVisuals>();
    }

    private void OnDrawGizmos()
    {
        gizmosPosition = transform.position;
        gizmosPosition.y += heightOffset;
        Gizmos.DrawWireSphere(gizmosPosition, attackRange);
    }


    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        target = PlayerController.Instance.gameObject;
        MaxHealth();
        SetUpAgent();
        direction = EntityDirection.LEFT;
        player = PlayerController.Instance.gameObject;
        weapon.attackDamage = attackDamage;
        AudioManager.Instance.Play("angerAmbient");
    }

    public override void Damage(int amount)
    {
        AudioManager.Instance.Play("meleeHit");
        base.Damage(amount);
    }

    public override void SetUpAgent()
    {
        base.SetUpAgent();
    }

    
    // Update is called once per frame
    void Update()
    {
        distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);
        internalAttackSpeed += 1 * Time.deltaTime;

        HandleFreeze();
        if (isFrozen)
        {
            return;
        }
        if (!agent.isStopped)
        {

        }
        if (GameManager.Instance.playerDead)
        {
            target = null;
        }
        switch (state)
        {
            case EntityAIState.IDLE:

                if (target != null)
                {
                    state = EntityAIState.MOVINGTOWARDS;
                    visuals.EntityAnimatorSetTrigger("Walk");
                    changedState = true;
                }
              
                return;

            case EntityAIState.MOVINGAWAY:
            case EntityAIState.MOVINGTOWARDS:
                changedState = false;

                if (target == null)
                {
                    state = EntityAIState.IDLE;
                    visuals.EntityAnimatorSetTrigger("Idle");
                    changedState = true;
                    return;
                }

                agent.SetDestination(target.transform.position);

              
                if (distanceFromPlayer <= attackRange)
                {
                    targetInRange = true;
                    state = EntityAIState.ATTACKING;
                    visuals.EntityAnimatorSetTrigger("AttackMode");
                    changedState = true;
                    return;
                }
                else
                {
                    targetInRange = false;
                    if (target == null)
                    {
                        agent.isStopped = false;
                        state = EntityAIState.IDLE;
                        visuals.EntityAnimatorSetTrigger("Idle");
                        return;
                    }
                }
                break;
            case EntityAIState.ATTACKING:

                if (target == null)
                {
                    agent.isStopped = false;
                    state = EntityAIState.IDLE;
                    changedState = true;
                    visuals.EntityAnimatorSetTrigger("Idle");
                    return;
                }

               

                if (distanceFromPlayer <= attackRange)
                {
                    changedState = false;
                    agent.isStopped = true;

                    if (internalAttackSpeed > attackSpeed)
                    {
                        //convert this to collider check
                        AudioManager.Instance.Play("meleeAttack");
                        PlayerController.Instance.Damage(attackDamage);
                        weapon.haveAppliedDamage = false;
                        visuals.EntityAnimatorSetTrigger("Attack");
                        internalAttackSpeed = 0;
                    }

                    return;
                }
                else
                {
                    targetInRange = false;
                   
                    if (target == null)
                    {
                        agent.isStopped = false;
                        state = EntityAIState.IDLE;
                        changedState = true;
                        visuals.EntityAnimatorSetTrigger("Idle");
                        return;
                    }
                    else
                    {
                        agent.isStopped = false;
                        state = EntityAIState.MOVINGTOWARDS;
                        visuals.EntityAnimatorSetTrigger("Walk");
                        changedState = true;
                        return;
                    }
                }
        }
    }

   
}
   

