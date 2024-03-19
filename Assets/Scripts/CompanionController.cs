using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionController : MonoBehaviour
{
    public static CompanionController Instance;

    [Header("Components")]
    [SerializeField] GameObject companionLight;
    NavMeshAgent agent;
    GameObject player;
    [HideInInspector]
    public GameObject targetToAttack;
    GameObject ballToFetch;

    [Header("Stats")]
    [SerializeField] float speed;
    [SerializeField] float anxietyRange;
    [SerializeField] float distanceToStartFollowing;
    [SerializeField] float distanceToAttack;
    [SerializeField] float stoppingDistance;
    [SerializeField] float waitTimeAtLocation;

    [Header("Visuals")]
    [SerializeField] CompanionVisuals visuals;
    public EntityDirection currentDir = EntityDirection.LEFT;
    private EntityDirection futureDir;
    private Vector3 previousPos;
    private float previousX;

    [Header("Abilities")]
    public bool ability_ballFollow;
    public bool petHeal;
    public bool ability_stay;
    public bool ability_attack;
    public bool petHeal2;
    public bool ability_freeze;
    public bool petFreeze;
    public bool petDamage;



    [Header("Ability Stats")]
    [SerializeField] float pettingDistance;
    [SerializeField] float pettingCooldown;
    [SerializeField] float fetchDistance;
    [SerializeField] int pettingHealAmount;
    [SerializeField] int pettingDamageAmount;
    [SerializeField] float pettingDamageRange;
    [SerializeField] float pettingFreezeTime;
    [SerializeField] float pettingFreezeRange;
    [SerializeField] float freezingCooldown;
    [SerializeField] float attackDamage;
    [SerializeField] float attackDistance;
    [SerializeField] float attackStoppingDistance;
    [SerializeField] float attackCooldown;

    [Header("Behaviour")]
    public CompanionState currentState = CompanionState.FOLLOW;
    public bool isMoving;
    public bool isSitting;
    public float panicTimer;
    public bool pettingPossible;
    public bool freezingPossible;
    public bool hasBallToReturn;
    //internal stuff
    bool stay;
    float distanceFromPlayer;
    float distanceFromTarget;
    Vector3 mousePos;
    float internalWaitTimer;
    float internalPettingCooldown;
    float internalPanicTimer;
    float internalFreezingtimer;
    float internalAttackTimer;
    bool overrideStayTimer;
    

    public float freezingPercentage {get { return Mathf.Clamp(internalFreezingtimer / freezingCooldown, 0, 1); }}
    public float pettingPercentage { get{ return Mathf.Clamp(internalPettingCooldown / pettingCooldown, 0, 1); }}

    float promptTimer;
    private void Awake()
    {
        Instance = this;
        internalPettingCooldown = pettingCooldown;
    }

    void Start()
    {
        player = PlayerController.Instance.gameObject;
        visuals = GetComponent<CompanionVisuals>();   
        PlayerController.Instance.companion = this.gameObject;
        SetupAgent();

        if (GameManager.Instance.hub != null)
        {
            companionLight.SetActive(false);
        }

        UpdateAbilities();
    }

    public void AddSpeed(int amount)
    {
        speed += amount;
        agent.speed = speed;
    }

    public void AddDamage(int amount)
    {
        attackDamage += amount;
    }

    void SetupAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
    }

    public void UpdateAbilities()
    {
        for (int i = 0; i <= DataManager.Instance.publicData.relationshipLevel-1; i++)
        {
            ActivateRelationshipLevelAbilities(i);
        }

        UIManager.Instance.UpdateAbilityElements();
        internalFreezingtimer = freezingCooldown;
        internalPettingCooldown = pettingCooldown;
    }

    public void ActivateRelationshipLevelAbilities(int levelIndex)
    {
        DogAbilities currentAbilityToUnlock = GameManager.Instance.dogAbilitiesUnlockOrder[levelIndex];
        switch (currentAbilityToUnlock)
        {
            case DogAbilities.BALL:
                ability_ballFollow = true;
                break;
            case DogAbilities.PETHEAL:
                petHeal = true;
                pettingHealAmount = 1;
                break;
            case DogAbilities.STAY:
                ability_stay = true;      
                break;
            case DogAbilities.ATTACK:
                ability_attack = true;
                break;
            case DogAbilities.PETHEAL2:
                petHeal2 = true;
                pettingHealAmount = 2;
                break;
            case DogAbilities.FREEZE:
                ability_freeze = true;
                break;
            case DogAbilities.PETDAMAGE:
                petDamage = true;
                break;
            case DogAbilities.PETFREEZE:
                petFreeze = true;
                break;
        }
    }

    public void OverrideAndStay(Vector3 location)
    {
        overrideStayTimer = true;
        StayAtDestination(location);
    }

    public void StayAtPosition()
    {
        AudioManager.Instance.Play("stay");
        if (currentState == CompanionState.PANIC || !ability_stay)
        {
            return;
        }

        UIManager.Instance.ShowPetLocation();
        TryRemoveTarget();
        internalWaitTimer = waitTimeAtLocation;
        currentState = CompanionState.STAYCOMMAND;
    }

    public void StayAtDestination(Vector3 position, Entity targetEntity = null)
    {
        if (currentState == CompanionState.PANIC || !ability_stay)
        {
            return;
        }

        if (currentState == CompanionState.FETCHBALL)
        {
            hasBallToReturn = false;
        }


        isSitting = false;
        agent.isStopped = false;

       

        if (targetEntity != null)
        {
            //TryRemoveTarget(targetEntity);
            //targetEntity.visuals.TargetToggle(true);
            //argetToAttack = targetEntity.gameObject;
            //currentState = CompanionState.ATTACKCOMMAND;
            //UIManager.Instance.ShowPetTarget();
        }
        else
        {
            if (!overrideStayTimer)
            {
                UIManager.Instance.ShowPetLocation();
            }
            TryRemoveTarget();
            agent.SetDestination(position);
            currentState = CompanionState.MOVECOMMAND;
        }
    }

    public void CallPet()
    {
        AudioManager.Instance.Play("come");
        if (currentState == CompanionState.PANIC)
        {
            return;
        }

        if (currentState == CompanionState.FETCHBALL)
        {
            hasBallToReturn = false;
        }

        TryRemoveTarget();
        visuals.EffectCall();
        currentState = CompanionState.FOLLOW;
        UIManager.Instance.ShowCallPrompt();
    }

    void TryRemoveTarget(Entity targetEntity = null)
    {
        if (currentState == CompanionState.ATTACKCOMMAND && targetToAttack != null)
        {
            if (targetEntity != null && targetEntity == targetToAttack.GetComponent<Entity>())
            {
                return;
            }

            UIManager.Instance.HidePetTarget();
            targetToAttack.GetComponent<Entity>().visuals.TargetToggle(false);
        }
    }

    public void FetchBall(GameObject newBall)
    {
        if (currentState == CompanionState.FETCHBALL)
        {
            Destroy(ballToFetch.gameObject);
            hasBallToReturn = false;
        }
        UIManager.Instance.HidePetLocation();
        ballToFetch = newBall;
        TryRemoveTarget();
        currentState = CompanionState.FETCHBALL;
    }

    public void SetEntityTarget(Entity target)
    {
        if (!ability_attack)
        {
            return;
        }
        TryRemoveTarget();
        targetToAttack = target.gameObject;
        currentState = CompanionState.ATTACKCOMMAND;
    }

    void Update()
    {
      
        UpdateDirection();

        distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);
        internalAttackTimer += Time.deltaTime;

        if (distanceFromPlayer > anxietyRange)
        {
            if (!PlayerController.Instance.shouldStress)
            {
              
                PlayerController.Instance.shouldStress = true;
            }
         
        }
        else
        {
            PlayerController.Instance.shouldStress = false;
        }

        CheckPetAvailability();
        
        if (ability_freeze)
        {
            CheckFreezingAvailability();
        }
        else
        {
            freezingPossible = false;
        }
       

        switch (currentState)
        {
            case CompanionState.FOLLOW:
                UIManager.Instance.HidePetLocation();
                isSitting = false;
                agent.isStopped = false;
                agent.SetDestination(player.transform.position);
                agent.stoppingDistance = stoppingDistance;

                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        {
                            if (hasBallToReturn)
                            {
                                visuals.HideBall();
                                player.GetComponent<TennisBallThrow>().canThrow = true;
                                hasBallToReturn = false;
                            }
                            currentState = CompanionState.PLAYERREACHED;
                            return;
                        }
                    }
                }
                break;
            case CompanionState.PLAYERREACHED:
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                isSitting = true;
                visuals.Stay();
                if (distanceFromPlayer > distanceToStartFollowing)
                {
                    currentState = CompanionState.FOLLOW;
                }
                break;
            case CompanionState.MOVECOMMAND:

                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        {
                            internalWaitTimer = waitTimeAtLocation;
                            currentState = CompanionState.STAYCOMMAND;

                        }
                    }
                }
                break;
            case CompanionState.STAYCOMMAND:
                
              
                internalWaitTimer -= Time.deltaTime;
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                isSitting = true;
                visuals.Stay();
                if (overrideStayTimer)
                {
                    return;
                }
                if (internalWaitTimer <= 0)
                {
                    UIManager.Instance.HidePetLocation();
                    currentState = CompanionState.FOLLOW;
                }
                break;
            case CompanionState.ATTACKCOMMAND:

                isSitting = false;
                agent.isStopped = false;

                if (targetToAttack == null)
                {
                    currentState = CompanionState.FOLLOW;
                    return;
                }

                distanceFromTarget = Vector3.Distance(targetToAttack.transform.position, transform.position);

                if (distanceFromTarget < distanceToAttack)
                {
                    if (internalAttackTimer > attackCooldown)
                    {
                        AttackEntity(targetToAttack.GetComponent<Entity>());
                        internalAttackTimer = 0;
                    }
                }
                else
                {
                    agent.SetDestination(targetToAttack.transform.position);
                    agent.stoppingDistance = stoppingDistance;
                }
                break;
            case CompanionState.PANIC:
                internalPanicTimer -= Time.deltaTime;
                //agent.SetDestination();
                break;
            case CompanionState.FETCHBALL:
                if (ballToFetch == null)
                {
                    visuals.HideBall();
                    currentState = CompanionState.FOLLOW;
                    return;
                }

                float distanceFromBall = Vector3.Distance(ballToFetch.transform.position, transform.position);
                isSitting = false;
                agent.isStopped = false;
                agent.SetDestination(ballToFetch.transform.position);
                agent.stoppingDistance = stoppingDistance;

                if (distanceFromBall < fetchDistance)
                {
                    visuals.ShowBall();
                    Destroy(ballToFetch);
                    hasBallToReturn = true;
                    currentState = CompanionState.FOLLOW;
                    return;
                }
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        {
                            visuals.ShowBall();
                            Destroy(ballToFetch);
                            hasBallToReturn = true;
                            currentState = CompanionState.FOLLOW;
                            return;
                        }
                    }
                }

                break;
            case CompanionState.RETURNBALL:

                break;
        }  
       
    }

    public void Freeze()
    {
        FreezeAbility();
        internalFreezingtimer = 0;
        freezingPossible = false;
        UIManager.Instance.UpdateAbilityElements();
    }

    void CheckFreezingAvailability()
    {
        internalFreezingtimer += Time.deltaTime;

        bool availability;

        if (internalFreezingtimer >= freezingCooldown)
        {
            availability = true;

        }
        else
        {
            availability = false;

        }

        if (freezingPossible != availability)
        {
            freezingPossible = availability;
            UIManager.Instance.UpdateAbilityElements();
        }
    }

    void CheckPetAvailability()
    {
        internalPettingCooldown += Time.deltaTime;

        bool pettingAvailability;
        
        if (distanceFromPlayer <= pettingDistance && internalPettingCooldown >= pettingCooldown)
        {
            pettingAvailability = true;

        }
        else
        {
            pettingAvailability = false;

        }

        if (pettingAvailability != pettingPossible)
        {
            pettingPossible = pettingAvailability;
            visuals.ToggleTongue(!pettingAvailability);
            UIManager.Instance.UpdateAbilityElements();
            if (pettingPossible)
            {
                UIManager.Instance.ShowPetPrompt();
            }
            else
            {
                UIManager.Instance.HidePetPrompt();
            }
        }
    }

 

    public void Pet()
    {
        visuals.Pet();
        AudioManager.Instance.Play("dogpanting");
        PlayerController.Instance.ChangeEmotionState(EntityStatus.HAPPY, true, 2f);
        if (GameManager.Instance.hub != null)
        {
            UIManager.Instance.HidePetPrompt();
            //DataManager.Instance.AddRelationshipLevel();
            if (!GameManager.Instance.hub.DogPet)
            {
                DataManager.Instance.AddRelationshipLevel();
                GameManager.Instance.hub.DogPet = true;
            }
        }

        if (petHeal)
        {
            visuals.EffectHeal();
            PlayerController.Instance.visuals.EffectHealthUp();
            player.GetComponent<PlayerController>().Heal(pettingHealAmount);
           
        }

        if (petFreeze)
        {
            FreezeAbility();
        }

        pettingPossible = false;
        internalPettingCooldown = 0;
        UIManager.Instance.HidePetPrompt();
        UIManager.Instance.UpdateAbilityElements();
    }

    public void FreezeAbility()
    {
        AudioManager.Instance.Play("freeze");
        if (petDamage || ability_attack)
        {
            visuals.AttackAbilityEffect();
        }
        else
        {
            visuals.FreezeAbilityEffect();
        }
      
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pettingFreezeRange);

        
        for (int i = 0; i < colliders.Length - 1; i++)
        {
            if (colliders[i].gameObject == this.gameObject)
            {
                continue;
            }
            else
            {
                if (colliders[i].gameObject.GetComponent<Entity>() != null)
                {
                    colliders[i].gameObject.GetComponent<Entity>().Freeze(pettingFreezeTime);
                    if (ability_attack || petDamage)
                    {
                      
                        AttackEntity(colliders[i].gameObject.GetComponent<Entity>());
                    }
                }
            }

        }
    }

    public void DamageAbility()
    {
        visuals.AttackAbilityEffect();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pettingDamageRange);
        for (int i = 0; i < colliders.Length - 1; i++)
        {
            if (colliders[i].gameObject == this.gameObject)
            {
                continue;
            }
            else
            {
                if (colliders[i].gameObject.GetComponent<Entity>() != null)
                {
                   
                }
            }

        }
    }

    public void AttackEntity(Entity targetEntity)
    {
        visuals.AttackAnimation();
        targetEntity.Damage(pettingDamageAmount);
    }

    public void UpdateDirection()
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

        if (futureDir != currentDir)
        {
            currentDir = futureDir;
            visuals.UpdateDirection(currentState, currentDir);
        }

        previousX = xAxis;

        bool newMovingState;
        if (agent.isStopped)
        {
            newMovingState = false;
        }
        else
        {
            newMovingState = true;
        }

        if (newMovingState != isMoving)
        {
            isMoving = newMovingState;

            visuals.SetMove(isMoving, isSitting);
        }
    }
}


public enum CompanionState
{
    FOLLOW, //follows player
    PLAYERREACHED, //
    STAYCOMMAND,
    ATTACKCOMMAND,
    MOVECOMMAND,
    PANIC,
    FETCHBALL,
    RETURNBALL
}

public enum DogAbilities
{
    BALL,
    PETHEAL,
    STAY,
    PETHEAL2,
    ATTACK,
    FREEZE,
    PETDAMAGE,
    PETFREEZE
}

