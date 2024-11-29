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
  
    // Вызывается перед первым кадром
    void Start()
    {
        animator = GetComponent<Animator>(); // Получение компонента Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // Получение компонента SpriteRenderer
        rigidBody2D = GetComponent<Rigidbody2D>(); // Получение компонента Rigidbody2D
    }

    // Вызывается каждый кадр
    void Update()
    {
        float movement_x = Input.GetAxis("Horizontal"); // Получение значения горизонтального движения
        // не должно пригодится float movement_y = Input.GetAxis("Vertical"); // Получение значения вертикального движения

        // Переворачивание спрайта в зависимости от направления движения
        if (movement_x > 0)
        {
            spriteRenderer.flipX = false; // Не переворачивать спрайт
        }
        if (movement_x < 0)
        {
            spriteRenderer.flipX = true; // Перевернуть спрайт
        }

    }

    // Добавление монет к счету
    public void AddCoins(int number)
    {
        coins += number;
    }
}
