using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [Tooltip("Уникальный номер двери")]
    public int doorID;

    [Tooltip("Дверь открывается автоматически при приближении")]
    public bool opensAutomatically = false;

    [Tooltip("Дверь может быть открыта персонажем")]
    public bool isOpenable = true;

    [Header("Teleport Settings")]
    [Tooltip("Название сцены для перехода")]
    public string targetSceneName;

    [Tooltip("Имя спавн-поинта в целевой сцене (оставьте пустым, если в сцене нет персонажа)")]
    public string spawnPointName;

    private Animator animator;
    private bool isOpen = false;
    private bool isPlayerNear = false;

    void Start()
    {
        // Получаем компонент Animator
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"Animator не найден на объекте {gameObject.name}. Убедитесь, что компонент Animator прикреплён.");
        }
        else
        {
            Debug.Log($"Animator успешно найден на двери {doorID} ({gameObject.name}).");
        }

        // Убедимся, что BoxCollider2D настроен как триггер
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null && !boxCollider.isTrigger)
        {
            boxCollider.isTrigger = true;
            Debug.LogWarning($"BoxCollider2D на двери {doorID} ({gameObject.name}) был настроен как не-триггер. Изменено на триггер.");
        }

        // Инициализация состояния двери
        SetDoorState(isOpen);
    }

    // Метод вызывается, когда другой коллайдер входит в триггер двери
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log($"Персонаж вошёл в зону двери {doorID} ({gameObject.name}).");

            if (opensAutomatically && isOpenable)
            {
                OpenDoor();
            }
        }
    }

    // Метод вызывается, когда другой коллайдер покидает триггер двери
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            Debug.Log($"Персонаж покинул зону двери {doorID} ({gameObject.name}).");

            if (opensAutomatically && isOpenable)
            {
                CloseDoor();
            }
        }
    }

    // Метод для установки состояния двери
    private void SetDoorState(bool open)
    {
        if (animator != null)
        {
            animator.SetBool("Is_open", open);
            Debug.Log($"Дверь {doorID} ({gameObject.name}) установлено состояние 'Is_open' = {open}.");
        }
        else
        {
            Debug.LogWarning($"Animator не найден на двери {doorID} ({gameObject.name}). Не удалось изменить состояние двери.");
        }
    }

    // Открытие двери
    public void OpenDoor()
    {
        if (!isOpen && isOpenable)
        {
            SetDoorState(true);
            isOpen = true;
            Debug.Log($"Дверь {doorID} ({gameObject.name}) открыта.");
        }
    }

    // Закрытие двери
    public void CloseDoor()
    {
        if (isOpen)
        {
            SetDoorState(false);
            isOpen = false;
            Debug.Log($"Дверь {doorID} ({gameObject.name}) закрыта.");
        }
    }

    // Реализация метода из интерфейса IInteractable
    public void Interact()
    {
        if (isOpenable)
        {
            if (animator != null)
            {
                if (isOpen)
                {
                    Debug.Log($"Взаимодействие с дверью {doorID} ({gameObject.name}) для перехода.");
                    TeleportPlayer();
                }
                else
                {
                    Debug.Log($"Взаимодействие с дверью {doorID} ({gameObject.name}) для открытия.");
                    ToggleDoor();
                }
            }
            else
            {
                Debug.LogWarning($"Animator не найден на двери {doorID} ({gameObject.name}). Не удалось взаимодействовать с дверью.");
            }
        }
        else
        {
            Debug.Log($"Дверь {doorID} ({gameObject.name}) не открываема. Взаимодействие невозможно.");
        }
    }

    // Метод для переключения состояния двери
    private void ToggleDoor()
    {
        if (isOpenable)
        {
            isOpen = !isOpen;
            SetDoorState(isOpen);
            Debug.Log($"Дверь {doorID} ({gameObject.name}) переключено состояние 'Is_open' = {isOpen}.");
        }
    }

    // Метод для телепортации персонажа через GameManager
    private void TeleportPlayer()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"Инициация перехода в сцену '{targetSceneName}' с спавн-поинтом '{spawnPointName}'.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Transition(targetSceneName, spawnPointName);
            }
            else
            {
                Debug.LogError("GameManager не найден. Убедитесь, что объект GameManager присутствует в сцене.");
            }
        }
        else
        {
            Debug.LogError($"Дверь {doorID} ({gameObject.name}) не настроена для перехода. Убедитесь, что 'targetSceneName' задан.");
        }
    }
}
