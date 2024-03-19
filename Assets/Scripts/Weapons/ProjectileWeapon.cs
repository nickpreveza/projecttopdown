using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feel;

public class ProjectileWeapon : Weapon
{
    [Header("Projectile Weapon Settings")]
    public AttackPattern attackPattern;
    public GameObject bulletPrefab;
    public float shotgunSpreadAngle;
    public float burstRate; //used for shotgun shells too
    public int burstAmmo;
    [SerializeField] int bulletsInPool;
    [SerializeField] List<GameObject> bulletPool = new List<GameObject>();
    [SerializeField] Vector3 bulletSpawnOffset;
    [SerializeField] GameObject bulletSpawnPosition;

    [SerializeField] WeaponDirection gunDirection = WeaponDirection.RIGHT;
    GameObject bulletObject;
    public DamageType damageType;
    float internalFireRate;
    public float projectileLifetime;
    public float projectileSpeed;

    public GameObject bulletPoolParent;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer weaponSprite;
    [SerializeField] SpriteRenderer handSprite;
    [SerializeField] SpriteRenderer muzzleFlash;
    [SerializeField] Animator weaponAnimator;

    public bool shootingReady;
    bool canShoot;
    
    [SerializeField] float cameraShakeAmount = 1;
    [SerializeField] float cameraShakeTime = 0.1f;
    private void Start()
    {
        CreateProjectilePool();
        muzzleFlash.gameObject.SetActive(false);
        internalFireRate = attackRate;
    }

    void CreateProjectilePool()
    {
        if (bulletPoolParent != null)
        {
            Destroy(bulletPoolParent);
        }

        bulletPoolParent = new GameObject("BulletPoolParent");
        bulletPool = new List<GameObject>();

        for (int i = 0; i < bulletsInPool; i++)
        {
            GameObject projectile = Instantiate(bulletPrefab, bulletPoolParent.transform);
            projectile.SetActive(false);
            bulletPool.Add(projectile);
        }

        shootingReady = true;
    }

    private void Update()
    {
        internalFireRate += Time.deltaTime;
        if (internalFireRate > attackRate)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }


    public override void Attack(Vector3 dir)
    {
        if (canShoot)
        {
            PlayerController.Instance.shootingFeedback?.PlayFeedbacks();

            AudioManager.Instance.Play("bullet_fall");
            internalFireRate = 0;
            switch (attackPattern)
            {
                case AttackPattern.Pistol:
                    AudioManager.Instance.Play("pistol");
                    Pistol(dir);
                    break;
                case AttackPattern.Shotgun:
                    AudioManager.Instance.Play("shotgun");
                    Invoke("PlayShotgunPump", 0.4f);
                    Shotgun(dir, shotgunSpreadAngle);
                    break;
                case AttackPattern.Burst:
                    StartCoroutine(BurstShot(dir, burstRate, burstAmmo));
                    break;
                case AttackPattern.DoubleBarrel:
                    AudioManager.Instance.Play("shotgun");
                    Invoke("PlayShotgunPump", 0.4f);
                    DoubleBarrel(dir, shotgunSpreadAngle);
                    break;
            }
        }
        else
        {
            AudioManager.Instance.Play("failed_shot");
        }

    }

    public void PlayShotgunPump()
    {
        AudioManager.Instance.Play("shotgun_pump");
    }

    public void Pistol(Vector3 dir)
    {
        weaponAnimator.SetTrigger("Shoot");
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
        bulletObject.GetComponent<Projectile>().origin = this;
        bulletObject.SetActive(true);
        bulletObject.GetComponent<Projectile>().Attack(dir, 0, damage, projectileSpeed, projectileLifetime);
      
    }


    public void Shotgun(Vector3 dir, float spread)
    {
        weaponAnimator.SetTrigger("Shoot");
        List<GameObject> bulletObjects = new List<GameObject>();
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeSelf)
            {
                if (!bulletObjects.Contains(bulletPool[i]))
                {
                    bulletObjects.Add(bulletPool[i]);
                }

                if (bulletObjects.Count >= 3)
                {
                    break;
                }
               
            }
        }

        for (int i = 0; i < 3; i++)
        {
            bulletObjects[i].transform.position = bulletSpawnPosition.transform.position;
            bulletObjects[i].transform.rotation = this.transform.rotation;
            bulletObjects[i].GetComponent<Projectile>().origin = this;
            bulletObjects[i].SetActive(true);
            bulletObjects[i].GetComponent<Projectile>().Attack(dir, spread - spread * i, damage, projectileSpeed, projectileLifetime);
        }
        bulletObjects.Clear();
    }

    public void DoubleBarrel(Vector3 dir, float spread)
    {
        weaponAnimator.SetTrigger("Shoot");
        List<GameObject> bulletObjects = new List<GameObject>();
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeSelf)
            {
                if (!bulletObjects.Contains(bulletPool[i]))
                {
                    bulletObjects.Add(bulletPool[i]);
                }

                if (bulletObjects.Count >= 2)
                {
                    break;
                }

            }
        }

        for (int i = 0; i < 2; i++)
        {
            bulletObjects[i].transform.position = bulletSpawnPosition.transform.position;
            bulletObjects[i].transform.rotation = this.transform.rotation;
            bulletObjects[i].GetComponent<Projectile>().origin = this;
            bulletObjects[i].SetActive(true);
            if (i != 0)
            {
                bulletObjects[i].GetComponent<Projectile>().Attack(dir, spread * -i, damage, projectileSpeed, projectileLifetime);
            }
            else
            {
                bulletObjects[i].GetComponent<Projectile>().Attack(dir, spread, damage, projectileSpeed, projectileLifetime);
            }
           
        }
        bulletObjects.Clear();
    }

    public IEnumerator BurstShot(Vector3 dir, float burstRate, int ammo)
    {
        for (int i = 0; i < ammo; i++)
        {
            AudioManager.Instance.Play("burst");
            Pistol(dir);
            yield return new WaitForSeconds(burstRate);
        }
    }

    public override void DirectionUpdate(WeaponDirection newDir)
    {
        if (newDir == gunDirection)
        {
            return;
        }

        gunDirection = newDir;
        switch (gunDirection)
        {
            case WeaponDirection.LEFT:
                bulletSpawnPosition.transform.localPosition += bulletSpawnOffset;
                weaponSprite.flipY = true;
                handSprite.flipY = true;
                break;
            case WeaponDirection.RIGHT:
                bulletSpawnPosition.transform.localPosition -= bulletSpawnOffset;
                weaponSprite.flipY = false;
                handSprite.flipY = false;
                break;
        }
    }
}

public enum AttackPattern
{
    Pistol,
    Shotgun,
    Charge,
    Burst,
    DoubleBarrel
}

