using UnityEngine;
using UnityEngine.SceneManagement; // ��� �������� �����
using UnityEngine.UI; // ��� ������ � UI

public class DeadZone : MonoBehaviour
{
    public GameObject RestartMenu; // ������ �� ���� ����������

    public void ShowRestartMenu()
    {
        // ��������� ���� �����������
        Time.timeScale = 0; // ������������� ����
        RestartMenu.SetActive(true);
    }


}
