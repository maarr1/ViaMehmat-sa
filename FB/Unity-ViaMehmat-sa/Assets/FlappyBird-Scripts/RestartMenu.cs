using UnityEngine;
using UnityEngine.SceneManagement; // ��� �������� �����

public class RestartMenu : MonoBehaviour
{
   
    public void RestartGame()
    {
        // ���������� �����
        Time.timeScale = 1;
        // ������������� ������� �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // ����� �� ����
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // ������������� ���� � ���������
#endif
    }
}