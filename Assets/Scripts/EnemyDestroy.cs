using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroy : MonoBehaviour
{
    public GameObject EnemyDestroyEffect;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            Instantiate(EnemyDestroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
