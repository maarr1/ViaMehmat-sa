using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Название текущей сцены
    private string currentScene;

    // Название целевой сцены и спавн-поинта
    private string targetScene;
    private string spawnPointName;

    // Стек для хранения истории сцен и позиций персонажа
    private List<SceneData> sceneHistory = new List<SceneData>();
    private const int maxHistorySize = 5;

    // Переменная для хранения данных о сцене при возврате
    private SceneData returnSceneData;

    // Флаг, указывающий, что игра находится на паузе (в меню)
    private bool isPaused = false;

    private void Awake()
    {
        // Реализуем паттерн Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Подписываемся на событие загрузки сцены
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Сохраняем текущую сцену
            currentScene = SceneManager.GetActiveScene().name;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Обработка нажатия клавиши Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                // Если игра не на паузе, ставим на паузу и переходим в меню
                PauseGame();
            }
            else
            {
                // Если игра на паузе, возвращаемся в игру
                ResumeGame();
            }
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события, чтобы избежать утечек памяти
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// Метод для постановки игры на паузу и перехода в меню.
    /// </summary>
    public void PauseGame()
    {
        Debug.Log("Пауза игры. Переход в меню.");

        // Сохраняем текущую сцену и позицию персонажа перед переходом
        SaveCurrentSceneData();

        // Устанавливаем флаг паузы
        isPaused = true;

        // Загружаем сцену меню
        targetScene = "PauseMenu"; // Название сцены с меню паузы
        spawnPointName = "";

        SceneManager.LoadScene(targetScene);
    }

    /// <summary>
    /// Метод для возобновления игры и возврата к сохранённой сцене.
    /// </summary>
    public void ResumeGame()
    {
        if (returnSceneData != null)
        {
            Debug.Log("Возобновление игры. Возврат к сцене: " + returnSceneData.sceneName);

            // Устанавливаем целевую сцену и очищаем spawnPointName
            targetScene = returnSceneData.sceneName;
            spawnPointName = "";

            // Загружаем сохранённую сцену
            SceneManager.LoadScene(targetScene);

            // Сбрасываем флаг паузы
            isPaused = false;
        }
        else
        {
            Debug.LogWarning("Нет сохранённых данных для возобновления игры.");
        }
    }

    /// <summary>
    /// Метод для выполнения перехода в новую сцену.
    /// </summary>
    /// <param name="sceneName">Название целевой сцены.</param>
    /// <param name="spawnName">Имя спавн-поинта в целевой сцене (оставьте пустым, если нет спавна).</param>
    public void Transition(string sceneName, string spawnName)
    {
        // Сохраняем текущую сцену и позицию персонажа перед переходом
        SaveCurrentSceneData();

        // Устанавливаем целевую сцену и спавн-поинт
        targetScene = sceneName;
        spawnPointName = spawnName;

        // Загружаем новую сцену
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Метод для возврата на предыдущую сцену.
    /// </summary>
    public void ReturnToPreviousScene()
    {
        if (sceneHistory.Count > 0)
        {
            // Извлекаем последнюю запись из истории
            returnSceneData = sceneHistory[sceneHistory.Count - 1];

            // Удаляем её из истории
            sceneHistory.RemoveAt(sceneHistory.Count - 1);

            // Устанавливаем целевую сцену и очищаем spawnPointName
            targetScene = returnSceneData.sceneName;
            spawnPointName = "";

            // Загружаем сцену
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("История сцен пуста. Возврат невозможен.");
        }
    }

    // Обработчик события загрузки сцены
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;

        // Проверяем, есть ли персонаж в сцене
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            // Если мы возвращаемся на сцену и есть сохранённые данные
            if (returnSceneData != null && currentScene == returnSceneData.sceneName)
            {
                // Восстанавливаем позицию персонажа
                playerObj.transform.position = returnSceneData.playerPosition;
                Debug.Log($"Персонаж восстановил позицию ({returnSceneData.playerPosition}) в сцене '{currentScene}'.");

                // Обнуляем returnSceneData после использования
                returnSceneData = null;
            }
            else if (!string.IsNullOrWhiteSpace(spawnPointName))
            {
                // Устанавливаем позицию персонажа на спавн-поинт
                SetPlayerPosition(playerObj.transform);
            }
            else
            {
                // Если spawnPointName пуст или состоит только из пробелов, оставляем персонажа на его текущей позиции
                Debug.Log($"Персонаж остаётся на текущей позиции в сцене '{currentScene}'.");
            }

            // Обновляем ссылку камеры на персонажа
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.SetPlayer(playerObj.transform);
                Debug.Log("CameraController обновил ссылку на персонажа.");
            }
            else
            {
                Debug.LogError("CameraController не найден на Main Camera.");
            }
        }
        else
        {
            Debug.Log("Персонаж не найден в сцене. Предполагается, что это сцена без персонажа.");
        }
    }



    /// <summary>
    /// Метод для установки позиции персонажа на спавн-поинт.
    /// </summary>
    /// <param name="player">Трансформ персонажа.</param>
    public void SetPlayerPosition(Transform player)
    {
        // Находим объект спавна по имени
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            player.position = spawnPoint.transform.position;
            Debug.Log($"Персонаж телепортирован на спавн-поинт '{spawnPointName}' в сцене '{targetScene}'.");
        }
        else
        {
            Debug.LogError($"Спавн-поинт '{spawnPointName}' не найден в сцене '{targetScene}'.");
        }
    }

    /// <summary>
    /// Метод для сохранения текущей сцены и позиции персонажа перед переходом.
    /// </summary>
    private void SaveCurrentSceneData()
    {
        // Находим персонажа в текущей сцене
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Vector3 currentPosition = playerObj.transform.position;

            // Создаём новый объект SceneData
            returnSceneData = new SceneData(currentScene, spawnPointName, currentPosition);

            // Добавляем его в историю
            sceneHistory.Add(returnSceneData);

            // Ограничиваем размер истории
            if (sceneHistory.Count > maxHistorySize)
            {
                // Удаляем самый старый элемент (первый в списке)
                sceneHistory.RemoveAt(0);
            }

            Debug.Log($"Сцена '{currentScene}' сохранена в истории. Текущий размер истории: {sceneHistory.Count}");
        }
        else
        {
            Debug.LogWarning("Персонаж не найден в текущей сцене. Позиция не сохранена.");
        }
    }

    /// <summary>
    /// Метод для очистки истории сцен.
    /// </summary>
    public void ClearSceneHistory()
    {
        sceneHistory.Clear();
        returnSceneData = null;
        isPaused = false;
        Debug.Log("История сцен очищена.");
    }

}

/// <summary>
/// Класс для хранения данных о сцене и позиции персонажа.
/// </summary>
[System.Serializable]
public class SceneData
{
    public string sceneName;
    public string spawnPointName;
    public Vector3 playerPosition;

    public SceneData(string sceneName, string spawnPointName, Vector3 playerPosition)
    {
        this.sceneName = sceneName;
        this.spawnPointName = spawnPointName;
        this.playerPosition = playerPosition;
    }
}
