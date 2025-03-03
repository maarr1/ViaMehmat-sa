using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject congratulationsText; // Ссылка на текстовое поле

    // Этот метод будет вызываться при входе в триггер
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, является ли объект игроком
        if (other.CompareTag("Crow")) 
        {
            CompleteLevel();
        }
    }

    public void CompleteLevel()
    {
        // Показать поздравление
        congratulationsText.SetActive(true);
        congratulationsText.GetComponent<Text>().text = "Поздравляю! Вы завершили уровень!";
    }
}