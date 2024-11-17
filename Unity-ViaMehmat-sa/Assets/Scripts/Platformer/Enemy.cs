using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float pointToX;
    public float moveSpeed;
    public bool canAttack;
   
    private float pointFromX;
    private float movePoint;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody2D;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        pointFromX = transform.position.x;
        movePoint = pointToX;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(movePoint, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, new Vector2(movePoint, transform.position.y)) < 0.1f)
        {
            if(movePoint == pointToX)
            {
                movePoint = pointFromX;
                spriteRenderer.flipX = false;
            }
            else
            {
                movePoint = pointToX;
                spriteRenderer.flipX = true;
            }
        }
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && canAttack)
        {
            collision.gameObject.GetComponent<Player>().HitPlayer(1);
            if(collision.gameObject.GetComponent<SpriteRenderer>().flipX == true)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 1.5f);
            }
            else
            {
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 1.5f);
            }
        }

        if (collision.gameObject.tag == "Bomb")
        {
            canAttack = false;
            Destroy(gameObject, 0.5f);
            rigidBody2D.AddForce(Vector2.up * 1);
            animator.Play("Hit");
        }
    }
}
