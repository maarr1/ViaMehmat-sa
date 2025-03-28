using UnityEngine;
using UnityEngine.UI; 

public class LevelManager : MonoBehaviour
{
    public GameObject congratulationsText; // Ссылка на текстовое поле

    public void CompleteLevel()
    {
        // Показать поздравление
        congratulationsText.SetActive(true);
        Time.timeScale = 0; // Останавливаем игру
    }

}