using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedStore : MonoBehaviour
{
    [SerializeField] Transform item1;
    [SerializeField] Transform item2;
    [SerializeField] Transform item3;

    public List<GameObject> storeItemsTier1;
    public List<GameObject> storeItemsTier2;


    GameObject storeItem1;
    GameObject storeItem2;
    GameObject storeItem3;

    public List<StoreItem> selectedStoreItems;

    [SerializeField] GameObject bedLinen;
    [SerializeField] GameObject bedPillow;
    void CleanItems()
    {
        foreach (Transform child in item1)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in item2)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in item3)
        {
            Destroy(child.gameObject);
        }
    }
    void Start()
    {
        //StoreAppear();
    }

    public void Selection(StoreItem item, StoreItemFunc itemEffect, int amount)
    {
        ApplyEffect(itemEffect, amount);
        CleanItems();
        StoreDisappear();
        //GameManager.Instance.spawner.
    }

    void ApplyEffect(StoreItemFunc itemEffect,int amount)
    {
        switch (itemEffect)
        {
            case StoreItemFunc.pistol:
                AudioManager.Instance.Play("get_weapon");
                PlayerController.Instance.SetWeapon(1);
                break;
            case StoreItemFunc.shotgun:
                AudioManager.Instance.Play("get_weapon");
                PlayerController.Instance.SetWeapon(2);
                break;
            case StoreItemFunc.rifle:
                AudioManager.Instance.Play("get_weapon");
                PlayerController.Instance.SetWeapon(3);
                break;
            case StoreItemFunc.revolver:
                AudioManager.Instance.Play("get_weapon");
                PlayerController.Instance.SetWeapon(4);
                break;
            case StoreItemFunc.shotgunPlus:
                AudioManager.Instance.Play("get_weapon");
                PlayerController.Instance.SetWeapon(5);
                break;
            case StoreItemFunc.rifleExtra:
                AudioManager.Instance.Play("thanks");
                AudioManager.Instance.Play("get_weapon");
                PlayerController.Instance.SetWeapon(6);
                break;
            case StoreItemFunc.damageUp:
                AudioManager.Instance.Play("thanks");
                PlayerController.Instance.AddWeaponDamage(amount);
                break;
            case StoreItemFunc.speedUp:
                AudioManager.Instance.Play("thanks");
                PlayerController.Instance.AddProjectileSpeed(amount);
                break;
            case StoreItemFunc.playerSpeedUp:
                AudioManager.Instance.Play("thanks");
                PlayerController.Instance.AddPlayerSpeed(amount);
                break;
            case StoreItemFunc.maxHealthUp:
                AudioManager.Instance.Play("thanks");
                PlayerController.Instance.AddMaxHealth(amount);
                break;
            case StoreItemFunc.healthUp:
                AudioManager.Instance.Play("thanks");
                PlayerController.Instance.Heal(amount);
                break;
            case StoreItemFunc.petDamageUp:
                AudioManager.Instance.Play("thanks");
                CompanionController.Instance.AddDamage(amount);
                break;
            case StoreItemFunc.petSpeedUp:
                AudioManager.Instance.Play("thanks");
                CompanionController.Instance.AddSpeed(amount);
                break;


        }
    }

    StoreItemFunc  FindStoreItemFuncForPlayerWeapon()
    {
        switch (PlayerController.Instance.weaponIndex)
        {
            case 1:
                return storeItemsTier1[0].GetComponent<StoreItem>().itemEffect;
            case 2:
                return storeItemsTier1[1].GetComponent<StoreItem>().itemEffect;
            case 3:
                return storeItemsTier1[2].GetComponent<StoreItem>().itemEffect;
            case 4:
                return storeItemsTier2[0].GetComponent<StoreItem>().itemEffect;
            case 5:
                return storeItemsTier2[1].GetComponent<StoreItem>().itemEffect;
            case 6:
                return storeItemsTier2[2].GetComponent<StoreItem>().itemEffect;
        }

        return StoreItemFunc.pistol;
    }

    public void StoreAppear(bool hasPlayerTakenDamage)
    {
        AudioManager.Instance.Play("storeAppear");
        CleanItems();

        List<GameObject> selectedItems = new List<GameObject>();

        int amountOfItems = hasPlayerTakenDamage ? 1 : 3;

        StoreItemFunc itemFunction = FindStoreItemFuncForPlayerWeapon();


        if (DataManager.Instance.publicData.day <= 4)
        {
            while (selectedItems.Count < amountOfItems)
            {
                int randomIndex = Random.Range(0, storeItemsTier1.Count);

                if (!selectedItems.Contains(storeItemsTier1[randomIndex]) && storeItemsTier1[randomIndex].GetComponent<StoreItem>().itemEffect != itemFunction)
                {
                    selectedItems.Add(storeItemsTier1[randomIndex]);

                    if (selectedItems.Count >= amountOfItems)
                    {
                        break;
                    }
                }
            }

        }
        else
        {
            while (selectedItems.Count < amountOfItems)
            {
                int randomIndex = Random.Range(0, storeItemsTier2.Count);

                if (!selectedItems.Contains(storeItemsTier2[randomIndex]) && storeItemsTier2[randomIndex].GetComponent<StoreItem>().itemEffect != itemFunction)
                {
                    selectedItems.Add(storeItemsTier2[randomIndex]);

                    if (selectedItems.Count >= amountOfItems)
                    {
                        break;
                    }
                }
            }
        }

        if (hasPlayerTakenDamage)
        {
            storeItem2 = Instantiate(selectedItems[0], item2.transform.position, Quaternion.identity);
            storeItem2.transform.parent = item2;
        }
        else
        {
            storeItem1 = Instantiate(selectedItems[0], item1.transform.position, Quaternion.identity);
            storeItem1.transform.parent = item1;
            storeItem1.transform.position = storeItem1.transform.parent.position;
            storeItem2 = Instantiate(selectedItems[1], item2.transform.position, Quaternion.identity);
            storeItem2.transform.parent = item2;
            storeItem2.transform.position = storeItem2.transform.parent.position;
            storeItem3 = Instantiate(selectedItems[2], item3.transform.position, Quaternion.identity);
            storeItem3.transform.parent = item3;
            storeItem3.transform.position = storeItem3.transform.parent.position;
        }
      

        bedLinen.SetActive(false);
        bedPillow.SetActive(false);
    }

    public void StoreDisappear()
    {
     
        bedLinen.SetActive(true);
        bedPillow.SetActive(true);

        this.gameObject.SetActive(false);

    }
}
