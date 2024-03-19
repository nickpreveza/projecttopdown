using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubSystems : MonoBehaviour
{
    public BedInteractable bed;
    public BoxInteractable box;
    public GameObject dummyEntity;
    public GameObject companion;
    public Transform photoSpawn;
    bool hasPettedDog;
    bool hasSeenPicture;
    [SerializeField] Sprite[] galleryImages;
    public bool DogPet
    {
        get
        {
            return hasPettedDog;
        }
        set
        {
            if (value == true)
            {
                if (hasSeenPicture)
                {
                    GameManager.Instance.hub.bed.isInteractable = true;
                }
               
            }
            hasPettedDog = value;
        }

    }

    public bool PictureSeen
    {
        get
        {
            return hasSeenPicture;
        }
        set
        {
            if (value == true)
            {
                if (hasPettedDog)
                {
                    GameManager.Instance.hub.bed.isInteractable = true;
                }
            }

            hasSeenPicture = value;
        }
    }

    public Sprite GetImageWithIndex(int index)
    {
        if (index >= galleryImages.Length)
        {
            Debug.LogError("Gallery Image Index exceeded images in database");
        }
        return galleryImages[index];
    }
}
