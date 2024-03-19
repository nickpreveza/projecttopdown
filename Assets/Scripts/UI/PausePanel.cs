using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

public class PausePanel : UIPanel
{
    [SerializeField] Volume globalVolume;
    [SerializeField] GameObject phonePause;
    [SerializeField] GameObject phoneHome;

    [SerializeField] GameObject galleryPanel;
    [SerializeField] GameObject imageViewPanel;
    [SerializeField] GameObject contactsPanel;
    [SerializeField] GameObject helpPanel; //this became settings last minute

    [SerializeField] GameObject phoneNotification;
    [SerializeField] GameObject galleryNotification;

    [Header("Gallery Only")]
    [SerializeField] Image imageViewHolder;
    [SerializeField] Image[] galleryImages;
    [SerializeField] TextMeshProUGUI galleryText;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider audioSlider;
    [SerializeField] Slider musicSlider;


    private void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.pausePanel = this;
            UIManager.Instance.AddPanel(this);
        }

        globalVolume = GameManager.Instance.globalVolume;
    }
    public override void Activate()
    {
        DepthOfField dof;
        if (globalVolume.profile.TryGet<DepthOfField>(out dof))
        {
            dof.active = true;
        }
        CloseAllPhonePanels();
        if (GameManager.Instance.isHome)
        {
            phoneHome.SetActive(true);
            if (GameManager.Instance.hub != null)
            {
                if (DataManager.Instance.publicData.hasNotification == 1)
                {
                    ShowNotification();
                }
                else
                {
                    HideNotification();
                }
            }
        }
        else
        {
            phonePause.SetActive(true);
            helpPanel.SetActive(false);
        }
       
      
        base.Activate();
    }

    void UpdateSettings()
    {
        if (AudioManager.Instance != null)
        {
            float masterVolume;
            AudioManager.Instance.masterMixer.GetFloat("masterVolume", out masterVolume);
            masterSlider.value = masterVolume;

            float audioVolume;
            AudioManager.Instance.masterMixer.GetFloat("audioVolume", out audioVolume);
            audioSlider.value = audioVolume;

            float musicVolume;
            AudioManager.Instance.masterMixer.GetFloat("musicVolume", out musicVolume);
            musicSlider.value = musicVolume;
        }
      
    }

    void CloseAllPhonePanels()
    {
        phonePause.SetActive(false);
        phoneHome.SetActive(false);
        CloseAllSubPanels();
    }

    public void HideNotification()
    {
        galleryNotification.SetActive(false);
        phoneNotification.SetActive(false);
    }

    public void ShowNotification()
    {
        galleryNotification.SetActive(true);
        phoneNotification.SetActive(true);
    }

    public void OpenPhone()
    {
        phoneNotification.SetActive(false);
        GameManager.Instance.SetPause = true;
    }

    void CloseAllSubPanels()
    {
        
        if (!GameManager.Instance.isHome)
        {
            helpPanel.SetActive(false);
            return;
        }
        galleryPanel.SetActive(false);
        contactsPanel.SetActive(false);
        helpPanel.SetActive(false);
        imageViewPanel.SetActive(false);
    }

    public override void Disable()
    {
        DepthOfField dof;
        if (globalVolume.profile.TryGet<DepthOfField>(out dof))
        {
            dof.active = false;
        }
        base.Disable();
    }

    public void Resume()
    {
        GameManager.Instance.SetPause = false;
    }

    public void Home()
    {
        GameManager.Instance.GoHome();
    }

    public void Exit()
    {
        //UIManager.Instance.popup.Exit();
    }

    public void OpenItchPage()
    {
        Application.OpenURL("https://nickpreveza.itch.io/dogdays/rate");
    }


    public void OpenContacts()
    {
        CloseAllSubPanels();
        contactsPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        CloseAllSubPanels();
    }

    public void OpenGallery()
    {
        CloseAllSubPanels();
        DataManager.Instance.RemoveNotification();
        UpdateGallery();
        HideNotification();
        galleryPanel.SetActive(true);
    }

    void UpdateGallery()
    {
        galleryText.text = DataManager.Instance.publicData.photos + " / 8  \n collected";

        foreach (Image img in galleryImages)
        {
            img.gameObject.GetComponent<Button>().interactable = false;
        }

        for(int i = 0; i < DataManager.Instance.publicData.photos; i++)
        {
            galleryImages[i].color = Color.white;
            galleryImages[i].gameObject.GetComponent<Button>().interactable = true;
            galleryImages[i].sprite = GameManager.Instance.hub.GetImageWithIndex(i);
        }
    }

    public void OpenImageView(int index)
    {
        CloseAllSubPanels();
        imageViewPanel.SetActive(true);
        imageViewHolder.sprite = GameManager.Instance.hub.GetImageWithIndex(index);
    }

    public void OpenExit()
    {
        CloseAllSubPanels();
    }

    public void OpenHelp()
    {
        CloseAllSubPanels();
        helpPanel.SetActive(true);
        UpdateSettings();
    }

    public void OpenHome()
    {
        CloseAllSubPanels();
    }

    public void CreditsLink(int index)
    {
        switch (index)
        {
            case 1:
                Application.OpenURL("https://nickpreveza.itch.io/");
                break;
            case 2:
                Application.OpenURL("https://farukosm.itch.io/");
                break;
            case 3:
                Application.OpenURL("https://marcushagg.itch.io/");
                break;
            case 4:
                Application.OpenURL("https://fabures.itch.io/"); 
                break;
            case 5:
                Application.OpenURL("https://nickpreveza.itch.io/dogdays");
           
                break;
        }
    }

    public void SetMasterVolume(float volume)
    {
        AudioManager.Instance.masterMixer.SetFloat("masterVolume", volume);
    }

    public void SetAudiovolume(float volume)
    {
        AudioManager.Instance.masterMixer.SetFloat("audioVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.masterMixer.SetFloat("musicVolume", volume);
    }
}
