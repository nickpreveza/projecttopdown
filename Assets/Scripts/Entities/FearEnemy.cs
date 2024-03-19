using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FearEnemy : Entity
{
    int newHealth = 5;
    int newSpeed = 10;
    float dist;
    NavMeshAgent agent;
    Vector2 playerPos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerPos = player.transform.position;
        dist = Vector2.Distance(transform.position, playerPos);
        if (dist > 10)
        {
            agent.SetDestination(playerPos);
        }
        else
        {
            agent.SetDestination(-playerPos);
        }
        if (agent.isStopped)
        {

        }
    }

    void Shoot()
    {

    }

    public override void SetSpeed(int amount)
    {
        base.SetSpeed(newSpeed);
    }

    public override void SetHealth(int amount, bool fillHealth)
    {
        base.SetHealth(newHealth, fillHealth);
    }
}
