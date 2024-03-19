using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using MoreMountains.Feedbacks;
using MoreMountains.Feel;

[RequireComponent(typeof(PlayerVisualsController))]
public class PlayerController : MonoBehaviour, IDamagable
{
    [SerializeField] Volume globalVolume;
    ColorAdjustments colorAdjustments;
    [SerializeField] Gradient postGradient;

    public static PlayerController Instance;

    Rigidbody2D rb;

    [HideInInspector]
    public PlayerVisualsController visuals;

    [Header("Character State")]
    public bool isRunning;
    public bool isMoving;
    public EntityDirection currentDir = EntityDirection.LEFT;
    public EntityStatus emotionState = EntityStatus.DEFAULT;
    [SerializeField] EntityStatus previousState;
    [SerializeField] float hitVisualLenght;
    [Header("Stats")]

    public int startingHealth = 10;
    public int Health { get; set; }
    [SerializeField] float tempSpeed;

    [Header("Attack")]
    bool hasActiveWeapon;
    [SerializeField] GameObject gunParent;
    [SerializeField] Weapon activeWeapon;
    public Weapon[] weapons;
    public GameObject companion;

    //PlayerRotation
    float moveX;
    float moveY;
    Vector2 tempPos;
    Vector2 newVel;
    
    EntityDirection futureDir;
    Vector3 mousePos;
    Vector3 aimDir;
    Vector3 euAngle;
    float angle;
    bool petFollow = true;

    [Header("Interactables")]
    public float interactableRadius;
    [SerializeField] LayerMask interactablesLayer;
    [SerializeField] LayerMask entitiesLayer;
    [SerializeField] Interactable currentInteractable;

    [SerializeField] GameObject ballPrefab;
    public bool controlsActive = true;

    public int stressThreshold;
    public float stressUpMultiplier;
    public float stressDownMultiplier;
    public float stressAmount;
    public bool shouldStress;
    bool isStressed;
    [SerializeField] float stressDamageTimer = 4;
    [SerializeField] float stressDamageInterval = 2;
    float internalStressTimer;
    DogConnection dogConnectionState;
    public int weaponIndex;
    public bool hasTakenDamageThisWave;

    public MMFeedbacks shootingFeedback;
    public MMFeedbacks hitFeedback;
    public MMFeedbacks healFeedback;
    Color ColorFromGradient(float value)  // float between 0-1
    {
        return postGradient.Evaluate(value);
    }
    private void Awake()
    {
        Instance = this;
        visuals = gameObject.GetComponent<PlayerVisualsController>();
        rb = GetComponent<Rigidbody2D>();
        Health = startingHealth; // this should come from GameManager > gameData
        if (activeWeapon != null && activeWeapon.GetComponent<Weapon>() != null)
        {
            hasActiveWeapon = true;
        }
        else
        {
            hasActiveWeapon = false;
        }
    }

