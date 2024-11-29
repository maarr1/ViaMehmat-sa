using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartMenuManagerFB : MonoBehaviour
{
    public GameObject restartMenu; // Ссылка на панель меню перезапуска

    // Метод для отображения меню перезапуска
    public void ShowRestartMenu()
    {
        restartMenu.SetActive(true); // Активируем меню перезапуска
        Time.timeScale = 0; // Останавливаем время в игре
    }

    // Метод для перезапуска игры
    public void RestartGame()
    {
        Time.timeScale = 1; // Возвращаем время в игру
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезагружаем текущую сцену
    }

    // Метод для выхода из игры
    public void QuitGame()
    {
        Application.Quit(); // Выход из игры
    }
}
