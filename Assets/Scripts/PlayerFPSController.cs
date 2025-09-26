using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFPSController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    public float maxLookX = 60f;
    public float minLookX = -60f;

    [Header("Cámara")]
    public Camera playerCamera;
    public float cameraHeight = 1.7f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private float rotationX = 0f; // Para rotación vertical de la cámara

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Evita que el cuerpo caiga rotando

        if (playerCamera == null)
            playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Mouse look ---
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Rotación horizontal del jugador
        transform.Rotate(Vector3.up * mouseX);

        // Rotación vertical de la cámara
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);
        playerCamera.transform.localEulerAngles = new Vector3(rotationX, 0, 0);

        // Posicionar la cámara a la altura del jugador
        playerCamera.transform.position = transform.position + Vector3.up * cameraHeight;

        // --- Movimiento ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = transform.forward * v + transform.right * h;
        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        // Mover al jugador con Rigidbody
        Vector3 moveVelocity = moveInput * moveSpeed;
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }
}
