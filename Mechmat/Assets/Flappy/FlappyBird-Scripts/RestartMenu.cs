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
        TeleportBackToPreviousScene();
    }

    void TeleportBackToPreviousScene()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("Инициация возврата на предыдущую сцену через GameManager.");
            GameManager.Instance.ReturnToPreviousScene();
        }
        else
        {
            Debug.LogError("GameManager не найден. Убедитесь, что объект GameManager присутствует в сцене.");
        }
    }
}