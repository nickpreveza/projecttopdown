using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    private float health = 0f;
    [SerializeField] private float max_health = 100f;

    private void Start()
    {
        health = max_health;
    }

    public void UpdateHealth(float value)
    {
        health += value;

        if (health <= 0f)
        {
            health = 0f;
            Debug.Log("You died");
        }
    }

}
