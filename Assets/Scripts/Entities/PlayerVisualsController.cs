using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualsController : MonoBehaviour
{
    [Header("Head")]
    [SerializeField] SpriteRenderer headSprite;
    [SerializeField] Sprite headRight;
    [SerializeField] Sprite headDown;
    [SerializeField] Sprite headUp;

    [Header("Body")]
    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] Sprite bodyRight;
    [SerializeField] Sprite bodyDown;
    [SerializeField] Sprite bodyUp;

    [Header("Animation")]
    public Animator playerAnimator;
    [SerializeField] Animator effectAnimator; 
    [Space(10)]
    [Header("Expressions")]

    [SerializeField] Sprite swirlRight;
    [SerializeField] Sprite swirlDown;
    [SerializeField] Sprite swirlUp;
    [Space(5)]
    [SerializeField] Sprite happyRight;
    [SerializeField] Sprite happyDown;
    [SerializeField] Sprite happyUp;
    [Space(5)]
    [SerializeField] Sprite sadRight;
    [SerializeField] Sprite sadDown;
    [SerializeField] Sprite sadUp;
    [Space(5)]
    [SerializeField] Sprite hitRight;
    [SerializeField] Sprite hitDown;
    [SerializeField] Sprite hitUp;

    IEnumerator statusChangeCoroutine;

    [Header("Hands")]
    [SerializeField] GameObject extraHands;
    Animator extraHandsAnim;
    [SerializeField] Vector3 rightRotation;
    [SerializeField] Vector3 leftRotation;

    private void Start()
    {
        playerAnimator.SetBool("isWalking", false);
        extraHandsAnim = extraHands.GetComponent<Animator>();

        foreach (Transform child in effectAnimator.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    public void UpdateStatus(EntityStatus previousStatus, float delay)
    {
        if (statusChangeCoroutine != null)
        {
            StopCoroutine(statusChangeCoroutine);
        }

        UpdateStatus();
        statusChangeCoroutine = RevertState(previousStatus, delay);
        StartCoroutine(statusChangeCoroutine);
    }
    public void UpdateStatus()
    {
        switch (PlayerController.Instance.emotionState)
        {
            case EntityStatus.DEFAULT:
                headRight = swirlRight;
                headDown = swirlDown;
                headUp = swirlUp;
                break;
            case EntityStatus.SAD:
                headRight = sadRight;
                headDown = sadDown;
                headUp = sadUp;
                break;
            case EntityStatus.HAPPY:
                headRight = happyRight;
                headDown = happyDown;
                headUp = happyUp;
                break;
            case EntityStatus.HIT:
                headRight = hitRight;
                headDown = hitDown;
                headUp = hitUp;
                break;

        }

        UpdateVisuals(PlayerController.Instance.currentDir);
    }

    IEnumerator RevertState(EntityStatus previousState, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerController.Instance.emotionState = previousState;
        UpdateStatus();
    }

    public void EffectHealthUp()
    {
        effectAnimator.SetTrigger("HealthUp");
    }

    public void EffectShield()
    {
        effectAnimator.SetTrigger("Shield");
    }

    public void Pet()
    {
        extraHandsAnim.SetTrigger("Pet");
    }

    public void Call()
    {
        extraHandsAnim.SetTrigger("Call");
    }

    public void Stay()
    {
        extraHandsAnim.SetTrigger("Stay");
    }

    public void UpdateVisuals(EntityDirection dir)
    {
        headSprite.flipX = false;
        bodySprite.flipX = false;
        bodySprite.sortingOrder = 4999;
        switch (dir)
        {
            case EntityDirection.UP:
                headSprite.sprite = headUp;
                bodySprite.sprite = bodyUp;
                bodySprite.sortingOrder = 5001;
                extraHands.transform.eulerAngles = rightRotation;
                break;
            case EntityDirection.DOWN:
                headSprite.sprite = headDown;
                bodySprite.sprite = bodyDown;
                extraHands.transform.eulerAngles = rightRotation;
                break;
            case EntityDirection.LEFT:
                headSprite.sprite = headRight;
                bodySprite.sprite = bodyRight;
                extraHands.transform.eulerAngles = leftRotation;

                headSprite.flipX = true;
                bodySprite.flipX = true;
                break;
            case EntityDirection.RIGHT:
                headSprite.sprite = headRight;
                bodySprite.sprite = bodyRight;
                extraHands.transform.eulerAngles = rightRotation;
                break;
        }
    }

    public void PlayerMoving(bool isMoving)
    {
        playerAnimator.SetBool("isWalking", isMoving);
    }
}

public enum WeaponDirection
{
    LEFT,
    RIGHT
}

public enum EntityStatus
{
    DEFAULT = 0,
    HAPPY = 1,
    SAD = 2,
    HIT = 3
}

