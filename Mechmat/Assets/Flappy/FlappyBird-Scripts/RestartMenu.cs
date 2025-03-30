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
        TeleportBackToPreviousScene();
    }

    void TeleportBackToPreviousScene()
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