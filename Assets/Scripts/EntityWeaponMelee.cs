using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWeaponMelee : MonoBehaviour
{
    public int attackDamage;
    public bool haveAppliedDamage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !haveAppliedDamage)
        {
            PlayerController.Instance.Damage(attackDamage);
            haveAppliedDamage = true;
        }
    }
}
