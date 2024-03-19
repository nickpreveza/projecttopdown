using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoInteractable : Interactable
{
    public void Start()
    {
        isInteractable = true;
    }

    public override void Interact()
    {
        if (!isInteractable)
        {
            return;
        }

        base.Interact();

        isInteractable = false;

        if (GameManager.Instance.hub == null)
        {
            GameManager.Instance.SequenceCompleted();
        }
        else
        {
            DataManager.Instance.AddPhoto();
            Destroy(this.gameObject);
        }
      
       
     
       

     
       

    }
}
