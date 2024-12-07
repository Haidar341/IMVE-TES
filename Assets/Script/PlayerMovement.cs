using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Kecepatan bergerak
    public float mouseSensitivity = 100f; // Sensitivitas mouse
    public Transform cameraTransform; // Transform kamera

    private float xRotation = 0f; // Rotasi vertikal kamera

    void Start()
    {
        // Menyembunyikan dan mengunci kursor ke tengah layar
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Pergerakan player menggunakan WASD
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical"); // W/S

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Rotasi kamera dengan mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Membatasi rotasi vertikal kamera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Mengatur rotasi kamera dan player
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
