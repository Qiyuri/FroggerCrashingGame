using UnityEngine;
using UnityEngine.SceneManagement;

public class FrogController : MonoBehaviour
{
    public float speed = 2f;
    public float jumpInterval = 1f;
    public float jumpHeight = 1f;
    public float moveRange = 5f; // How far the frog can move from spawn
    public ScoreSystem scoreSystem; // Reference to the ScoreSystem

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float lastJumpTime;
    private bool isJumping = false;
    private float jumpStartY;

    void Start()
    {
        startPosition = transform.position;
        SetNewTarget();
        lastJumpTime = Time.time;
    }

    void Update()
    {
        // Jump periodically
        if (Time.time - lastJumpTime > jumpInterval && !isJumping)
        {
            StartJump();
        }

        if (isJumping)
        {
            UpdateJump();
        }
        else
        {
            // Move towards target when not jumping
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Keep on ground level
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Check if reached target
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                new Vector3(targetPosition.x, 0, targetPosition.z)) < 0.1f)
            {
                SetNewTarget();
            }
        }
    }

    void StartJump()
    {
        isJumping = true;
        jumpStartY = transform.position.y;
        lastJumpTime = Time.time;
    }

    void UpdateJump()
    {
        // Simple jump animation
        float jumpProgress = (Time.time - lastJumpTime) / 0.5f; // 0.5 second jump
        if (jumpProgress >= 1f)
        {
            isJumping = false;
            transform.position = new Vector3(transform.position.x, jumpStartY, transform.position.z);
        }
        else
        {
            float height = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(transform.position.x, jumpStartY + height, transform.position.z);
        }
    }

    void SetNewTarget()
    {
        // Pick a random target within range
        float x = startPosition.x + Random.Range(-moveRange, moveRange);
        float z = startPosition.z + Random.Range(-moveRange, moveRange);

        targetPosition = new Vector3(x, startPosition.y, z);
    }

    // Optional: Handle being hit by car or enemy
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name + " Tag: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Car"))
        {
            // Frog gets squished - you could play sound, particle effect, etc.
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy the enemy when the frog collides with it
            Destroy(collision.gameObject);
            // Increase score
            if (scoreSystem != null)
            {
                scoreSystem.AddScore(10); // Add 10 points for hitting an enemy
                Debug.Log("Score increased! New score should be updated.");
            }
            else
            {
                Debug.LogError("ScoreSystem reference is null! Make sure to assign it in the Inspector.");
            }
        }
    }
}