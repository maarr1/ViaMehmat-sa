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
    [Tooltip("������� ��� ����� ����� ��������� �� ����, ��� ������ �������� ������ ������")]
    public int lossesBeforeQuitEnabled = 3;

    // ����������� �������, ����������� ����� �������������� �����
    private static int lossCount = 0;
    private static bool initialized = false;

    void Awake()
    {
        // ������������� ���� ���
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
    /// ���������� ��� ������ ���������.
    /// ���������� ������ � ��������� ����������� ������ "�����".
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
        // ��� ����������� ����� ����������� lossCount �����������
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void TeleportBackToPreviousScene()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("��������� �������� �� ���������� ����� ����� GameManager.");
            GameManager.Instance.ReturnToPreviousScene();
        }
        else
        {
            Debug.LogError("GameManager �� ������. ���������, ��� ������ GameManager ������������ � �����.");
        }
    }
}
