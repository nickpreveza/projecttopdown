using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using MoreMountains.Feel;
using MoreMountains.Feedbacks;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool devMode;
    public bool isPaused;
    public bool playerDead = false;
    public bool isHome;

    [Header("Abilities")]
    public DogAbilities[] dogAbilitiesUnlockOrder;

    [Header("Cheats")]
    public bool infinitePetHeal;
    public bool infiniteHealth;
    public bool noSenceChanges;
    public bool noWaveSpawn;
    public bool noSave;
    public bool noLoad;
    public bool overrideWaveDay;
    public int ovr_sequence;
    public int ovr_wave;

    [Header("Weapons")]
    public GameObject shotgunPrefab;
    public GameObject pistolPrefab;
    public GameObject riflePrefab;
    public GameObject shotgunPlusPrefab;
    public GameObject revolverPrefab;
    public GameObject rifleExtra;
    public GameObject handPrefab;

    [Header("Visual Prefabs")]
    public GameObject companionClone;
    public GameObject playerClone;


    [Header("System Prefabs")]
    public GameObject companionPrefab;
    public GameObject playerPrefab;
    public GameObject photoPrefab;
    [Header("Enemy Prefabs")]
    public GameObject angerEnemyPrefab;
    public GameObject anxietyEnemyPrefab;
    public GameObject lonelinessEnemyPrefab;
    public GameObject fearEnemyPrefab;
    public GameObject resentmessEnemyPrefab;
    public GameObject hopelessnessEnemyPrefab;

    [Header("SubSystems")]
    public HubSystems hub;
    public WaveHandler spawner;

    [Header("PostProcess")]
    public Volume globalVolume;

    public Material targetMaterial;
    public Material frozenMaterial;
    public Material normalMaterial;

    [SerializeField] Sprite normalProjectileSprite;
    [SerializeField] Sprite extraDamageProjectileSprite;
    [SerializeField] Sprite extraSpeedProjectileSprite;

    [Header("ParticleEffects")]
    public MMFeedbacks anxietyFeedback;
    public bool SetPause
    {
        get
        {
            return isPaused;
        }
        set
        {
            isPaused = value;
            if (isPaused)
            {
             
                Time.timeScale = 0;
            }
            else
            {
                
                Time.timeScale = 1;
            }
            UIManager.Instance.PauseChanged();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void Start()
    {
       if (hub != null)
        {
            AudioManager.Instance.Stop("ambient");
            AudioManager.Instance.Play("ambientHub");

            AudioManager.Instance.Play("themeHub");
            AudioManager.Instance.Stop("themeGame");
        }
        else
        {
            AudioManager.Instance.Play("ambient");
            AudioManager.Instance.Stop("ambientHub");

            AudioManager.Instance.Stop("themeHub");
            AudioManager.Instance.Play("themeGame");
        }
    }

    public void DataLoaded()
    {
        if (hub == null)
        {
           
        }
        else
        {
            if (DataManager.Instance.publicData.home_hasOpenedBox == 0)
            {
                hub.box.gameObject.SetActive(true);
                hub.dummyEntity.SetActive(false);
            }
            else
            {
                hub.box.gameObject.SetActive(false);
                hub.dummyEntity.SetActive(true);
                if (hub.companion == null)
                {
                    hub.companion = Instantiate(companionPrefab, hub.box.gameObject.transform.position, Quaternion.identity);
                    hub.companion.transform.parent = null;
                    hub.companion.transform.position = hub.box.gameObject.transform.position;
                }
            }

            if (DataManager.Instance.publicData.showPhoto == 1)
            {
                GameObject obj = Instantiate(photoPrefab, hub.photoSpawn);

            }
            else
            {
                hub.PictureSeen = true;
            }

            if (DataManager.Instance.publicData.relationshipLevel >= DataManager.Instance.publicData.day)
            {
                hub.DogPet = true;
            }

            if (DataManager.Instance.publicData.day == 9)
            {
                hub.bed.isInteractable = true;
            }

            UIManager.Instance.gamePanel.GetComponent<GamePanel>().HomeSetup();

        }

    }

  

    public void StartRun()
    {
        if (DataManager.Instance.publicData.day == 9)
        {
            hub.bed.RollCredits();
            return;
        }
       LevelLoader.Instance.LoadScene(1);
    }

    public void GoHome()
    {
        Time.timeScale = 1;
        LevelLoader.Instance.LoadScene(0);
    }

    public void SequenceCompleted()
    {

        DataManager.Instance.AddDay();
     

        if (GameManager.Instance.devMode && GameManager.Instance.noSenceChanges)
        {
            if (GameManager.Instance.overrideWaveDay)
            {
                GameManager.Instance.ovr_sequence++;
            }
            Time.timeScale = 1;
            LevelLoader.Instance.LoadScene(1);
            
        }
        GoHome();
    }

    public void CheatRefillHealth()
    {
        PlayerController.Instance.HealMax();
    }

    public void SpawnPlayer(Transform spawnPoint)
    {
        PlayerController.Instance.gameObject.transform.position = spawnPoint.transform.position;
        PlayerController.Instance.ShowPlayer();
        PlayerController.Instance.controlsActive = true;
    }

    void DevUpdate()
    {
        if (Input.GetKey(KeyCode.Alpha0))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                infiniteHealth = !infiniteHealth;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CheatRefillHealth();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                infinitePetHeal = true;
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayerController.Instance.SetWeapon(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayerController.Instance.SetWeapon(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayerController.Instance.SetWeapon(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                PlayerController.Instance.SetWeapon(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                PlayerController.Instance.SetWeapon(5);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)))
        {
            if (!UIManager.Instance.menuActive && !UIManager.Instance.unlockInProgress && !LevelLoader.Instance.sceneLoadingInProgress)
            {
                if (!playerDead)
                {
                    isPaused = !isPaused;
                    if (isPaused)
                    {
                        Time.timeScale = 0;
                    }
                    else
                    {
                        Time.timeScale = 1;
                    }
                    UIManager.Instance.PauseChanged();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.BackQuote) && Debug.isDebugBuild)
        {
            devMode = !devMode;
        }

        if (devMode && Debug.isDebugBuild)
        {
            DevUpdate();
        }

        if (UIManager.Instance.creditsEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Escape) ||Input.GetMouseButtonDown(0))
            {
                GoHome();
            }
        }


    }

    public void GameOver()
    {
        playerDead = true;
        UIManager.Instance.GameOver();
    }
}
