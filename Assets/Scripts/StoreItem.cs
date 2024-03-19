using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItem : Interactable
{
    [SerializeField] int amount;
    public StoreItemFunc itemEffect;
    public override void Interact()
    {
        GameManager.Instance.spawner.bedStore.Selection(this, itemEffect, amount);
        isInteractable = false;
     
        base.Interact();
    }
}

public enum StoreItemFunc
{
    healthUp,
    maxHealthUp,
    damageUp,
    speedUp,
    playerSpeedUp,
    petSpeedUp,
    petDamageUp,
    shotgun,
    shotgunPlus,
    rifle,
    pistol,
    revolver,
    rifleExtra

}
