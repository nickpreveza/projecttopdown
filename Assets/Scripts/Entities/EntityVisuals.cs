using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisuals : MonoBehaviour
{
    public bool hasHeadOffset;
    [SerializeField] Vector3 headOffest;
    Vector3 originalHeadPosition;
    Vector3 adjustedPosition;

    [SerializeField] SpriteRenderer[] otherBodyParts;
    [SerializeField] Material bodySpriteNormal;
    [SerializeField] Material bodySpriteHit;
    [SerializeField] Material headSpriteNormal;
    [SerializeField] Material headSpriteHit;
    [SerializeField] Material frozenMaterial;
    [Header("Head")]
    public SpriteRenderer headSprite;
    [SerializeField] Sprite headRight;
    [SerializeField] Sprite headDown;
    [SerializeField] Sprite headUp;

    [Header("Body")]
    public SpriteRenderer bodySprite;
    [SerializeField] Sprite bodyRight;
    [SerializeField] Sprite bodyDown;
    [SerializeField] Sprite bodyUp;

    [Header("Animation")]
    [SerializeField] Animator entityAnimator;
    [SerializeField] Animator effectAnimator;
    [Header("Status Change Sprites")]

    [SerializeField] Sprite hitRight;
    [SerializeField] Sprite hitDown;
    [SerializeField] Sprite hitUp;

    [Space(5)]
    Sprite defaultRight;
    Sprite defaultDown;
    Sprite defaultUp;

    IEnumerator statusChangeCoroutine;

    private void Start()
    {
        defaultRight = headRight;
        defaultDown = headDown;
        defaultUp = headUp;
        originalHeadPosition = headSprite.gameObject.transform.position;
        adjustedPosition = originalHeadPosition + headOffest;
        VisualReturnToNormal();
    }

    public void TargetToggle(bool isTarget)
    {
        if (isTarget)
        {

        }
        else
        {

        }
    }

    public void EffectTarget()
    {
        effectAnimator.SetTrigger("Target");
    }


    public void EffectHealthUp()
    {
        effectAnimator.SetTrigger("HealthUp");
    }

    public void EffectShield()
    {
        effectAnimator.SetTrigger("Shield");
    }

    public void EffectFreeze()
    {
        effectAnimator.SetTrigger("Freeze");
        bodySprite.sharedMaterial = GameManager.Instance.frozenMaterial;
        headSprite.sharedMaterial = GameManager.Instance.frozenMaterial;

        foreach(SpriteRenderer renderer in otherBodyParts)
        {
            renderer.sharedMaterial = GameManager.Instance.frozenMaterial;
        }
    }

    public void EffectUnfreeze()
    {
        effectAnimator.SetTrigger("Default");
        bodySprite.sharedMaterial = bodySpriteNormal;
        headSprite.sharedMaterial = headSpriteNormal;

        foreach (SpriteRenderer renderer in otherBodyParts)
        {
            renderer.sharedMaterial = GameManager.Instance.normalMaterial;
        }
    }



    public void VisualFeedbackHit(bool isFrozen)
    {
        bodySprite.sharedMaterial = bodySpriteHit;
        headSprite.sharedMaterial = headSpriteHit;

        foreach (SpriteRenderer renderer in otherBodyParts)
        {
            renderer.sharedMaterial = bodySpriteHit;
        }

        if (!isFrozen)
        {
            Invoke("VisualReturnToNormal", 0.2f);
        }
        else
        {
            Invoke("EffectFreeze", 0.2f);
        }
      
    }

    public void VisualReturnToNormal()
    {
        bodySprite.material = bodySpriteNormal;
        headSprite.material = headSpriteNormal;

        foreach (SpriteRenderer renderer in otherBodyParts)
        {
            renderer.sharedMaterial = GameManager.Instance.normalMaterial;
        }
    }
    public void UpdateStatus(Entity targetEntity, EntityStatus previousState)
    {
        if (statusChangeCoroutine != null)
        {
            StopCoroutine(statusChangeCoroutine);
        }

        UpdateStatus(targetEntity);
        statusChangeCoroutine = RevertState(targetEntity, previousState);
        StartCoroutine(statusChangeCoroutine);
    }
    public void UpdateStatus(Entity targetEntity)
    {
        switch (targetEntity.status)
        {
            case EntityStatus.HIT:
                headRight = hitRight;
                headDown = hitDown;
                headUp = hitUp;
                break;
            case EntityStatus.DEFAULT:
                headRight = defaultRight;
                headDown = defaultDown;
                headUp = defaultUp;
                break;
        }

        //UpdateVisuals(targetEntity);
    }

    IEnumerator RevertState(Entity targetEntity, EntityStatus previousStatus)
    {
        yield return new WaitForSeconds(targetEntity.hitFeedbackTime);
        targetEntity.status = previousStatus;
        UpdateStatus(targetEntity);
    }

    public void UpdateVisuals(Entity targetEntity)
    {
        headSprite.flipX = false;
        bodySprite.flipX = false;
        bodySprite.sortingOrder = 4999;
        switch (targetEntity.direction)
        {
            case EntityDirection.UP:
               
                headSprite.sprite = headUp;
                bodySprite.sprite = bodyUp;
                bodySprite.sortingOrder = 5001;
                break;
            case EntityDirection.DOWN:
                headSprite.sprite = headDown;
                bodySprite.sprite = bodyDown;
                break;
            case EntityDirection.LEFT:
                if (hasHeadOffset)
                {
                    headSprite.gameObject.transform.position = adjustedPosition;
                }
               
                headSprite.sprite = headRight;
                bodySprite.sprite = bodyRight;
                
                headSprite.flipX = true;
                bodySprite.flipX = true;
                break;
            case EntityDirection.RIGHT:
                headSprite.gameObject.transform.position = originalHeadPosition;
                headSprite.sprite = headRight;
                bodySprite.sprite = bodyRight;
                break;
        }
    }

    public void EntityAnimatorSetTrigger(string trigger)
    {
        entityAnimator.SetTrigger(trigger);
    }

}