    void Start()
    {
        ColorAdjustments _colorAdjustments;
        if (globalVolume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
        {
            colorAdjustments = _colorAdjustments;
        }
    }

    public void DataLoaded()
    {
        if (GameManager.Instance.hub == null)
        {
            SetWeapon(DataManager.Instance.publicData.player_weapon);
        }
      
    }

    public void HidePlayer()
    {
        controlsActive = false;
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ShowPlayer()
    {
        foreach(Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
        }

        controlsActive = true;
    }

    IEnumerator DestroyWithDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }

    public void SetWeapon(int index)
    {
        weaponIndex = index;
        switch (weaponIndex)
        {
            case 1:
                SetWeapon(GameManager.Instance.pistolPrefab);
                break;
            case 2:
                SetWeapon(GameManager.Instance.shotgunPrefab);
                break;
            case 3:
                SetWeapon(GameManager.Instance.riflePrefab);
                break;
            case 4:
                SetWeapon(GameManager.Instance.revolverPrefab);
                break;
            case 5:
                SetWeapon(GameManager.Instance.shotgunPlusPrefab);
                break;
            case 6:
                SetWeapon(GameManager.Instance.rifleExtra);
                break;
        }

        if (GameManager.Instance.hub == null)
        {
            UIManager.Instance.UpdateAbilityElements();
        }
    }

    public void AddWeaponDamage(int amount)
    {
        activeWeapon.damage += amount;
    }

    public void AddProjectileSpeed(int amount)
    {
        activeWeapon.GetComponent<ProjectileWeapon>().projectileSpeed += amount;
    }

    public void AddPlayerSpeed(int amount)
    {
        tempSpeed += amount;
    }

    private void SetWeapon(GameObject prefabWeapon)
    {
        Vector3 currentRotation = Vector3.zero;

        if (activeWeapon != null)
        {
            currentRotation = activeWeapon.transform.eulerAngles;
            ProjectileWeapon projectileWeapon = activeWeapon.GetComponent<ProjectileWeapon>();
            if (projectileWeapon != null)
            {
                StartCoroutine(DestroyWithDelay(projectileWeapon.bulletPoolParent, 2f));
            }
        }

        GameObject previousWeapon = gunParent.transform.GetChild(0).gameObject;

        if (previousWeapon != null)
        {
            currentRotation = previousWeapon.transform.eulerAngles;
            Destroy(previousWeapon);
        }

        GameObject newWeapon = Instantiate(prefabWeapon, gunParent.transform.position, Quaternion.Euler(currentRotation));
        Weapon newWeaponComponent = newWeapon.GetComponent<Weapon>();
        newWeapon.transform.parent = gunParent.transform;
        newWeapon.transform.localPosition = Vector3.zero;

        if (newWeaponComponent != null)
        {
            newWeapon.transform.localPosition += newWeaponComponent.offsetFromSource;
            hasActiveWeapon = true;
            activeWeapon = newWeaponComponent;
        }
        else
        {
            Vector3 offset = new Vector3(1.22f, 0, 0);
            newWeapon.transform.localPosition += offset;
            hasActiveWeapon = false;
        }
    }

    public void PostProcessingEffects()
    {
        float percentageOfStress = stressAmount / stressThreshold;
        //colorAdjustments.postExposure.value = (percentageOfStress) * -1;
        colorAdjustments.colorFilter.value = ColorFromGradient(percentageOfStress);


    }

    public void PlayStep()
    {
        AudioManager.Instance.Play("footsteps");

        if (!GameManager.Instance.isHome)
        {
            float randomChance = Random.Range(0, 100);

            if (randomChance <= 10)
            {
                AudioManager.Instance.Play("creaks");
            }
        }
    }
    
    private void Update()
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        if (GameManager.Instance.playerDead || !controlsActive)
        {
            rb.velocity = Vector3.zero;
            isMoving = false;
            visuals.PlayerMoving(false);
            return;
        }
        
        Move();

        if (isMoving)
        {
            RaycastHit2D[] results = Physics2D.CircleCastAll(this.transform.position, interactableRadius, this.transform.up, Mathf.Infinity, interactablesLayer);
            if (results.Length > 0)
            {
                RaycastHit2D selectedHit = results[0];
                if (results.Length > 1)
                {
                    float smallestDistance = Vector3.Distance(transform.position, selectedHit.transform.position);
                    for (int i = 0; i < results.Length - 1; i++)
                    {
                        if (results[i].transform.gameObject.GetComponent<Interactable>() != null)
                        {
                            float newDistance = Vector3.Distance(transform.position, results[i].transform.position);
                            if (newDistance < smallestDistance)
                            {
                                if (results[i].transform.gameObject.GetComponent<Interactable>().isInteractable)
                                {
                                    smallestDistance = newDistance;
                                    selectedHit = results[i];
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                      
                    }
                }

                if (selectedHit.transform.gameObject.GetComponent<Interactable>().isInteractable)
                {
                    currentInteractable = selectedHit.transform.gameObject.GetComponent<Interactable>();
                    UIManager.Instance.ShowOverlay(selectedHit.transform.gameObject, currentInteractable.overlayOffset);
                }
                else
                {
                    UIManager.Instance.HideOverlay();
                }
            }
            else
            {
                UIManager.Instance.HideOverlay();
                currentInteractable = null;
            }
        }

        if (currentInteractable != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteractable.Interact();
            }
        }


        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        aimDir = (mousePos - gunParent.transform.position).normalized;
        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        //Debug.Log(angle);
        euAngle.z = angle;
        gunParent.transform.eulerAngles = euAngle;
        UpdateDirection(angle);

        if (hasActiveWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                activeWeapon.Attack(aimDir);
            }
        }

       
        if (companion != null)
        {
            if (Input.GetKeyDown(KeyCode.F) &&  CompanionController.Instance.pettingPossible)
            {
                visuals.Pet();
                CompanionController.Instance.Pet();
            }

            if (Input.GetKeyDown(KeyCode.Q) && CompanionController.Instance.freezingPossible && CompanionController.Instance.ability_freeze)
            {
                CompanionController.Instance.Freeze();
            }

            if (Input.GetMouseButtonDown(1) && CompanionController.Instance.ability_stay)
            {
                visuals.Stay();
                CompanionController.Instance.StayAtPosition();
            }

            if (Input.GetKeyDown(KeyCode.C) && CompanionController.Instance.ability_stay)
            {
                visuals.Call();
                CompanionController.Instance.CallPet();
            }

            if (petFollow && companion != null)
            {

            }

        }
    }

    void ThrowBall()
    {

    }


    /// <summary>
    /// This Move instantly works with controlelrs and all directional buttons. Plus the run thingy.
    /// </summary>
    void Move()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        bool newMovingState;

        if (moveX != 0 || moveY != 0)
        {
            newMovingState = true;
        }
        else
        {
            newMovingState = false;
        }

        tempPos.x = transform.position.x + moveX * tempSpeed * Time.deltaTime;
        tempPos.y = transform.position.y + moveY * tempSpeed * Time.deltaTime;
        newVel.x = moveX * tempSpeed;
        newVel.y = moveY * tempSpeed;

        rb.velocity = newVel; ;

        if (moveX > 0)
        {
            futureDir = EntityDirection.RIGHT;
        }
        if (moveX < 0)
        {
            futureDir = EntityDirection.LEFT;
        }
        if (moveY > 0)
        {
            futureDir = EntityDirection.UP;
        }
        if (moveY < 0)
        {
            futureDir = EntityDirection.DOWN;
        }

        if (newMovingState != isMoving)
        {
            isMoving = newMovingState;
            visuals.PlayerMoving(isMoving);
        }

        if (futureDir != currentDir)
        {
            currentDir = futureDir;
            visuals.UpdateVisuals(currentDir);
        }
    }

