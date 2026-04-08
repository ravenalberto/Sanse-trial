using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 5f;
	public Transform cameraTransform;

Animator anim;
	SpriteRenderer sr;

	// 🔥 NEW: store last direction
	float lastmoveX = 0;
	float lastmoveZ = -1; // default facing down

	void Start()
	{
		anim = GetComponent<Animator>();
		sr = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		// Movement relative to camera
		Vector3 forward = cameraTransform.forward;
		Vector3 right = cameraTransform.right;

		forward.y = 0;
		right.y = 0;

		forward.Normalize();
		right.Normalize();

		Vector3 move = forward * v + right * h;

		transform.Translate(move * speed * Time.deltaTime, Space.World);

		// 🎮 ANIMATION
		bool isMoving = h != 0 || v != 0;

		anim.SetBool("isMoving", isMoving);
		anim.SetFloat("moveX", h);
		anim.SetFloat("moveZ", v);

		// 🔥 SAVE LAST DIRECTION WHEN MOVING
		if (isMoving)
		{
			lastmoveX = h;
			lastmoveZ = v;
		}

		// 🔥 SEND LAST DIRECTION TO ANIMATOR
		anim.SetFloat("lastmoveX", lastmoveX);
		anim.SetFloat("lastmoveZ", lastmoveZ);
	}

}
