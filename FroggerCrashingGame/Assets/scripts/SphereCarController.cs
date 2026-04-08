using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereCarController : MonoBehaviour
{
    [Header("Speed & Acceleration")]
    public float maxSpeed = 18f;
    public float acceleration = 40f;
    public float reverseAcceleration = 28f;

    [Header("Steering")]
    public float steerStrength = 8f;
    public float lowSpeedSteerBoost = 1.6f;
    public float highSpeedSteerReduction = 0.65f;

    [Header("Braking / Drag")]
    public float groundDrag = 1.2f;
    public float airDrag = 0.05f;
    public float brakeDrag = 6.5f;

    [Header("Grip / Drift")]
    public float lateralGrip = 10f;
    public float handbrakeGripMultiplier = 0.35f;
    public float handbrakeDriftForce = 15f;

    [Header("Grounding")]
    public float groundRayLength = 0.65f;
    public LayerMask groundMask = ~0;

    [Header("Optional Visual Body")]
    public Transform visualBody;
    public float visualTurnSmoothing = 10f;

    private Rigidbody rb;

    private float throttle;
    private float steer;
    private bool brake;
    private bool handbrake;

    private bool isGrounded;
    private Vector3 groundNormal = Vector3.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        // Beperk rotatie op x- en z-as om kantelen te voorkomen
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // Input ophalen
        throttle = Input.GetAxisRaw("Vertical");   // W/S of pijltjestoetsen omhoog/omlaag
        steer = Input.GetAxisRaw("Horizontal");    // A/D of pijltjestoetsen links/rechts
        brake = Input.GetKey(KeyCode.Space);
        handbrake = Input.GetKey(KeyCode.LeftShift); // Handrem met linker Shift
    }

    void FixedUpdate()
    {
        GroundCheck();

        // Pas drag aan afhankelijk van grondcontact en remmen
        rb.linearDamping = isGrounded ? (brake ? brakeDrag : groundDrag) : airDrag;

        Vector3 velocity = rb.linearVelocity;
        float speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;

        // Voorwaartse richting geprojecteerd op grondvlak
        Vector3 forwardOnGround = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        if (forwardOnGround.sqrMagnitude < 0.001f) forwardOnGround = transform.forward;

        // Acceleratie en snelheidslimiet
        if (isGrounded)
        {
            float accel = throttle >= 0f ? acceleration : reverseAcceleration;

            float forwardSpeed = Vector3.Dot(rb.linearVelocity, forwardOnGround);
            bool canAccelerateForward = (throttle > 0f && forwardSpeed < maxSpeed) ||
                                        (throttle < 0f && forwardSpeed > -maxSpeed * 0.6f);

            if (Mathf.Abs(throttle) > 0.01f && canAccelerateForward)
            {
                Vector3 force = forwardOnGround * (throttle * accel);
                force.y = 0f; // Zorg dat kracht horizontaal is
                rb.AddForce(force, ForceMode.Acceleration);
            }
        }
        else
        {
            // Kleine luchtcontrole
            Vector3 airForce = transform.forward * (throttle * (acceleration * 0.25f));
            airForce.y = 0f;
            rb.AddForce(airForce, ForceMode.Acceleration);
        }

        // Sturen
        if (isGrounded)
        {
            float speed01 = Mathf.Clamp01(speed / Mathf.Max(0.01f, maxSpeed));
            float steerScale = Mathf.Lerp(lowSpeedSteerBoost, highSpeedSteerReduction, speed01);

            float steerAmount = steer * steerStrength * steerScale * Mathf.Clamp(speed * 0.25f, 0f, 1f);

            if (Mathf.Abs(steerAmount) > 0.0001f)
            {
                Quaternion turn = Quaternion.AngleAxis(steerAmount, groundNormal);
                Vector3 newVel = turn * rb.linearVelocity;
                newVel.y = rb.linearVelocity.y; // behoud verticale snelheid
                rb.linearVelocity = newVel;

                // Auto alleen om y-as draaien, voorkom kantelen
                Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                if (flatVel.sqrMagnitude > 0.1f)
                {
                    Vector3 flatForward = flatVel.normalized;
                    Quaternion targetRot = Quaternion.LookRotation(flatForward, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 5f);
                }
            }
        }

        // Zijdelingse grip (vermindert glijden)
        if (isGrounded)
        {
            Vector3 rightOnGround = Vector3.Cross(groundNormal, forwardOnGround).normalized;
            float lateralSpeed = Vector3.Dot(rb.linearVelocity, rightOnGround);

            float grip = lateralGrip * (brake ? handbrakeGripMultiplier : 1f);

            Vector3 lateralVel = rightOnGround * lateralSpeed;
            Vector3 gripForce = -lateralVel * grip;
            gripForce.y = 0f;
            rb.AddForce(gripForce, ForceMode.Acceleration);

            // Handrem driftkracht in stuurrichting
            if (handbrake && Mathf.Abs(steer) > 0.1f)
            {
                Vector3 driftDirection = rightOnGround * Mathf.Sign(steer);
                Vector3 driftForce = driftDirection * handbrakeDriftForce;
                driftForce.y = 0f;
                rb.AddForce(driftForce, ForceMode.Acceleration);
            }
        }

        // Beperk horizontale snelheid tot maxSpeed
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }

        // Visuele rotatie van de auto
        if (visualBody != null)
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.sqrMagnitude > 0.2f)
            {
                Quaternion targetRot = Quaternion.LookRotation(flatVel.normalized, Vector3.up);
                visualBody.rotation = Quaternion.Slerp(visualBody.rotation, targetRot, Time.fixedDeltaTime * visualTurnSmoothing);
            }
        }

        // Stabiliseer rotatie om kantelen te voorkomen
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x = 0f;
        euler.z = 0f;
        transform.rotation = Quaternion.Euler(euler);
    }

    private void GroundCheck()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, groundRayLength, groundMask, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
            groundNormal = hit.normal;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
    }
}
