using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisBallThrow : MonoBehaviour
{
    public GameObject tennisBallPrefab;
    public float baseSpeed;
    private float height;
    [Range(0, 1)] public float arcHeight;
    public float ballReturnDur;
    public int bounceCount;

    private PlayerController player;
    private GameManager gameManager;
    private TennisBallController tennisBall;

    private bool isItHub;
    private bool flying;

    //Ball throw
    private float progress;
    private float stepSize;
    private float distance;
    private Vector2 dir;
    private Vector3 targetPos;
    private Vector3 startPos;

    private bool arrived = false;
    private int bounceIndex = 0;
    private bool stopBouncing;

    private List<Vector3> targets = new List<Vector3>();
    public bool canThrow;

    internal float speed;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        gameManager = GameManager.Instance;
        isItHub = gameManager.hub != null;
        height = arcHeight;
    }

    // Update is called once per frame
    void Update() 
    {
        if (CompanionController.Instance != null)
        {
            if (CompanionController.Instance.ability_ballFollow)
            {
                if (Input.mouseScrollDelta.y > 0 && canThrow)
                {
                    if (PlayerController.Instance.controlsActive)
                    {
                        SetupThrow();
                    }
                }
            }
        }

        if (tennisBall != null)
        {
            if (!arrived)
            {
                tennisBall.transform.position = ParabolicMovement(targets[bounceIndex], targets[bounceIndex + 1]);
            }
            if (arrived && flying)
            {
                OnArrival(tennisBall);
            }
        }
    }

    Vector2 ParabolicMovement(Vector3 startPos, Vector3 target)
    {
        arcHeight = height * distance / (bounceIndex + 1);
        stepSize = speed / distance;
        flying = true;

        // Increment our progress from 0 at the start, to 1 when we arrive.
        progress = Mathf.Min(progress + Time.deltaTime * stepSize, 1.0f);

        // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.
        float parabola = 1.0f - 4.0f * (progress - 0.5f) * (progress - 0.5f);

        // Travel in a straight line from our start position to the target.        
        Vector3 nextPos = Vector3.Lerp(startPos, target, progress);

        // Then add a vertical arc in excess of this.
        nextPos.y += parabola * arcHeight;


        if (progress >= 1.0f)
        {
            if (bounceIndex == targets.Count - 2)
            {
                arrived = true;
            }
            else if (bounceIndex < targets.Count - 2)
            {
                progress = 0;
                bounceIndex++;
            }
        }

        return nextPos;
    }

    void SetupThrow()
    {
        AudioManager.Instance.Play("go");

        if (tennisBall != null)
        {
            return;
        }
        canThrow = false;
        speed = baseSpeed;
        stopBouncing = false;
        targets.Clear();
        bounceIndex = 0;
        startPos = player.gameObject.transform.position;
        targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tennisBall = Instantiate(tennisBallPrefab).GetComponent<TennisBallController>();
        tennisBall.gameObject.transform.position = startPos;
        distance = Vector2.Distance(targetPos, startPos);
        progress = 0;
        dir = (targetPos - startPos).normalized;
        targets.Add(startPos);
        targets.Add(targetPos);
        StartCoroutine(DogFetch());
        for (int i = 0; i < bounceCount; i++)
        {
            Vector3 nextPos = targets[i + 1] + (Vector3)dir * distance / ((i + 1) * 2);
            RaycastHit2D hit = Physics2D.Raycast(targets[i + 1], targets[i + 1] - nextPos, Vector2.Distance(targets[i + 1], nextPos));
            if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            {
                break;
            }
            targets.Add(nextPos);
        }
        arrived = false;
    }

    private IEnumerator DogFetch()
    {
        yield return new WaitForSeconds(0.4f);
        CompanionController.Instance.FetchBall(tennisBall.gameObject);
    }



    private void OnArrival(TennisBallController tennisBall)
    {
        
        flying = false;
        tennisBall.rb.simulated = true;
        tennisBall.rb.AddForce(30 * speed * dir / (bounceIndex + 1));
        if (tennisBall.gameObject.transform.position.x - player.transform.position.x >= 0)
        {
            tennisBall.rb.AddTorque(-(speed / (bounceIndex + 1) * 10));
        }
        else //if (tennisBall.gameObject.transform.position.x < 0)
        {
            tennisBall.rb.AddTorque(speed / (bounceIndex + 1)* 10);
        }
       
        WaitAndDestroyBall();
    }

    public void WaitAndDestroyBall()
    {
        StartCoroutine(WaitBeforeDestroyCO(tennisBall.gameObject, ballReturnDur));
    }
    private IEnumerator WaitBeforeDestroyCO(GameObject ball, float duration)
    {
       yield return new WaitForSeconds(duration);
       canThrow = true;
        if (ball != null)
        {
            Destroy(ball);
        }
       
    }
}