using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionVisuals : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Animator effectAnimator;
    [SerializeField] Vector3 eulerRight;
    [SerializeField] Vector3 eulerLeft;
    [SerializeField] GameObject body;
    [SerializeField] GameObject tongue;
    [Header("Head")]
    [SerializeField] SpriteRenderer headSprite;
    [SerializeField] float hitFeedbackTime;
    float feedbackTimerInternal;
    bool hasChangedStatus;
    EntityStatus currentStatus = EntityStatus.DEFAULT;

    [SerializeField] Sprite defaultHead;
    [SerializeField] Sprite hitHead;
    [SerializeField] Sprite happyHead;
    [SerializeField] Sprite sadHead;
    [SerializeField] Sprite blinkHead;


    [SerializeField] SpriteRenderer[] allBodyParts;
    [SerializeField] GameObject ball;
    bool localIsWalking;
    [Header("Effects")]

    [SerializeField] ParticleSystem freezeEffect;
    [SerializeField] ParticleSystem healEffect;
    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isWalking", false);
        anim.SetBool("isSitting", false);
        tongue.SetActive(false);

    }

    public void EffectCall()
    {
        anim.SetTrigger("Coming");
    }

    public void EffectHeal()
    {
        //healEffect.Play();
    }

    public void FreezeAbilityEffect()
    {
        effectAnimator.SetTrigger("FreezeAbility");
        freezeEffect.gameObject.SetActive(true);
        freezeEffect.Play();
    }

    public void AttackAbilityEffect()
    {
        effectAnimator.SetTrigger("AttackAbility");
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
       // effectAnimator.SetTrigger("Freeze");
    }
    public void UpdateDirection(CompanionState state, EntityDirection direction)
    {
        if (state == CompanionState.PLAYERREACHED)
        {
            return;
        }

        switch (direction)
        {
            case EntityDirection.RIGHT:
                body.transform.eulerAngles = eulerRight;
              
                break;
            case EntityDirection.LEFT:
                body.transform.eulerAngles = eulerLeft;
                break;
        }
    }



    public void Stay()
    {
        anim.SetBool("isSitting", true);
    }
    public void Pet()
    {
        anim.SetTrigger("Pet");
        ChangeEffect(EntityStatus.HAPPY, 1f);
        UIManager.Instance.HidePetPrompt();
    }

    public void ShowBall()
    {
        ball.gameObject.SetActive(true);
    }

    public void HideBall()
    {
        ball.gameObject.SetActive(false);
    }

    public void ToggleTongue(bool toggle)
    {
        tongue.SetActive(toggle);
    }
    public void AttackAnimation()
    {
        anim.SetTrigger("Attack");
    }
    public void SetMove(bool isMoving, bool isSitting)
    {
        if (isMoving != localIsWalking)
        {
            localIsWalking = isMoving;
            anim.SetBool("isWalking", localIsWalking);
            if (!localIsWalking)
            {

                anim.SetBool("isSitting", true);
            }
        }
        //anim.SetBool("isWalking", isMoving);
       
       
    }

    public void ChangeEffect(EntityStatus status, float reverseTimer)
    {
        switch (status)
        {
            case EntityStatus.DEFAULT:
                headSprite.sprite = defaultHead;
                break;
            case EntityStatus.HAPPY:
                headSprite.sprite = blinkHead;
                break;
            case EntityStatus.HIT:
                headSprite.sprite = hitHead;
                break;
            case EntityStatus.SAD:
                headSprite.sprite = sadHead;
                break;
        }
        hasChangedStatus = true;
        feedbackTimerInternal = reverseTimer;
    }

    void SetDefaultStatus()
    {
        headSprite.sprite = defaultHead;
    }

    private void Update()
    {
        if (hasChangedStatus)
        {
            feedbackTimerInternal -= Time.deltaTime;

            if (feedbackTimerInternal <= 0)
            {
                hasChangedStatus = false;
                SetDefaultStatus();
            }
        }

        if (this.gameObject.transform.position.y > PlayerController.Instance.gameObject.transform.position.y)
        {
            ChangeSortingOrder(4000);
        }
        else
        {
            ChangeSortingOrder(6000);
        }
    }

    public void ChangeSortingOrder(int amount)
    {
        for (int i = 0; i < allBodyParts.Length; i++) 
        {
            allBodyParts[i].sortingOrder = amount + i;
        }
    }
}
