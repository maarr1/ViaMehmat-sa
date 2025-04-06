using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
	[SerializeField] private float speed = 3f;
	[SerializeField] private int lives = 2;
	[SerializeField] private float jumpForce = 3f;
	private bool isGrounded = false;
   
    private Rigidbody2D rb;
	private SpriteRenderer sprite;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		sprite = rb.GetComponentInChildren<SpriteRenderer>();
	}
    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
		if (Input.GetButton("Horizontal")) Run();

		if (isGrounded && Input.GetButton("Jump")) Jump();
    }

    private void Run()
	{
		Vector3 dir = transform.right * Input.GetAxis("Horizontal");
		transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

		sprite.flipX = dir.x > 0.0f; //реверсия, тк спрайты смотрят влево
	}

	private void Jump()
	{
		rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);	
	}

	private void CheckGround()
	{
		Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
		isGrounded = collider.Length > 1;
	}
}