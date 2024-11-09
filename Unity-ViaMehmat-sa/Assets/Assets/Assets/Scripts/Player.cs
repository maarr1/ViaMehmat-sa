using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed; // Скорость движения
    public int coins; // Количество монет

    private Animator animator; // Аниматор
    private SpriteRenderer spriteRenderer; // Спрайт-рендерер
    private Rigidbody2D rigidBody2D; // Ригидбоди
    private GroundCollider groundCollider; // Коллидер для определения контакта с поверхностью

    // Вызывается перед первым кадром
    void Start()
    {
        animator = GetComponent<Animator>(); // Получение компонента Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // Получение компонента SpriteRenderer
        rigidBody2D = GetComponent<Rigidbody2D>(); // Получение компонента Rigidbody2D
        groundCollider = GetComponentInChildren<GroundCollider>(); // Получение компонента GroundCollider
    }

    // Вызывается каждый кадр
    void Update()
    {
        float movement_x = Input.GetAxis("Horizontal"); // Получение значения горизонтального движения
        // не должно пригодится float movement_y = Input.GetAxis("Vertical"); // Получение значения вертикального движения
        transform.position += new Vector3(movement_x, 0, 0) Time.deltaTime moveSpeed; // Перемещение персонажа по горизонтали

        // Переворачивание спрайта в зависимости от направления движения
        if (movement_x > 0)
        {
            spriteRenderer.flipX = false; // Не переворачивать спрайт
        }
        if (movement_x < 0)
        {
            spriteRenderer.flipX = true; // Перевернуть спрайт
        }

        // Анимация ходьбы, если персонаж на земле и двигается
        if (movement_x != 0 && groundCollider.OnGround())
        {
            animator.SetInteger("State", 1); // Запуск анимации ходьбы
        }

        // Анимация простоя, если персонаж на земле и не двигается
        if (movement_x == 0 && groundCollider.OnGround())
        {
            animator.SetInteger("State", 0); // Запуск анимации простоя
        }
    }

    // Добавление монет к счету
    public void AddCoins(int number)
    {
        coins += number;
    }
}
