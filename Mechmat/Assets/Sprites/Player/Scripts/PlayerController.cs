using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Скорость движения персонажа по оси X")]
    public float speed = 7f;

    [Header("Boundary Settings")]
    [Tooltip("Ссылка на объект, определяющий границы игры")]
    public GameBoundary gameBoundary;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isMoving = false;

    // Переменная для хранения исходного масштаба
    private Vector3 originalScale;

    // Ссылка на CapsuleCollider2D
    private CapsuleCollider2D capsuleCollider;

    // Список ближайших интерактивных объектов
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();

    void Start()
    {
        // Получаем компоненты Animator и Rigidbody2D, прикреплённые к персонажу
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Получаем CapsuleCollider2D
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider == null)
        {
            Debug.LogError($"CapsuleCollider2D не найден на объекте {gameObject.name}.");
        }

        // Сохраняем исходный масштаб персонажа
        originalScale = transform.localScale;

        // Проверка на наличие компонентов
        if (animator == null)
        {
            Debug.LogError($"Компонент Animator не найден на объекте {gameObject.name}.");
        }

        if (rb == null)
        {
            Debug.LogError($"Компонент Rigidbody2D не найден на объекте {gameObject.name}.");
        }

        // Проверка на наличие GameBoundary
        if (gameBoundary == null)
        {
            Debug.LogError($"GameBoundary не назначен в PlayerController.");
        }
    }

    void Update()
    {
        // Получаем ввод пользователя по оси X (клавиши A/D или стрелки влево/вправо)
        float inputX = Input.GetAxisRaw("Horizontal");
        movement = new Vector2(inputX, 0f);

        // Определяем, движется ли персонаж
        isMoving = Mathf.Abs(inputX) > 0.1f;

        // Обновляем параметр Animator
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }

        // Поворачиваем персонажа в зависимости от направления движения
        if (inputX > 0)
        {
            // Движение вправо: инвертируем масштаб по X для поворота
            transform.localScale = new Vector3(originalScale.x * -1, originalScale.y, originalScale.z);
        }
        else if (inputX < 0)
        {
            // Движение влево: устанавливаем нормальный масштаб по X
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        }

        // Проверка на нажатие кнопки взаимодействия (например, E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractWithNearest();
        }
    }

    void FixedUpdate()
    {
        if (gameBoundary == null || capsuleCollider == null)
            return;

        // Получаем границы из GameBoundary
        Bounds bounds = gameBoundary.GetBounds();

        // Получаем размеры коллайдера
        Vector2 colliderSize = capsuleCollider.size;
        // Учёт масштаба персонажа
        Vector2 scaledColliderSize = Vector2.Scale(colliderSize, new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y)));

        // Рассчитываем новое положение персонажа
        Vector2 newPosition = rb.position + movement * speed * Time.fixedDeltaTime;

        // Ограничиваем позицию персонажа по осям X и Y на основе границ и размеров коллайдера
        float clampedX = Mathf.Clamp(newPosition.x, bounds.min.x + scaledColliderSize.x / 2, bounds.max.x - scaledColliderSize.x / 2);
        float clampedY = Mathf.Clamp(newPosition.y, bounds.min.y + scaledColliderSize.y / 2, bounds.max.y - scaledColliderSize.y / 2);

        newPosition = new Vector2(clampedX, clampedY);

        // Перемещаем персонажа
        rb.MovePosition(newPosition);
    }

    // Метод для взаимодействия с ближайшим интерактивным объектом
    private void InteractWithNearest()
    {
        if (nearbyInteractables.Count > 0)
        {
            // Для простоты взаимодействуем с первым в списке
            IInteractable interactable = nearbyInteractables[0];
            interactable.Interact();
            Debug.Log($"Взаимодействие с объектом типа {interactable.GetType().Name}.");
        }
        else
        {
            Debug.Log("Нет объектов для взаимодействия.");
        }
    }

    // Добавление объекта в список при входе в триггер
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Door") || other.CompareTag("Interactable"))
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null && !nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Add(interactable);
                Debug.Log($"Добавлен объект для взаимодействия: {other.gameObject.name}.");
            }
        }
    }

    // Удаление объекта из списка при выходе из триггера
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Door") || other.CompareTag("Interactable"))
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null && nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Remove(interactable);
                Debug.Log($"Удалён объект для взаимодействия: {other.gameObject.name}.");
            }
        }
    }
}
