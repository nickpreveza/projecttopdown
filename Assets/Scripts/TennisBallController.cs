using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisBallController : Interactable
{
    public CircleCollider2D col;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        col = gameObject.GetComponent<CircleCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        isInteractable = true;
    }

    public override void Interact()
    {
        base.Interact();
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.simulated = true;
        
    }

    public void DestroyBall()
    {
        Destroy(this.gameObject);
    }
}
