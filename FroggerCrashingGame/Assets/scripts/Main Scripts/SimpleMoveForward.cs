using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
public class RigidbodyMoveForward : MonoBehaviour
{
    public float speed = 5f; // Instelbare snelheid

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Zorg dat Rigidbody niet door physics wordt beïnvloed door rotatie (optioneel)
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        // Beweeg het object vooruit met Rigidbody
        Vector3 forwardMovement = transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMovement);
    }
}
