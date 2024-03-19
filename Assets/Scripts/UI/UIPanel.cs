using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public bool isActive;
    public GameObject panelObject;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        panelObject = transform.GetChild(0).gameObject;
        canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        Disable();
    }
    public virtual void Activate()
    {
        isActive = true;
        panelObject.SetActive(true);
    }

    public virtual void Setup()
    {

    }

    public virtual void Disable()
    {
        isActive = false;
        panelObject.SetActive(false);
    }
}
