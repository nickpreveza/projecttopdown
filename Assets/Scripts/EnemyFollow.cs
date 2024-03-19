using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    //Allows the target(in this case the player) to be dragged onto the script to be used as the thing it will follow
    [SerializeField] private GameObject target;
    
    //Speed of the enemy
    [SerializeField] private float speed = 2f;
    //Attack of enemy
    [SerializeField] private float attack = 10f;
    //Attack interval of enemy
    [SerializeField] private float attackspd = 1f;
    private float CanAttack = 1f;

    [SerializeField] int health;
    int currHealth;

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Checks if collider has found the player
        if (collision.gameObject.tag == target.tag)
        {
            Debug.Log("Enemy touched player");
            if (attackspd <= CanAttack)
            {
                //Takes the attack damage of enemy from player health
                collision.gameObject.GetComponent<PlayerHealth>().UpdateHealth(-attack);
                CanAttack = 0f;
            }
            else
            {
                CanAttack += Time.deltaTime;
            }
        }
    }

    private void Awake()
    {
        currHealth = health;
    }

    void Update()
    {
        //Checks position of Enemy and moves towards position of target/player
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        //Rotates sprite towards the direction it is moving in
        transform.right = target.transform.position - transform.position;
    }

    public void TakeDamage(int damage)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
