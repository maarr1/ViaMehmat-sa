using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("�������� ������� ����� ��� ��������")]
    public string targetSceneName;

    [Tooltip("��� �����-������ � ������� ����� (����� ���� ������, ���� ����� �� ���������)")]
    public string spawnPointName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (string.IsNullOrWhiteSpace(spawnPointName))
            {
                Debug.Log($"�������� ����� � ���� �������� �� ����� '{targetSceneName}', �����-����� �� �����.");
            }
            else
            {
                Debug.Log($"�������� ����� � ���� �������� �� ����� '{targetSceneName}' � �����-������� '{spawnPointName}'.");
            }

            // �������������� ������� ����� GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Transition(targetSceneName, spawnPointName);
            }
            else
            {
                Debug.LogError("GameManager �� ������. ���������, ��� ������ GameManager ������������ � �����.");
            }
        }
    }
}
