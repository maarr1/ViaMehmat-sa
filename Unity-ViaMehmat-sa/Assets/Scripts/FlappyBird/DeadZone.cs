using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки сцены
using UnityEngine.UI; // Для работы с UI

public class DeadZone : MonoBehaviour
{
    public GameObject RestartMenu; // Ссылка на меню перезапуск


    public void ShowRestartMenu()
    {
        // Открываем меню перезапуска
        Time.timeScale = 0; // Останавливаем игру
        RestartMenu.SetActive(true);
    }

    public void RestartGame()
    {
        // Сбрасываем время
        Time.timeScale = 1;
        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Выход из игры
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Останавливаем игру в редакторе
#endif
    }
}
