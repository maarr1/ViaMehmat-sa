using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverMenu; // Ссылка на меню окончания игры

    public void EndGame()
    {
        FindObjectOfType<RestartMenuManagerFB>().ShowRestartMenu(); // Показываем меню перезапуска
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Сбрасываем время
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезапускаем текущую сцену
    }

    public void QuitGame()
    {
        Application.Quit(); // Выход из игры
    }
}
