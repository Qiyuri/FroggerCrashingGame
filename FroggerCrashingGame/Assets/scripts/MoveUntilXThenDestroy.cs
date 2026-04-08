using UnityEngine;

public class MoveUntilXThenDestroy : MonoBehaviour
{
    public Transform targetObject;    // Het object waarvan we de x-positie als grens gebruiken
    public float speed = 5f;          // Snelheid van het bewegende object

    private float targetX;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("TargetObject is niet ingesteld.");
            enabled = false;
            return;
        }

        targetX = targetObject.position.x;
    }

    void Update()
    {
        // Beweeg het object vooruit (in de lokale z-richting)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Controleer of we de target x-positie hebben bereikt of gepasseerd
        // We vergelijken de wereldpositie x van dit object met targetX
        if (transform.position.x >= targetX)
        {
            Destroy(gameObject);
        }
    }
}
