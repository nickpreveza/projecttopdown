using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyTest : Entity
{
    [Header("Enemy Test Settings - Navigation Tests")]
    NavMeshAgent agent;
    Vector3 positionToMove;
    public  bool targetPlayer;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        MaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer)
        {
            if (target == null)
            {
                target = PlayerController.Instance.gameObject;
            }

            agent.SetDestination(target.transform.position);
        }

        //calculate direction
        EntityDirection futureDir = EntityDirection.DOWN; 
        if (futureDir != direction)
        {
            direction = futureDir;
            DirectionChanged();
        }

    }
}
