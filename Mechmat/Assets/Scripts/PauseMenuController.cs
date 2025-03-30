using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public Button resumeButton;      // Кнопка "Продолжить"
    public Button mainMenuButton;    // Кнопка "Выйти в Главное Меню"
    public Button exitButton;        // Кнопка "Выйти из Игры"

    void Start()
    {
        // Проверяем, назначены ли кнопки
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }
        else
        {
            Debug.LogError("Resume Button не назначена в инспекторе.");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
        else
        {
            Debug.LogError("Main Menu Button не назначена в инспекторе.");
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

    // Метод вызывается при нажатии на кнопку "Продолжить"
    void OnResumeButtonClicked()
    {
        Debug.Log("Нажата кнопка 'Продолжить'. Возобновление игры.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        else
        {
            Debug.LogError("GameManager не найден.");
        }
    }

    // Метод вызывается при нажатии на кнопку "Выйти в Главное Меню"
    void OnMainMenuButtonClicked()
    {
        Debug.Log("Нажата кнопка 'Выйти в Главное Меню'. Загрузка главного меню.");
        if (GameManager.Instance != null)
        {
            // Очищаем историю сцен и загружаем главное меню
            GameManager.Instance.ClearSceneHistory();
            GameManager.Instance.Transition("MainMenu", "");
        }
        else
        {
            Debug.LogError("GameManager не найден.");
        }
    }

    // Метод вызывается при нажатии на кнопку "Выйти из Игры"
    void OnExitButtonClicked()
    {
        Debug.Log("Нажата кнопка 'Выйти из Игры'. Закрытие приложения.");
        Application.Quit();

        // Для редактора Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
