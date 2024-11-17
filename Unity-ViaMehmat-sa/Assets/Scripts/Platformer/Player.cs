using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float jumpCooldown;
    public int hitPoints;
    public int coins;
    public GameObject bomb;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody2D;
    private GroundCollider groundCollider;

    private bool canJump;
    private float jumpTimer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        groundCollider = GetComponentInChildren<GroundCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float movement_x = Input.GetAxis("Horizontal");
        float movement_y = Input.GetAxis("Vertical");
        transform.position += new Vector3(movement_x, 0, 0) * Time.deltaTime * moveSpeed;

        if (movement_x > 0)
        {
            spriteRenderer.flipX = false;
        }
        if (movement_x < 0)
        {
            spriteRenderer.flipX = true;
        }

        if (movement_x != 0 && groundCollider.OnGround())
        {
            animator.SetInteger("State", 1);
        }

        if (movement_x == 0 && groundCollider.OnGround())
        {
            animator.SetInteger("State", 0);
        }

        if (movement_y > 0)
        {
            if (groundCollider.OnGround() && canJump)
            {
                rigidBody2D.AddForce(Vector2.up * jumpForce);
                canJump = false;
                jumpTimer = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            GameObject newBomb = Instantiate(bomb);
            newBomb.transform.position = transform.position;
            if (spriteRenderer.flipX)
            {
                newBomb.GetComponent<Bomb>().SetDirection(-1);
            }

        }

        jumpTimer += Time.deltaTime;

        if (jumpTimer > jumpCooldown)
        {
            canJump = true;
        }
    }

    public void OnDestroy()
    {
        Debug.Log("Player dead");
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void HitPlayer(int damage)
    {
        animator.Play("Hit");
        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }    
    }    

    public void AddCoins(int number)
    {
        coins += number;
    }
}
