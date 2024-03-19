using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D rb;
    public GunHandler gunHandler;
    Vector2 force;
    int damage = 1;
    private void Awake()
    {
       rb = GetComponent<Rigidbody2D>(); 
    }

    public void Attack(float speed, float lifetime)
    {
        rb.AddForce(gunHandler.transform.right * speed); //Adds force in the direction of the mouse
        Invoke("ReturnToPool", lifetime); //Returns the bullet to the pool
    }

    void ReturnToPool()
    {
        if (!this.gameObject.activeSelf)
        {
            return;
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject colObject = collision.gameObject;
        if (colObject.CompareTag("Enemy"))
        {
            colObject.GetComponent<EnemyFollow>().TakeDamage(damage); //Deals damage to the enemy according to what weapon you use
        }
    }
}