    public void HealMax()
    {
        Health = startingHealth;
        UIManager.Instance.UpdatePlayerHealth(false);
    }

    public void AddMaxHealth(int amount)
    {
        startingHealth += amount;
        UIManager.Instance.UpdatePlayerHealth(true);
    }

    public void Heal(int amount)
    {
        healFeedback?.PlayFeedbacks();
        Health = Mathf.Clamp(Health + amount, 0, startingHealth);
        UIManager.Instance.UpdatePlayerHealth(false);
    }

    public void Damage(int amount)
    {
        if (GameManager.Instance.devMode && GameManager.Instance.infiniteHealth)
        {
            return;
        }

        if (GameManager.Instance.playerDead) 
        {
            return;
        }

        AudioManager.Instance.Play("hit");
        hitFeedback?.PlayFeedbacks();

        hasTakenDamageThisWave = true;
        Health = Mathf.Clamp(Health - amount, 0, startingHealth);

        if (Health <= 0)
        {
            isMoving = false;
            visuals.playerAnimator.SetBool("isWalking", false);
            AudioManager.Instance.Stop("hit");
            AudioManager.Instance.Play("death");
            GameManager.Instance.GameOver();
            return;
        }

        UIManager.Instance.UpdatePlayerHealth(false);

        ChangeEmotionState(EntityStatus.HIT, true);
    }

    public void TestStatusChange()
    {
        float nextStatusIndex = (int)emotionState + 1;

        if (nextStatusIndex >= EntityStatus.GetNames(typeof(EntityStatus)).Length)
        {
            nextStatusIndex = 0;
        }

        emotionState = (EntityStatus)nextStatusIndex;
        visuals.UpdateStatus();
    }

    public void ChangeEmotionState(EntityStatus newStatus, bool revert, float effectTime = 0)
    {
        previousState = EntityStatus.DEFAULT;
        emotionState = newStatus;

        if (revert)
        {
            if (effectTime != 0)
            {
                visuals.UpdateStatus(previousState, hitVisualLenght);
            }
            else
            {
                visuals.UpdateStatus(previousState, effectTime);
            }
           
        }
        else
        {
            visuals.UpdateStatus();
        }
    }

    public void UpdateDirection(float angle)
    {

        if (angle > 90 || angle < -90)
        {
            futureDir = EntityDirection.LEFT;
        }
        else
        {
            futureDir = EntityDirection.RIGHT;
        }

        if (angle > 45 && angle <= 135)
        {
            futureDir = EntityDirection.UP;
        }

        if (angle < -45 && angle >= -135)
        {
            futureDir = EntityDirection.DOWN;
        }

        if (futureDir != currentDir)
        {
            currentDir = futureDir;
            visuals.UpdateVisuals(currentDir);
        }

        if (activeWeapon != null)
        {
            if (angle > 90 || angle < -90)
            {

                activeWeapon.DirectionUpdate(WeaponDirection.LEFT);
            }
            else
            {
                activeWeapon.DirectionUpdate(WeaponDirection.RIGHT);
            }
        }
    }
}

public enum EntityDirection
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public enum DogConnection
{
    damaged,
    normal,
    loving
}