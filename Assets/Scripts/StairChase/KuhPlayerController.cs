using UnityEngine;

public class KuhPlayerController : MonoBehaviour
{
	public float speed = 0;

	private Rigidbody2D rb;
	private Vector2 movement;

	private SpriteRenderer sr;

	public Sprite frontSprite;
	public Sprite sideSprite;
	public Sprite backSprite;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");

		// ❌ REMOVE DIAGONALS
		if (movement.x != 0)
			movement.y = 0;

		UpdateSpriteDirection();
	}

	void FixedUpdate()
	{
		rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
	}

	void UpdateSpriteDirection()
	{
		if (movement == Vector2.zero) return;

		if (movement.x > 0)
		{
			sr.sprite = sideSprite;
			sr.flipX = false;
		}
		else if (movement.x < 0)
		{
			sr.sprite = sideSprite;
			sr.flipX = true;
		}
		else if (movement.y > 0)
		{
			sr.sprite = backSprite;
		}
		else if (movement.y < 0)
		{
			sr.sprite = frontSprite;
		}
	}
}