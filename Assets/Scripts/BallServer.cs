using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BallServer : NetworkBehaviour
{
    [Header("Motion")]
    [SerializeField] float speed = 7f;
    [SerializeField] float maxBounceAngleDeg = 60f;

    Vector2 vel;
    bool roundRunning;         
    float nextHitTime = 0f;      // hit cooldown
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;   // only server drives the ball
        FreezeNow();             // start centered and stopped
    }

    void Update()
    {
        if (!IsServer) return;
        if (!roundRunning) return;

        transform.position += (Vector3)(vel * Time.deltaTime); // manual move for speed
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;
        if (!roundRunning) return;
        if (Time.time < nextHitTime) return; // prevent repeated triggers
        
        nextHitTime = Time.time + 0.05f;     // buffer

        if (other.CompareTag("Wall"))
        {
            vel.y *= -1f;                    // bounce vertical
            vel = vel.normalized * speed;
            return;
        }

        if (other.CompareTag("Paddle"))
        {
            float dy = transform.position.y - other.transform.position.y; // hit offset from paddle center
            float half = other.bounds.extents.y;                          // paddle half height
            float t = (half > 0f) ? Mathf.Clamp(dy / half, -1f, 1f) : 0f; // normalize -1..1

            float angle = t * maxBounceAngleDeg * Mathf.Deg2Rad;          // clamp bounce angle

            float dirX = (transform.position.x < other.transform.position.x) ? -1f : 1f; // bounce away from paddle

            Vector2 newDir = new Vector2(Mathf.Cos(angle) * dirX, Mathf.Sin(angle));

            newDir.y += Random.Range(-0.05f, 0.05f); // tiny variation for randomness

            vel = newDir.normalized * speed;
            return;
        }
    }

    public void FreezeNow()
    {
        if (!IsServer) return;
        roundRunning = false;
        vel = Vector2.zero;            // kill velocity
        transform.position = Vector3.zero;
    }

    public void StartRoundAfterDelay(float seconds, bool serveRight)
    {
        if (!IsServer) return;
        StopAllCoroutines();                 
        FreezeNow();                         
        StartCoroutine(StartRoundCoroutine(seconds, serveRight));
    }

    IEnumerator StartRoundCoroutine(float seconds, bool serveRight)
    {
        yield return new WaitForSeconds(seconds); // countdown delay

        roundRunning = true;
        vel = new Vector2(serveRight ? 1f : -1f, Random.Range(-0.35f, 0.35f)).normalized * speed; // serve
    }
}