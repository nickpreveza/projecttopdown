using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float overlayOffset;
    public bool isInteractable = true;
    public virtual void Interact()
    {
        UIManager.Instance.overlayPanel.GetComponent<OverlayPanel>().DisablePrompt();
    }
}
