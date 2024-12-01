using UnityEngine;

public class Crow : MonoBehaviour
{
    public float flapForce = 5f; // Сила, с которой ворона взлетает
    public float forwardSpeed = 2f; // Скорость движения вперед
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Получаем компонент Animator
    }

    void Update()
    {
        // Проверяем, нажата ли клавиша пробела или ПКМ
        if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            Flap();
        }

        // Двигаем вороны вперед
        transform.Translate(Vector2.right * forwardSpeed * Time.deltaTime);
    }

    void Flap()
    {
        rb.velocity = Vector2.up * flapForce; // Применяем силу к вороне
        animator.SetTrigger("Fly"); // Запускаем анимацию взлета
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, столкнулася ли ворона с объектом DeadZone
        if (other.CompareTag("DeadZone"))
        {
            Debug.Log("Game Over!"); // Логируем окончание игры
            FindObjectOfType<DeadZone>().ShowRestartMenu(); // Вызываем метод рестарта игры
        }
    }
}