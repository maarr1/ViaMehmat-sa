using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки сцены
using UnityEngine.UI; // Для работы с UI

public class DeadZone : MonoBehaviour
{
    public GameObject restartMenu; // Ссылка на меню перезапуска

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, если объект, вошедший в зону, является игроком
        if (other.CompareTag("Crow"))
        {
            // Открываем меню перезапуска
            ShowRestartMenu();
        }
    }

    private void ShowRestartMenu()
    {
        // Открываем меню перезапуска
        restartMenu.SetActive(true);
        Time.timeScale = 0; // Останавливаем игру
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
