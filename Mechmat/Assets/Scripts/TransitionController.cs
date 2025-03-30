using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("Название целевой сцены для перехода")]
    public string targetSceneName;

    [Tooltip("Имя спавн-поинта в целевой сцене (может быть пустым, если спавн не требуется)")]
    public string spawnPointName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (string.IsNullOrWhiteSpace(spawnPointName))
            {
                Debug.Log($"Персонаж вошёл в зону перехода на сцену '{targetSceneName}', спавн-поинт не задан.");
            }
            else
            {
                Debug.Log($"Персонаж вошёл в зону перехода на сцену '{targetSceneName}' с спавн-поинтом '{spawnPointName}'.");
            }

            // Инициализируем переход через GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Transition(targetSceneName, spawnPointName);
            }
            else
            {
                Debug.LogError("GameManager не найден. Убедитесь, что объект GameManager присутствует в сцене.");
            }
        }
    }
}
