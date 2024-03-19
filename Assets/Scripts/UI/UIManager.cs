using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public UIPanel pausePanel;
    public UIPanel gamePanel;
    public UIPanel overPanel;
    public UIPanel overlayPanel;
    public UIPanel mainMenuPanel;
    public UIPanel onboardingPanel;
    public UIPanel creditsPanel;
    public PopUpHandler popup;
    List<UIPanel> allPanelsList;
    public bool menuActive;

    [SerializeField] UIPanel unlockPanel;
    public ProgressionUI progression;
    public bool unlockInProgress;

    public bool creditsEnabled;
    public bool onboardingEnabled;

    [SerializeField] GameObject item_ball;
    [SerializeField] GameObject item_petHeal;
    [SerializeField] GameObject item_moveStay;
    [SerializeField] GameObject item_ballAttack;
    [SerializeField] GameObject item_petHeal2;
    [SerializeField] GameObject item_freeze;
    [SerializeField] GameObject item_petDamage;
    [SerializeField] GameObject item_petFreeze;

    [SerializeField] GameObject item_ball_unlock;
    [SerializeField] GameObject item_petHeal_unlock;
    [SerializeField] GameObject item_moveStay_unlock;
    [SerializeField] GameObject item_ballAttack_unlock;
    [SerializeField] GameObject item_petHeal2_unlock;
    [SerializeField] GameObject item_freeze_unlock;
    [SerializeField] GameObject item_petDamage_unlock;
    [SerializeField] GameObject item_petFreeze_unlock;

    public Color callColor;
    public Color targetColor;
    public Color locationColor;

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

    public GameObject GetProgressionItem(int levelIndex, bool isUnlock)
    {
        DogAbilities currentAbilityToUnlock = GameManager.Instance.dogAbilitiesUnlockOrder[levelIndex];
        switch (currentAbilityToUnlock)
        {
            case DogAbilities.BALL:
                if (isUnlock)
                {
                    return item_ball_unlock;
                }
                return item_ball;
            case DogAbilities.PETHEAL:
                if (isUnlock)
                {
                    return item_petHeal_unlock;
                }
                return item_petHeal;
            case DogAbilities.STAY:
                if (isUnlock)
                {
                    return item_moveStay_unlock;
                }
                return item_moveStay;
            case DogAbilities.ATTACK:
                if (isUnlock)
                {
                    return item_ballAttack_unlock;
                }
                return item_ballAttack;
            case DogAbilities.PETHEAL2:
                if (isUnlock)
                {
                    return item_petHeal2_unlock;
                }
                return item_petHeal2;
            case DogAbilities.FREEZE:
                if (isUnlock)
                {
                    return item_freeze_unlock;
                }
                return item_freeze;
            case DogAbilities.PETDAMAGE:
                if (isUnlock)
                {
                    return item_petDamage_unlock;
                }
                return item_petDamage;
            case DogAbilities.PETFREEZE:
                if (isUnlock)
                {
                    return item_petFreeze_unlock;
                }
                return item_petFreeze;
        }

        return null;
    }


    public void UnlockRelationshipLevel()
    {
        pausePanel.Disable();
        gamePanel.Disable();
        unlockPanel.Activate();
        unlockPanel.GetComponent<UnlockPanel>().UnlockRelationshipLevel();
    }

    public void UnlockPhoto()
    {
        PlayerController.Instance.isMoving = false;
        pausePanel.Disable();
        gamePanel.Disable();
        unlockPanel.Activate();
        unlockPanel.GetComponent<UnlockPanel>().UnlockPhoto();
        pausePanel.GetComponent<PausePanel>().ShowNotification();
        GameManager.Instance.hub.PictureSeen = true;
    }

    public void RollCredits()
    {
        creditsPanel.Activate();
        creditsEnabled = true;
    }

    /// <summary>
    /// Panels use this method to subscribe to the list of UIManager.
    /// </summary>
    /// <param name="newPanel"></param>
    public void AddPanel(UIPanel newPanel)
    {
        if (allPanelsList == null)
        {
            allPanelsList = new List<UIPanel>();
        }

        if (!allPanelsList.Contains(newPanel))
        {
            allPanelsList.Add(newPanel);
        }
    }

    public void UpdateAbilityElements()
    {
        gamePanel.GetComponent<GamePanel>().UpdateAbilityElements(); 
    }

    public void OpenGamePanel()
    {
        if (unlockPanel != null)
        {
            unlockPanel.Disable();
        }
      
        onboardingPanel.Disable();
        mainMenuPanel.Disable();
        pausePanel.Disable();
     
        gamePanel.GetComponent<GamePanel>().Setup();
        gamePanel.Activate();
       
        menuActive = false;
    }

    public void OpenMainMenu()
    {
        menuActive = true;
        onboardingPanel.Disable();
        gamePanel.Disable();
        pausePanel.Disable();
        mainMenuPanel.Setup();
        mainMenuPanel.Activate();
    }

    public void OpenOnboarding()
    {
        if (!onboardingEnabled)
        {
            onboardingPanel.GetComponent<OnboardingPanel>().Close();
            return;
        }
        mainMenuPanel.Disable();
        onboardingPanel.Activate();
    }

    public void GameOver()
    {
        gamePanel.Disable();
        pausePanel.Disable();
        overPanel.GetComponent<GameOverPanel>().Setup();
        overPanel.Activate();
    }

    public void UpdatePlayerHealth(bool maxHealthChanged)
    {
        if (maxHealthChanged)
        {
            gamePanel.GetComponent<GamePanel>().MaxHealthChanged();
        }
        else
        {
            gamePanel.GetComponent<GamePanel>().UpdateHealth();
        }
       
    }

    public void ShowOverlay(GameObject target, float offset)
    {
        overlayPanel.Activate();
        overlayPanel.GetComponent<OverlayPanel>().EnableOverlayPrompt(target, offset);
    }

    public void HideOverlay() 
    {
        overlayPanel.GetComponent<OverlayPanel>().DisablePrompt(); 
    }

    public void ShowPetLocation()
    {
        overlayPanel.Activate();
        overlayPanel.GetComponent<OverlayPanel>().EnablePetLocation();
    }

    public void ShowPetTarget()
    {
        overlayPanel.Activate();
        overlayPanel.GetComponent<OverlayPanel>().EnableCompanionTarget();
    }

    public void HidePetTarget()
    {
        overlayPanel.GetComponent<OverlayPanel>().DisableCompanionTarget();
    }

    public void HidePetLocation()
    {
        overlayPanel.GetComponent<OverlayPanel>().DisablePetLocation();
    }

    public void ShowCallPrompt()
    {
        UIManager.Instance.HidePetLocation();
        overlayPanel.Activate();
        overlayPanel.GetComponent<OverlayPanel>().EnableCallOverlay();
    }

    public void HideCallPrompt()
    {
        overlayPanel.GetComponent<OverlayPanel>().DisableCallOverlay();
    }


    public void ShowPetPrompt()
    {
        overlayPanel.Activate();
        overlayPanel.GetComponent<OverlayPanel>().EnablePetOverlay();
    }

    public void HidePetPrompt()
    {
        overlayPanel.GetComponent<OverlayPanel>().DisablePetOverlay();
    }
    
    /// <summary>
    /// Used to close all the panels registered to the list
    /// </summary>
    public void CloseAllPanels()
    {
        if (allPanelsList == null)
        {
            Debug.LogWarning("CloseAllPanels failed. No panels registered");
            return;
        }
        foreach(UIPanel panel in allPanelsList)
        {
            panel.Disable();
        }
    }

    /// <summary>
    /// Called from GameManager when the Paused state is changed. Could be using an event here
    /// </summary>
    public void PauseChanged()
    {
        if (GameManager.Instance.isPaused)
        {
            gamePanel.canvasGroup.alpha = 0;
            pausePanel.Activate();
        }
        else
        {
            gamePanel.canvasGroup.alpha = 1;
            pausePanel.Disable();
        }
    }
}
