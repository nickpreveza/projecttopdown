using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandler : MonoBehaviour
{
    [SerializeField] Sprite bulletSprite;
    [SerializeField] Sprite flashSprite;
    [SerializeField] GameObject bulletSpawnPosition;

    [SerializeField] GameObject bulletPoolParent;
    [SerializeField] List<GameObject> bulletPool = new List<GameObject>();

    GameObject bulletObject;
    [SerializeField] Vector3 bulletSpawnOffset;
    [SerializeField] WeaponDirection curDirection = WeaponDirection.RIGHT; //important to be set as right

    private void Awake()
    {
        bulletPool = new List<GameObject>();
        foreach (Transform child in bulletPoolParent.transform)
        {
            bulletPool.Add(child.gameObject);
        }
    }

    public void UpdateBulletSpawnOffset(WeaponDirection newDir)
    {
        if (newDir == curDirection)
        {
            return;
        }
        curDirection = newDir;
        switch (curDirection)
        {
            case WeaponDirection.LEFT:
                bulletSpawnPosition.transform.localPosition += bulletSpawnOffset;
                break;
            case WeaponDirection.RIGHT:
                bulletSpawnPosition.transform.localPosition -= bulletSpawnOffset;
                break;
        }
    }

    public void Shoot(float speed, float lifetime)
    {
        bulletObject = null;
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeSelf)
            {
                bulletObject = bulletPool[i];
                break;
            }
        }

        if (bulletObject == null)
        {
            bulletObject = bulletPool[0];
            Debug.LogError("Bullet Pool does not have enough bullets for Fire Rate");
        }

        bulletObject.transform.position = bulletSpawnPosition.transform.position;
        bulletObject.transform.rotation = this.transform.rotation;
        bulletObject.GetComponent<BulletScript>().gunHandler = this;
        bulletObject.SetActive(true);
        bulletObject.GetComponent<BulletScript>().Attack(speed, lifetime);
    }
}
