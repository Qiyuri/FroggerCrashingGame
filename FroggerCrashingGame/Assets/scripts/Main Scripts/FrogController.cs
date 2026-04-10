using UnityEngine;
using UnityEngine.SceneManagement;

public class FrogController : MonoBehaviour
{
    public float speed = 2f;
    public float moveRange = 5f; // How far the frog can move from spawn
    // public ScoreSystem scoreSystem; // Reference to the ScoreSystem - now found automatically

    private Vector3 startPosition;
    private ScoreSystem scoreSystem; // Cached reference

    void Start()
    {
        startPosition = transform.position;

        // Find the ScoreSystem automatically
        scoreSystem = FindFirstObjectByType<ScoreSystem>();
        if (scoreSystem == null)
        {
            Debug.LogWarning("ScoreSystem not found in the scene! Make sure there's a GameObject with the ScoreSystem component.");
        }
    }

    void Update()
    {
        // Walk forward continuously
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
                Debug.LogError("ScoreSystem not found! Make sure there's a GameObject with the ScoreSystem component in the scene.");
            }
        }
    }
}