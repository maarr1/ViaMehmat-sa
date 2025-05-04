// FlappyController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FlappyController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;

    [Header("Rotation Settings")]
    [Tooltip("Максимальный угол наклона вверх")]
    public float maxUpAngle = 45f;
    [Tooltip("Максимальный угол наклона вниз")]
    public float maxDownAngle = -90f;
    [Tooltip("Скорость поворота")]
    public float rotateSpeed = 5f;

    [Header("Start Position")]
    [Tooltip("Отступ от центра камеры по X")]
    public float startXOffset = 6f;

    [Header("Game Logic")]
    public InfiniteLevel levelManager;

    [Header("Death Bounds")]
    public float topMargin = 0.5f;
    public float bottomMargin = 0.5f;

    [Header("Start GIF")]
    public GameObject startGif;

    private Rigidbody2D rb;
    private Animator animator;
    private Camera cam;
    private bool hasStarted = false;
    private bool isDead = false;
    private float startX;
    private int scrollDir = -1;  // -1 = птица справа (движ влево), +1 = птица слева

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cam = Camera.main;

        rb.isKinematic = true;
        if (startGif != null) startGif.SetActive(true);

        // изначально держим по fixed X
        startX = cam.transform.position.x + startXOffset * scrollDir;
        SetXPosition();
    }

    public void SetScrollDirection(InfiniteLevel.ScrollDir dir)
    {
        scrollDir = -(int)dir;
        // пересчёт стартовой X
        startX = cam.transform.position.x - startXOffset * scrollDir;
        // зеркалирование спрайта
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * scrollDir;
        transform.localScale = s;
        // тут же зафиксируем позицию
        SetXPosition();
    }

    void Update()
    {
        // поддерживаем фиксированную X до и после старта
        SetXPosition();

        if (!hasStarted)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                hasStarted = true;
                rb.isKinematic = false;
                levelManager?.StartGame();
                DoJump();
                if (startGif != null) startGif.SetActive(false);
            }
            return;
        }

        if (isDead) return;

        // прыжок
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            DoJump();

        // расчёт базового угла
        float t = Mathf.InverseLerp(-5f, jumpForce, rb.linearVelocity.y);
        float baseAngle = Mathf.Lerp(maxDownAngle, maxUpAngle, t);
        // инвертируем его при scrollDir = +1 (движение вправо)
        float targetAngle = baseAngle * scrollDir;

        float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle,
                                      Time.deltaTime * rotateSpeed);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // проверка выхода за пределы по Y
        float camTop = cam.transform.position.y + cam.orthographicSize + topMargin;
        float camBot = cam.transform.position.y - cam.orthographicSize - bottomMargin;
        if (transform.position.y > camTop || transform.position.y < camBot)
            Die();
    }

    private void SetXPosition()
    {
        Vector3 p = transform.position;
        p.x = startX;
        transform.position = p;
    }

    private void DoJump()
    {
        rb.linearVelocity = new Vector2(0f, jumpForce);
        transform.rotation = Quaternion.Euler(0f, 0f, maxUpAngle * -scrollDir);
        animator.SetTrigger("DoFlap");
    }

    private void OnCollisionEnter2D(Collision2D _)
    {
        Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        animator.SetTrigger("Die");
        levelManager?.Lose();
    }
}
