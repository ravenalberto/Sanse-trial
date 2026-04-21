using UnityEngine;

public class CameraFllow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The Player/Person object
    public Vector3 eyeOffset = new Vector3(0, 1.6f, 0); // Position of the eyes relative to feet

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public float smoothSpeed = 20f; // Higher for responsive FPV

    [Header("Rotation Constraints")]
    public float minPitch = -80f; // Look down limit
    public float maxPitch = 80f;  // Look up limit

    float yaw = 0f;
    float pitch = 0f;

    void Start()
    {
        // Lock cursor to center of screen for FPV feels
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize angles based on target's starting rotation
        if (target != null)
        {
            yaw = target.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Get Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Calculate Rotation
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 3. Rotate the Player Body (Yaw only)
        // This ensures the person walks in the direction they are looking
        target.rotation = Quaternion.Lerp(
            target.rotation,
            Quaternion.Euler(0, yaw, 0),
            smoothSpeed * Time.deltaTime
        );

        // 4. Update Camera Position (Eye Level)
        // Move camera to the target's "eyes"
        transform.position = target.position + eyeOffset;

        // 5. Update Camera Rotation (Pitch + Yaw)
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}