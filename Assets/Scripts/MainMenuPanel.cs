using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanel : UIPanel
{
    [SerializeField] GameObject continueButton;

    private void Awake()
    {
      
    }

    public override void Setup()
    {
        if (DataManager.Instance.canLoad)
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }
    public void Continue()
    {
        DataManager.Instance.Load();
        UIManager.Instance.OpenGamePanel();
        GameManager.Instance.hub.bed.WakeUp();
    }

    public void NewGame()
    {
        DataManager.Instance.Save();
        UIManager.Instance.OpenOnboarding();
    }
}
