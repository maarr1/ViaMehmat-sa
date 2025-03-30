using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button startButton; // Ссылка на кнопку "Старт"
    public Button exitButton;  // Ссылка на кнопку "Выход"

    [Header("Start Scene Settings")]
    [Tooltip("Название сцены, которую нужно загрузить при нажатии на 'Старт'.")]
    public string startSceneName = "MainScene"; // Название сцены для начала игры

    [Tooltip("Имя спавн-поинта в целевой сцене.")]
    public string spawnPointName = "SpawnPoint_Main"; // Имя спавн-поинта

    void Start()
    {
        // Проверяем, назначены ли кнопки
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogError("Start Button не назначена в инспекторе.");
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        else
        {
            Debug.LogError("Exit Button не назначена в инспекторе.");
        }
    }

    // Метод вызывается при нажатии на кнопку "Старт"
    public void OnStartButtonClicked()
    {
        Debug.Log("Нажата кнопка 'Старт'. Загрузка сцены: " + startSceneName);
        if (GameManager.Instance != null)
        {
            // Очищаем историю сцен перед началом новой игры
            GameManager.Instance.ClearSceneHistory();
            GameManager.Instance.Transition(startSceneName, spawnPointName);
        }
        else
        {
            Debug.LogError("GameManager не найден.");
        }
    }

    // Метод вызывается при нажатии на кнопку "Выход"
    void OnExitButtonClicked()
    {
        Debug.Log("Нажата кнопка 'Выход'. Завершение игры.");
        Application.Quit();

        // Для редактора Unity (работает только в сборке)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
