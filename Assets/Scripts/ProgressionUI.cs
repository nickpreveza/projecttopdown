using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressionUI : MonoBehaviour
{
    [SerializeField] bool levelOnly;
    [SerializeField] bool hasDoneSetup;
    public List<ProgressionItemUI> progressionItems;
    [SerializeField] Transform progressionItemsHolder;
    [SerializeField] Transform levelItemHolder;
    public List<GameObject> levelItems;


    private void OnEnable()
    {
        Setup();
    }

    void GenerateProgressionItems()
    {
        ClearOldItems();
        progressionItems = new List<ProgressionItemUI>();

        for (int i = 0; i < 8; i++)
        {
            GameObject newObj = Instantiate(UIManager.Instance.GetProgressionItem(i, false), progressionItemsHolder);
            progressionItems.Add(newObj.GetComponent<ProgressionItemUI>());
        }
    }

    void ClearOldItems()
    {
        foreach(Transform child in progressionItemsHolder)
        {
            Destroy(child.gameObject);
        }
    }

  
    public void Setup()
    {
        levelItems = new List<GameObject>();

        if (!levelOnly)
        {
            if (!hasDoneSetup)
            {
                GenerateProgressionItems();
                hasDoneSetup = true;
            }

            foreach (ProgressionItemUI item in progressionItems)
            {
                item.Lock();
            }

        }

        foreach (Transform child in levelItemHolder)
        {
            levelItems.Add(child.gameObject);
        }

        foreach (GameObject obj in levelItems)
        {
            obj.transform.GetChild(0).gameObject.SetActive(false); //image
            obj.transform.GetChild(1).gameObject.SetActive(false); //text
        }

        Unlocked();
    }

    public void Unlocked()
    {
        if (DataManager.Instance == null)
        {
            return;
        }

        for (int i = 0; i < DataManager.Instance.publicData.relationshipLevel; i++)
        {
            if (!levelOnly)
            {
                progressionItems[i].Unlock();
            }

            levelItems[i].transform.GetChild(0).gameObject.SetActive(true); //image


            if (i == DataManager.Instance.publicData.relationshipLevel - 1)
            {
                levelItems[i].transform.GetChild(1).gameObject.SetActive(true); //text
                levelItems[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.publicData.relationshipLevel.ToString();
            }
        }


    }

}
