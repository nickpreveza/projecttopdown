using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockPanel : UIPanel
{
    [Header("Relationship Unlock")]
    [SerializeField] GameObject relationshipUnlock;
    [SerializeField] Transform relationshipUnlockContentParent;
    [SerializeField] TextMeshProUGUI relationshipLevelText;
    [Header("Photo Unlock")]
    [SerializeField] GameObject photoUnlock;
    [SerializeField] Image photoHolder;

    public void UnlockRelationshipLevel()
    {
        StopAllCoroutines();
        StartCoroutine(UnlockLevelEnum());
    }

    public void UnlockPhoto()
    {
        StopAllCoroutines();
        StartCoroutine(UnlockPhotoEnum());
    }

    IEnumerator UnlockLevelEnum()
    {
        photoUnlock.SetActive(false);
        relationshipUnlock.SetActive(true);

        PlayerController.Instance.controlsActive = false;
        UIManager.Instance.unlockInProgress = true;
        relationshipLevelText.text = DataManager.Instance.publicData.relationshipLevel.ToString();

        foreach(Transform child in relationshipUnlockContentParent)
        {
            Destroy(child.gameObject);
        }

        GameObject obj = Instantiate(UIManager.Instance.GetProgressionItem(DataManager.Instance.publicData.relationshipLevel-1, true), relationshipUnlockContentParent);

        yield return new WaitForSeconds(3f);
        relationshipUnlock.SetActive(false);
        CompanionController.Instance.UpdateAbilities();
        UIManager.Instance.unlockInProgress = false;
       
        UIManager.Instance.OpenGamePanel();
        UIManager.Instance.UpdateAbilityElements();
        PlayerController.Instance.controlsActive = true;
    }

    IEnumerator UnlockPhotoEnum()
    {
        relationshipUnlock.SetActive(false);
        UIManager.Instance.unlockInProgress = true;
        PlayerController.Instance.controlsActive = false;
        photoHolder.sprite = GameManager.Instance.hub.GetImageWithIndex(DataManager.Instance.publicData.photos-1);
        photoUnlock.SetActive(true);
        yield return new WaitForSeconds(3f);
        UIManager.Instance.OpenGamePanel();
        UIManager.Instance.unlockInProgress = false;
        PlayerController.Instance.controlsActive = true;
    }
}
