using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteractable : Interactable
{
    Animator anim;
    [SerializeField] GameObject interactPoint;
    [SerializeField] GameObject playerClone;
    [SerializeField] Transform petLocationTarget;
    [SerializeField] GameObject companionClone;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        isInteractable = false;
        companionClone.SetActive(false);
    }

    public override void Interact()
    {
        if (!isInteractable)
        {
            return;
        }
        base.Interact();
        UIManager.Instance.HideOverlay();
        StartCoroutine(InteractCoroutine());


    }

    IEnumerator CreditsCoroutine()
    {
        PlayerController.Instance.HidePlayer();
        DataManager.Instance.ReachedCredits();
        DataManager.Instance.Save();
        anim.SetTrigger("Interact");
        isInteractable = false;
        CompanionController.Instance.OverrideAndStay(petLocationTarget.position);
        yield return new WaitForSeconds(1f);
        while (!CompanionController.Instance.isSitting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        CompanionController.Instance.gameObject.SetActive(false);

        anim.SetTrigger("Credits");
        yield return new WaitForSeconds(2f);
        UIManager.Instance.RollCredits();  
    }

    IEnumerator InteractCoroutine()
    {
        PlayerController.Instance.HidePlayer();
        AudioManager.Instance.Play("bed");
        anim.SetTrigger("Interact");
        isInteractable = false;
        yield return new WaitForSeconds(1f);
        GameManager.Instance.StartRun();
    }

    public void RollCredits()
    {
        StartCoroutine(CreditsCoroutine());
    }

    public void WakeUp()
    {
        StartCoroutine(WakeUpCoroutine());
    }

    IEnumerator WakeUpCoroutine()
    {
        AudioManager.Instance.Play("bed");
        yield return new WaitForSeconds(1f);

        anim.SetTrigger("Wake");
        yield return new WaitForSeconds(1f);
        GameManager.Instance.SpawnPlayer(interactPoint.transform);
        PlayerController.Instance.controlsActive = false;
        anim.SetTrigger("Hide");
        yield return new WaitForSeconds(0.5f);
        PlayerController.Instance.controlsActive = true;
    }
}
