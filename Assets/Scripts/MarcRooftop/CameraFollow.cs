using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;

	public Vector3 targetOffset = new Vector3(0, 1.5f, 0);

	public float distance = 6f;
	public float height = 0.5f;

	public float mouseSensitivity = 100f;
	public float smoothSpeed = 5f;

	float yaw = 0f;
	float pitch = 20f;

	public float minPitch = 10f;
	public float maxPitch = 50f;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void LateUpdate()
	{
		if (target == null) return;

		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		yaw += mouseX;
		pitch -= mouseY;

		pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

		// 👉 ONLY rotate horizontally for position
		Quaternion yawRotation = Quaternion.Euler(0, yaw, 0);

		Vector3 direction = yawRotation * Vector3.back;

		Vector3 desiredPosition = target.position + direction * distance + Vector3.up * height;

		transform.position = Vector3.Lerp(
			transform.position,
			desiredPosition,
			smoothSpeed * Time.deltaTime
		);

		// 👉 Apply pitch ONLY to where we look
		Quaternion lookRotation = Quaternion.Euler(pitch, yaw, 0);
		Vector3 lookTarget = target.position + targetOffset;
		transform.LookAt(lookTarget);
	}
}