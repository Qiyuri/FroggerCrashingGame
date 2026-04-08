using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 2f;
    public float destroyX = 15f;
    public float leftBound = -15f;
    public float rightBound = 15f;

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x > rightBound + destroyX || transform.position.x < leftBound - destroyX)
        {
            Destroy(gameObject);
        }
    }
}
