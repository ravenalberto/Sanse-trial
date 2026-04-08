using UnityEngine;

public class Billboard : MonoBehaviour
{
	public Camera mainCamera;

	void LateUpdate()
	{
		Vector3 cameraPosition = mainCamera.transform.position;

		cameraPosition.y = transform.position.y;

		transform.LookAt(cameraPosition);
		transform.Rotate(0f, 180f, 0f);
	}
}