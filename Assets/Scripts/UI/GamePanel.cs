using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GamePanel : UIPanel
{
    [SerializeField] GameObject healthHolder;
    [SerializeField] GameObject abilityHolder;
    [SerializeField] GameObject anxietyHolder;
    [SerializeField] RectTransform dogGraphic;
    [SerializeField] GameObject healthPrefab;
    public List<GameObject> healthVessel;
    public List<GameObject> halfHearts;

    public TextMeshProUGUI devText;
    public TextMeshProUGUI waveCounter;
    public TextMeshProUGUI waveHeader;

    public Image characterExpression;
    public Image waveGraphic;
    public Image fillMeter;
    public Image fakeFill;
    public Sprite normal;
    public Sprite happy;
    public Sprite anxious;

    public Color normalColor;
    public Color waveColor;
    [Range(0,1)]
    [SerializeField] float fillAmount;
    public Gradient gradient;

    [Header("Tutorial Scene")]
    public bool hasDoneSetUp;

    [Header("Abilties")]
    [SerializeField] Sprite pistolSprite;
    [SerializeField] Sprite rifleSprite;
    [SerializeField] Sprite shotgunSprite;
    [SerializeField] Sprite revolverSprite;
    [SerializeField] Sprite shotgunPlusSprite;
    [SerializeField] Image petAbility;
    [SerializeField] Image petFilled;
    [SerializeField] Image weapon;
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;
    [SerializeField] Image freezingAbility;
    [SerializeField] Image freezingFilled;


    [Header("Abilties")]
    [SerializeField] GameObject freezeAbility;
    [SerializeField] GameObject ballAbility;
    [SerializeField] GameObject stayAbility;
    [SerializeField] GameObject callAbility;

    Color ColorFromGradient(float value)  // float between 0-1
    {
        return gradient.Evaluate(value);
    }

    public float SetFillAmount
    {
        get
        {
            return fillAmount;
        }
        set
        {
            if (value < 0.2)
            {
                characterExpression.sprite = happy;
            }
            else if (value >= 0.2 && value < 0.7)
            {
                characterExpression.sprite = normal;
            }
            else if(value >= 0.7)
            {
                characterExpression.sprite = anxious;
            }
            fillMeter.color = ColorFromGradient(fillAmount);
            fakeFill.color = fillMeter.color;
            fillAmount = Mathf.Clamp(value, 0, 1);
        }

        

    }
    void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.gamePanel = this;
            UIManager.Instance.AddPanel(this);
        }

        SetDefaultAbilities();
    }

    public void SetDefaultAbilities()
    {
        petAbility.color = activeColor;
        freezeAbility.SetActive(false);
        stayAbility.SetActive(false);
        callAbility.SetActive(false);
        ballAbility.SetActive(false);
    }

    public void UpdateAbilityElements()
    {
        if (CompanionController.Instance == null)
        {
            freezeAbility.SetActive(false);
            stayAbility.SetActive(false);
            callAbility.SetActive(false);
            ballAbility.SetActive(false);
        }
        else
        {
            if (CompanionController.Instance.pettingPossible)
            {
                petAbility.color = activeColor;
            }
            else
            {
                petAbility.color = inactiveColor;
            }


            if (CompanionController.Instance.ability_freeze)
            {
                freezeAbility.SetActive(true);

            }
            else
            {
                freezeAbility.SetActive(false);
            }

            if (CompanionController.Instance.ability_stay)
            {
                stayAbility.SetActive(true);
                callAbility.SetActive(true);

            }
            else
            {
                stayAbility.SetActive(false);
                callAbility.SetActive(false);
            }

            if (CompanionController.Instance.ability_ballFollow)
            {
                ballAbility.SetActive(true);
            }
            else
            {
                ballAbility.SetActive(false);
            }

            if (CompanionController.Instance.freezingPossible)
            {
                freezingAbility.color = activeColor;
                if (CompanionController.Instance.ability_attack)
                {
                    freezingAbility.color = UIManager.Instance.targetColor;
                }
            }
            else
            {
                freezingAbility.color = inactiveColor;
            }

            if (CompanionController.Instance.ability_attack)
            {
                freezingFilled.color = UIManager.Instance.targetColor;
            }

            if (GameManager.Instance.hub == null)
            {
                weapon.sprite = GetWeaponSpriteWithIndex(PlayerController.Instance.weaponIndex);
            }
        }
     

       
    }

    public Sprite GetWeaponSpriteWithIndex(int index)
    {
        switch (index)
        {
            case 1:
                return pistolSprite;
            case 2:
                return shotgunSprite;
            case 3:
                return rifleSprite;
            case 4:
                return revolverSprite;
            case 5:
                return shotgunPlusSprite;
        }

        Debug.LogWarning("GetWeaponSpriteWithIndex returned unhandled case default");
        return pistolSprite;
    }

    public void HomeSetup()
    {
        waveCounter.text = DataManager.Instance.publicData.day.ToString();
        waveHeader.text = "DAY";
        healthHolder.SetActive(false);
        UIManager.Instance.UpdateAbilityElements();
    }

    private void Update()
    {
        
        if (CompanionController.Instance != null)
        {
            if (CompanionController.Instance.pettingPossible)
            {
                petFilled.fillAmount = 0;
            }
            else
            {
                petFilled.fillAmount = CompanionController.Instance.pettingPercentage;
            }

            if (CompanionController.Instance.freezingPossible)
            {
                freezingFilled.fillAmount = 0;
            }
            else
            {
                freezingFilled.fillAmount = CompanionController.Instance.freezingPercentage;
            }

            fillMeter.fillAmount = fillAmount;
        }

        if (GameManager.Instance.isHome)
        {
            return;
        }

        SetFillAmount = PlayerController.Instance.stressAmount / PlayerController.Instance.stressThreshold;

        dogGraphic.localPosition = new Vector3(fillMeter.GetComponent<Image>().fillAmount * 100 - 50, 0, 0);

        if (GameManager.Instance.devMode)
        {
            devText.text = "Health: " + PlayerController.Instance.Health;
            devText.text += "\n DevMode Enabled";
            devText.text += "enemies remaining: " + GameManager.Instance.spawner.enemiesRemaining;
            if (GameManager.Instance.infiniteHealth)
            {
                devText.text += "\n infiniteHealth = True";
            }
            else
            {
                devText.text += "\n infiniteHealth = False";
            }

            if (GameManager.Instance.infinitePetHeal)
            {
                devText.text += "\n infinitePetHeal = True";
            }
            else
            {
                devText.text += "\n infinitePetHeal = False";
            }
        }
        else
        {
            devText.text = "";
        }

        if (GameManager.Instance.spawner != null)
        {
            switch (GameManager.Instance.spawner.state)
            {
                case WaveHandler.SpawnState.INACTIVE:
                    waveHeader.text = "WAVE";
                    waveCounter.text = (GameManager.Instance.spawner.waveIndex + 1).ToString() + "/" + (GameManager.Instance.spawner.totalWavesInDay).ToString();
                    waveGraphic.color = normalColor;
                    break;
                case WaveHandler.SpawnState.COUNTING:
                    waveHeader.text = "NEXT:";
                    waveCounter.text = ((int)GameManager.Instance.spawner.waveCountdown).ToString();
                    waveGraphic.color = waveColor;
                    break;
                case WaveHandler.SpawnState.ACTIVE:
                    waveHeader.text = "WAVE";
                    waveCounter.text = (GameManager.Instance.spawner.waveIndex + 1).ToString() + "/" + (GameManager.Instance.spawner.totalWavesInDay).ToString();
                    waveGraphic.color = normalColor;
                    break;
                case WaveHandler.SpawnState.SPAWNING:
                    waveHeader.text = "WAVE";
                    waveCounter.text = (GameManager.Instance.spawner.waveIndex + 1).ToString() + "/" + (GameManager.Instance.spawner.totalWavesInDay).ToString();
                    waveGraphic.color = normalColor;
                    break;
            }
          

        }
        else
        {
            waveHeader.text = "";
            waveCounter.text = "";
        }
       
    }

    public override void Setup()
    {
        base.Setup();

        for (int i = 1; i < healthHolder.transform.childCount; i++)
        {
            Destroy(healthHolder.transform.GetChild(i).gameObject);
        }

        healthVessel = new List<GameObject>();
        halfHearts = new List<GameObject>();

        for (int i = 0; i < (int)(PlayerController.Instance.startingHealth / 2); i++)
        {
            GameObject obj = Instantiate(healthPrefab, healthHolder.transform);
            healthVessel.Add(obj);
            halfHearts.Add(obj.transform.GetChild(0).gameObject);
            halfHearts.Add(obj.transform.GetChild(1).gameObject);
        }

        UpdateHealth();
    }

    public void MaxHealthChanged()
    {
        Setup();
    }

    public void UpdateHealth()
    {
       foreach(GameObject obj in halfHearts)
       {
            obj.SetActive(false);
       }

        for (int i = 0; i < PlayerController.Instance.Health; i++)
        {
            halfHearts[i].SetActive(true);
        }
    }

    public override void Activate()
    {
        base.Activate();

    }

    public override void Disable()
    {
        base.Disable();
    }
}
