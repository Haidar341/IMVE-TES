using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Kecepatan bergerak
    public float mouseSensitivity = 100f; // Sensitivitas mouse
    public Transform cameraTransform; // Transform kamera
    public float jumpHeight = 2f; // Tinggi lompatan
    public float gravity = -9.81f; // Gaya gravitasi

    public Transform groundCheck; // Posisi untuk memeriksa tanah
    public float groundDistance = 0.4f; // Jarak untuk mendeteksi tanah
    public LayerMask groundMask; // Layer untuk tanah

    private float xRotation = 0f; // Rotasi vertikal kamera
    private Vector3 velocity; // Kecepatan vertikal (gravitasi/lompatan)
    private bool isGrounded; // Apakah pemain berada di tanah

    private CharacterController characterController;

    // Clamp untuk rotasi vertikal
    public float topClamp = -45f; // Batas rotasi ke atas
    public float bottomClamp = 45f; // Batas rotasi ke bawah

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Cek apakah pemain berada di tanah
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Tetap menempel di tanah
        }

        // Pergerakan player menggunakan WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Rotasi kamera dengan mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Membatasi rotasi vertikal kamera menggunakan topClamp dan bottomClamp
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Mengatur rotasi kamera dan player
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Lompatan
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravitasi
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
