// UIManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Buttons")]
    public Button goRestartButton;
    public Button goQuitButton;
    public Button vRestartButton;
    public Button vQuitButton;

    [Header("Loss Limit")]
    [Tooltip("Сколько раз игрок может проиграть до того, как станет доступна кнопка «Выход»")]
    public int lossesBeforeQuitEnabled = 3;

    // Статический счётчик, сохраняется между перезагрузками сцены
    private static int lossCount = 0;
    private static bool initialized = false;

    void Awake()
    {
        // Инициализация один раз
        if (!initialized)
        {
            lossCount = 0;
            initialized = true;
        }

        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);

        goRestartButton.onClick.AddListener(OnRestartClicked);
        goQuitButton.onClick.AddListener(TeleportBackToPreviousScene);
        vRestartButton.onClick.AddListener(OnRestartClicked);
        vQuitButton.onClick.AddListener(TeleportBackToPreviousScene);

        goRestartButton.interactable = true;
        goQuitButton.interactable = (lossCount >= lossesBeforeQuitEnabled);
    }

    /// <summary>
    /// Вызывается при каждом проигрыше.
    /// Показывает панель и обновляет доступность кнопки "Выход".
    /// </summary>
    public void ShowGameOver()
    {
        lossCount++;
        goQuitButton.interactable = (lossCount >= lossesBeforeQuitEnabled);
        gameOverPanel.SetActive(true);
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
    }

    private void OnRestartClicked()
    {
        // При перезапуске сцены статический lossCount сохраняется
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void TeleportBackToPreviousScene()
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
