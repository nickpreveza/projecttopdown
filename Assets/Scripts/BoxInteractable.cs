using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInteractable : Interactable
{
    public Sprite boxOpened;
    Animator anim;
    GameObject content;
    [SerializeField] Vector3 contentSpawnOffset;
    private void Start()
    {
        anim = GetComponent<Animator>();
        content = transform.GetChild(0).gameObject;
    }
    public override void Interact()
    {
        if (!isInteractable)
        {
            return;
        }

        this.GetComponent<SpriteRenderer>().sprite = boxOpened;
        Invoke("ShowContents", 1f);
        isInteractable = false;
        //show dog
        //vanish in a bit or something;
        base.Interact();

    }

    void ShowContents()
    {
        AudioManager.Instance.Play("openbox");
        DataManager.Instance.OpenBox();
       anim.SetTrigger("Show");
        Invoke("SpawnRealContetnt", 0.8f);
    }

    void SpawnRealContetnt()
    {
        GameObject newPet = Instantiate(GameManager.Instance.companionPrefab, content.transform.position + contentSpawnOffset, Quaternion.identity);
        newPet.transform.parent = null;
        newPet.transform.position = content.transform.position;
        content.SetActive(false);
    }
}
