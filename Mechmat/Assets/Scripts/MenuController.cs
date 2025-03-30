using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button startButton; // ������ �� ������ "�����"
    public Button exitButton;  // ������ �� ������ "�����"

    [Header("Start Scene Settings")]
    [Tooltip("�������� �����, ������� ����� ��������� ��� ������� �� '�����'.")]
    public string startSceneName = "MainScene"; // �������� ����� ��� ������ ����

    [Tooltip("��� �����-������ � ������� �����.")]
    public string spawnPointName = "SpawnPoint_Main"; // ��� �����-������

    void Start()
    {
        // ���������, ��������� �� ������
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogError("Start Button �� ��������� � ����������.");
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        else
        {
            Debug.LogError("Exit Button �� ��������� � ����������.");
        }
    }

    // ����� ���������� ��� ������� �� ������ "�����"
    public void OnStartButtonClicked()
    {
        Debug.Log("������ ������ '�����'. �������� �����: " + startSceneName);
        if (GameManager.Instance != null)
        {
            // ������� ������� ���� ����� ������� ����� ����
            GameManager.Instance.ClearSceneHistory();
            GameManager.Instance.Transition(startSceneName, spawnPointName);
        }
        else
        {
            Debug.LogError("GameManager �� ������.");
        }
    }

    // ����� ���������� ��� ������� �� ������ "�����"
    void OnExitButtonClicked()
    {
        Debug.Log("������ ������ '�����'. ���������� ����.");
        Application.Quit();

        // ��� ��������� Unity (�������� ������ � ������)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
