using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    [SerializeField] GameData data;
    bool menuPassed;
    public bool canLoad;
    bool isHome;
    public bool skipMenu;
    bool shouldLoad = false;
    bool setUpInProgress;

    public GameData publicData
    {
        get
        {
            return data;
        }
        private set
        {

        }

    }
   
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        if (!menuPassed && !skipMenu)
        {
            SetUpPersistentData(false);
        }
        else
        {
            SetUpPersistentData(true);
        }      


        Time.timeScale = 1;
    }

    void SetUpPersistentData(bool instantLoad)
    {
        if (PlayerPrefs.HasKey("day"))
        {
            
            if (instantLoad)
            {
                Load();
            }
            else
            {
                if (PlayerPrefs.GetInt("day") >= 1 && PlayerPrefs.GetInt("day") <= 9)
                {
                    canLoad = true;
                }
                else
                {
                    PlayerPrefs.SetInt("day", 9);
                    canLoad = true;
                }
                
               
            }
         
        }
        else
        {
            CreatePrefs();
        }

        StartGame();

    }

    void StartGame()
    {
        if (GameManager.Instance.hub != null)
        {
            isHome = true;
        }
        else
        {
            isHome = false;
        }

        if (isHome)
        {
            PlayerController.Instance.HidePlayer();

            if (!menuPassed)
            {
                UIManager.Instance.OpenMainMenu();
                menuPassed = true;
            }
            else
            {
                UIManager.Instance.OpenGamePanel();
                GameManager.Instance.hub.bed.WakeUp();
                
            }
         
        }
        else
        {
            if (GameManager.Instance.devMode && GameManager.Instance.noWaveSpawn)
            {
                return;
            }

            GameManager.Instance.spawner.SetWave(0);
            UIManager.Instance.OpenGamePanel();

        }

        Cursor.visible = true;
     
        GameManager.Instance.SetPause = false;
    }

    public void ReachedCredits()
    {
        data.gameFinished = 1;
        Save();
    }
    public void OpenBox()
    {
       data.home_hasOpenedBox = 1;
       Save();
    }

    public void RemoveNotification()
    {
        data.hasNotification = 0;
        Save();
    }

    public void PictureReceived()
    {
        data.showPhoto = 0;
        Save();
    }

    public void AddDay()
    {
        data.day++;
        if(data.day < 9)
        {
            data.showPhoto = 1;
        }
        else
        {
            data.showPhoto = 0;
        }
        data.player_maxHealth = PlayerController.Instance.startingHealth;
        data.player_weapon = PlayerController.Instance.weaponIndex;

        Save();
    }

    public void AddRelationshipLevel()
    {
        int newLevel = Mathf.Clamp(data.relationshipLevel + 1, 0, 8);
        if (newLevel > data.day)
        {
           return;
        }
        if (newLevel > data.relationshipLevel)
        {
            AudioManager.Instance.Play("levelUnlock");
            data.relationshipLevel = newLevel;
            UIManager.Instance.UnlockRelationshipLevel();
        }
     
        Save();
    }

    public void AddPhoto()
    {

        int newPhoto = Mathf.Clamp(data.photos + 1, 0, 8);
        data.hasNotification = 1;
        data.showPhoto = 0;
        if (newPhoto > data.photos)
        {
            AudioManager.Instance.Play("photoUnlock");
            data.photos = newPhoto;
            UIManager.Instance.UnlockPhoto();
        }

        Save();
    }

    void CreatePrefs()
    {
        data.day = 1;
        data.photos = 0;
        data.player_weapon = 1;
        data.player_maxHealth = 6;
        data.home_hasOpenedBox = 0;
        data.relationshipLevel = 0;
        data.hasNotification = 0;
        data.showPhoto = 1;
        data.gameFinished = 0;

        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("day", data.day);
        PlayerPrefs.SetInt("photos", data.photos);
        PlayerPrefs.SetInt("player_weapon", data.player_weapon);
        PlayerPrefs.SetInt("player_maxHealth", data.player_maxHealth);
        PlayerPrefs.SetInt("home_box", data.home_hasOpenedBox);
        PlayerPrefs.SetInt("relationship_level", data.relationshipLevel);
        PlayerPrefs.SetInt("phone_notification", data.hasNotification);
        PlayerPrefs.SetInt("show_photo", data.showPhoto);
        PlayerPrefs.SetInt("game_finished", data.gameFinished);

        PlayerPrefs.Save();
    }

    public void Load()
    {
        data.day = PlayerPrefs.GetInt("day", 1);
        data.photos = PlayerPrefs.GetInt("photos", 0);
        data.player_weapon = PlayerPrefs.GetInt("player_weapon", 1);
        data.player_maxHealth = PlayerPrefs.GetInt("player_maxHealth", 6);
        data.home_hasOpenedBox = PlayerPrefs.GetInt("home_box", 0);
        data.relationshipLevel = PlayerPrefs.GetInt("relationship_level", 0);
        data.hasNotification = PlayerPrefs.GetInt("phone_notification", 0);
        data.showPhoto = PlayerPrefs.GetInt("show_photo", 1);
        data.gameFinished = PlayerPrefs.GetInt("game_finished", 0);

        PlayerController.Instance.DataLoaded();
        GameManager.Instance.DataLoaded();
    }
}
