using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки сцены

public class RestartMenu : MonoBehaviour
{
   
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