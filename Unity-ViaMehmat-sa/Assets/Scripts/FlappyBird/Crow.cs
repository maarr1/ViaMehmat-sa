using UnityEngine;

public class Crow : MonoBehaviour
{
    public float flapForce = 5f; // Сила, с которой воробей взлетает
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
        if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(1))
        {
            Flap();
        }
    }

    void Flap()
    {
        rb.velocity = Vector2.up * flapForce; // Применяем силу к воробью
        animator.SetTrigger("Fly"); // Запускаем анимацию взлета
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Здесь можно обработать логику, когда воробей сталкивается с препятствием
        Debug.Log("Game Over!");
        FindObjectOfType<GameManager>().EndGame(); // Вызываем метод завершения игры
    }
}