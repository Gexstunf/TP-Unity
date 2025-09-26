using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;

    [Header("Animación")]
    [SerializeField] Animator anim;

    [Header("Cámara")]
    [SerializeField] Transform cameraTransform; // arrastra aquí la cámara en el inspector

    Rigidbody rb;
    Vector3 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // evita que se caiga
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Dirección en base a cámara
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0; // quitar inclinación vertical
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveInput = (camForward * v + camRight * h).normalized;

        // Animación
        float speed = moveInput.magnitude;
        if (anim != null)
            anim.SetFloat("Speed", speed);
    }

    void FixedUpdate()
    {
        Vector3 moveVelocity = moveInput * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
