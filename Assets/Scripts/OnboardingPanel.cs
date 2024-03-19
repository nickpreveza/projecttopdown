using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingPanel : UIPanel
{
    public void Close()
    {
        UIManager.Instance.OpenGamePanel();
        GameManager.Instance.DataLoaded();
        GameManager.Instance.hub.bed.WakeUp();
    }
}
