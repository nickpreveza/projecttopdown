using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEnemyCollision : MonoBehaviour
{
    TennisBallController controller;
    bool hasTouchedEnemy;
    private void Awake()
    {
        controller = transform.parent.GetComponent<TennisBallController>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTouchedEnemy)
        {
            hasTouchedEnemy = true;
            Entity enemyCollided = collision.gameObject.GetComponent<Entity>();
            if (enemyCollided != null)
            {
                Debug.Log("Enemy touched by ball");
                //enemyCollided.SetAsCompanionTarget();
                //controller.DestroyBall();
            }
        }
       
    }

}
