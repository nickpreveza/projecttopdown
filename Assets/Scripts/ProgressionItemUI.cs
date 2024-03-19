using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ProgressionItemUI : MonoBehaviour
{
    [SerializeField] Image lockImage;
    [SerializeField] Image contentImage;
    [SerializeField] Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isUnlocked", true);
    }

    public void Lock()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        anim.SetBool("isUnlocked", false);
    }

    public void Unlock()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        anim.SetBool("isUnlocked", true);
    }
}
