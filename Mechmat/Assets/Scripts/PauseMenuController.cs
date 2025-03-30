using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public Button resumeButton;      // ������ "����������"
    public Button mainMenuButton;    // ������ "����� � ������� ����"
    public Button exitButton;        // ������ "����� �� ����"

    void Start()
    {
        // ���������, ��������� �� ������
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }
        else
        {
            Debug.LogError("Resume Button �� ��������� � ����������.");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
        else
        {
            Debug.LogError("Main Menu Button �� ��������� � ����������.");
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

    // ����� ���������� ��� ������� �� ������ "����������"
    void OnResumeButtonClicked()
    {
        Debug.Log("������ ������ '����������'. ������������� ����.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        else
        {
            Debug.LogError("GameManager �� ������.");
        }
    }

    // ����� ���������� ��� ������� �� ������ "����� � ������� ����"
    void OnMainMenuButtonClicked()
    {
        Debug.Log("������ ������ '����� � ������� ����'. �������� �������� ����.");
        if (GameManager.Instance != null)
        {
            // ������� ������� ���� � ��������� ������� ����
            GameManager.Instance.ClearSceneHistory();
            GameManager.Instance.Transition("MainMenu", "");
        }
        else
        {
            Debug.LogError("GameManager �� ������.");
        }
    }

    // ����� ���������� ��� ������� �� ������ "����� �� ����"
    void OnExitButtonClicked()
    {
        Debug.Log("������ ������ '����� �� ����'. �������� ����������.");
        Application.Quit();

        // ��� ��������� Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
