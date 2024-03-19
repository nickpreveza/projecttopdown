using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    [SerializeField] GameObject panel;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image toFadeIn;
    [SerializeField] Image toFadeOut;
    [SerializeField] Sprite sunSprite;
    [SerializeField] Sprite moonSprite;
    Animator anim;
    public bool sceneLoadingInProgress;
    bool sceneLoaded;
    public void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        anim = GetComponent<Animator>();
        panel.SetActive(false);
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

        sceneLoaded = true;
    }

    public void LoadScene(int index, LoadingEffect inEffect = LoadingEffect.FADEIN, LoadingEffect outEffect = LoadingEffect.FADEOUT)
    {
        if (GameManager.Instance.devMode && GameManager.Instance.noSenceChanges)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);

        }
        else
        {
            StartCoroutine(SceneLoading(index, inEffect, outEffect));
           
        }

    }

    IEnumerator SceneLoading(int index, LoadingEffect inEffect, LoadingEffect outEffect)
    {
        if (index == 0)
        {
            toFadeIn.sprite = sunSprite;
            toFadeOut.sprite = moonSprite;
        }
        else
        {
            toFadeIn.sprite = moonSprite;
            toFadeOut.sprite = sunSprite;
        }
        panel.SetActive(true);
        sceneLoadingInProgress = true;
        sceneLoaded = false;
        //GetLoadingEffect(outEffect);
        // 
        GetLoadingEffect(inEffect);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(index);
        while (!sceneLoaded)
        {
            yield return new WaitForSeconds(.1f);
        }
        sceneLoadingInProgress = false;
    }

    public void GetLoadingEffect(LoadingEffect effect)
    {
        switch (effect)
        {
            case LoadingEffect.FADEIN:
                FadeInEffect();
                break;
            case LoadingEffect.FADEOUT:
                FadeOutEffect();
                break;
        }
    }

    public void FadeInEffect()
    {
        anim.SetTrigger("FadeIn");
    }

    public void FadeOutEffect()
    {
        anim.SetTrigger("FadeOut");
    }

   
}

public enum LoadingEffect
{ 
    FADEIN,
    FADEOUT
}
