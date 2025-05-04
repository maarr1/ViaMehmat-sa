using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlatformerCharacter : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 40f;
    public float velPower = 0.9f;

    [Header("Jump")]
    public float jumpForce = 15f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;

    [Header("Wall Slide / Jump")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;
    public LayerMask wallLayer;
    public float wallSlideSpeedMax = 3f;
    public Vector2 wallJumpForce = new Vector2(12f, 16f);
    public float wallJumpLockTime = 0.2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Death & Victory")]
    [Tooltip("Y-координата, ниже которой персонаж умирает")]
    public float deathY = -10f;
    [Tooltip("Слой убивающих объектов (IsTrigger = true)")]
    public LayerMask deathLayer;
    [Tooltip("Слой победных объектов (IsTrigger = true)")]
    public LayerMask victoryLayer;

    // internal
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private float horizontalInput;
    private bool facingRight = true;
    private bool controlEnabled = true;

    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool isWallSliding;
    private bool wallJumping;
    private float wallJumpCounter;

    private UIManager uiManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
            Debug.LogError("UIManager не найден в сцене!");
    }

    void Update()
    {
        if (!controlEnabled) return;

        // 1) Смерть по Y
        if (transform.position.y < deathY)
        {
            OnDeath();
            return;
        }

        // 2) Чтение ввода
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 3) Coyote Time & Jump Buffer
        coyoteCounter = IsGrounded() ? coyoteTime : coyoteCounter - Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // 4) Wall Slide Check
        bool touchingWall = Physics2D.Raycast(
            wallCheck.position,
            facingRight ? Vector2.right : Vector2.left,
            wallCheckDistance,
            wallLayer
        );
        isWallSliding = !IsGrounded() && touchingWall && rb.linearVelocity.y < 0f;

        // 5) Попытка прыжка
        if (jumpBufferCounter > 0f)
        {
            if (coyoteCounter > 0f)
                DoJump();
            else if (isWallSliding)
                DoWallJump();
            jumpBufferCounter = 0f;
        }

        // 6) Анимации
        animator.SetBool("Go", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("IsGrounded", IsGrounded());
        animator.SetBool("WallSlide", isWallSliding);
    }

    void FixedUpdate()
    {
        if (!controlEnabled) return;

        // Горизонтальное движение или блокировка после wall-jump
        if (wallJumping)
        {
            wallJumpCounter -= Time.fixedDeltaTime;
            if (wallJumpCounter <= 0f)
                wallJumping = false;
        }
        else
        {
            float targetSpeed = horizontalInput * maxMoveSpeed;
            float speedDiff = targetSpeed - rb.linearVelocity.x;
            float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
            rb.AddForce(Vector2.right * movement);
        }

        // Улучшенная гравитация
        if (rb.linearVelocity.y < 0f)
            rb.gravityScale = fallMultiplier;
        else if (rb.linearVelocity.y > 0f && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow))
            rb.gravityScale = lowJumpMultiplier;
        else
            rb.gravityScale = 1f;

        // Скользение по стене
        if (isWallSliding)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeedMax, float.MaxValue));

        // Flip спрайта
        if (!wallJumping)
        {
            if (horizontalInput < 0f && !facingRight) Flip();
            else if (horizontalInput > 0f && facingRight) Flip();
        }
    }

    private void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteCounter = 0f;
    }

    private void DoWallJump()
    {
        wallJumping = true;
        wallJumpCounter = wallJumpLockTime;
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Vector2 force = new Vector2(dir.x * wallJumpForce.x, wallJumpForce.y);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        // Поворот после wall-jump
        if ((force.x > 0 && !facingRight) || (force.x < 0 && facingRight))
            Flip();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        sprite.flipX = !sprite.flipX;
        // Сдвиг точки wallCheck на другую сторону
        Vector3 p = wallCheck.localPosition;
        wallCheck.localPosition = new Vector3(-p.x, p.y, p.z);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        int layer = 1 << col.gameObject.layer;
        if ((layer & deathLayer) != 0) OnDeath();
        else if ((layer & victoryLayer) != 0) OnVictory();
    }

    private void OnDeath()
    {
        controlEnabled = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Die");
        if (uiManager != null) uiManager.ShowGameOver();
    }

    private void OnVictory()
    {
        controlEnabled = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Victory");
        if (uiManager != null) uiManager.ShowVictory();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = IsGrounded() ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                wallCheck.position,
                wallCheck.position + (facingRight ? Vector3.right : Vector3.left) * wallCheckDistance
            );
        }
        // Линия смерти по Y
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(-100f, deathY, 0f), new Vector3(100f, deathY, 0f));
    }
}
