using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWeapon : Weapon
{
    float internalFireRate;
    public bool canShoot;

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
}
