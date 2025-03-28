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


}
