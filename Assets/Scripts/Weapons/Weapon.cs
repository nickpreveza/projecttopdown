using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject handler;
    public Vector3 offsetFromSource;
    public int damage;
    public float attackRate;


   public virtual void Attack(Vector3 dir)
   {

   }

    public virtual void DirectionUpdate(WeaponDirection newDir)
    {

    }
}
