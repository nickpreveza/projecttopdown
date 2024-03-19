using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawner : MonoBehaviour 
{
    SpriteRenderer thisRenderer;
    public Transform spawnPosition;
    public Sprite activeSpawner;
    public Sprite disabledSpawner;
    public GameObject localLight;

    private void Start()
    {
        thisRenderer = this.GetComponent<SpriteRenderer>();
        SetAsDisabled();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(spawnPosition.position, 1f);
    }

    public void SetAsActive()
    {
        thisRenderer.sprite = activeSpawner;
        localLight.SetActive(true);
    }

    public void SetAsDisabled()
    {
        thisRenderer.sprite = disabledSpawner;
        localLight.SetActive(false);
    }
    public void Spawn(EnemyType type)
    {
        GameObject obj;

        switch (type)
        {
            case EnemyType.Fear:
                obj = Instantiate(GameManager.Instance.fearEnemyPrefab, spawnPosition.position, Quaternion.identity);
                obj.transform.parent = null;
                break;
            case EnemyType.Hopelessness:
                obj = Instantiate(GameManager.Instance.hopelessnessEnemyPrefab, spawnPosition.position, Quaternion.identity);
                obj.transform.parent = null;
                break;
            case EnemyType.Resentment:
                obj = Instantiate(GameManager.Instance.resentmessEnemyPrefab, spawnPosition.position, Quaternion.identity);
                obj.transform.parent = null;
                break;
            case EnemyType.Loneliness:
                obj = Instantiate(GameManager.Instance.lonelinessEnemyPrefab, spawnPosition.position, Quaternion.identity);
                obj.transform.parent = null;
                break;
            case EnemyType.Anxiety:
                obj = Instantiate(GameManager.Instance.anxietyEnemyPrefab, spawnPosition.position, Quaternion.identity);
                obj.transform.parent = null;
                break;
            case EnemyType.Anger:
                obj = Instantiate(GameManager.Instance.angerEnemyPrefab, spawnPosition.position, Quaternion.identity);
                obj.transform.parent = null;
                break;

        }
    }
}
