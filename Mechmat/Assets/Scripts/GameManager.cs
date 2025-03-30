using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // �������� ������� �����
    private string currentScene;

    // �������� ������� ����� � �����-������
    private string targetScene;
    private string spawnPointName;

    // ���� ��� �������� ������� ���� � ������� ���������
    private List<SceneData> sceneHistory = new List<SceneData>();
    private const int maxHistorySize = 5;

    // ���������� ��� �������� ������ � ����� ��� ��������
    private SceneData returnSceneData;

    // ����, �����������, ��� ���� ��������� �� ����� (� ����)
    private bool isPaused = false;

    private void Awake()
    {
        // ��������� ������� Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ������������� �� ������� �������� �����
            SceneManager.sceneLoaded += OnSceneLoaded;

            // ��������� ������� �����
            currentScene = SceneManager.GetActiveScene().name;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // ��������� ������� ������� Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                // ���� ���� �� �� �����, ������ �� ����� � ��������� � ����
                PauseGame();
            }
            else
            {
                // ���� ���� �� �����, ������������ � ����
                ResumeGame();
            }
        }
    }

    private void OnDestroy()
    {
        // ������������ �� �������, ����� �������� ������ ������
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// ����� ��� ���������� ���� �� ����� � �������� � ����.
    /// </summary>
    public void PauseGame()
    {
        Debug.Log("����� ����. ������� � ����.");

        // ��������� ������� ����� � ������� ��������� ����� ���������
        SaveCurrentSceneData();

        // ������������� ���� �����
        isPaused = true;

        // ��������� ����� ����
        targetScene = "PauseMenu"; // �������� ����� � ���� �����
        spawnPointName = "";

        SceneManager.LoadScene(targetScene);
    }

    /// <summary>
    /// ����� ��� ������������� ���� � �������� � ���������� �����.
    /// </summary>
    public void ResumeGame()
    {
        if (returnSceneData != null)
        {
            Debug.Log("������������� ����. ������� � �����: " + returnSceneData.sceneName);

            // ������������� ������� ����� � ������� spawnPointName
            targetScene = returnSceneData.sceneName;
            spawnPointName = "";

            // ��������� ���������� �����
            SceneManager.LoadScene(targetScene);

            // ���������� ���� �����
            isPaused = false;
        }
        else
        {
            Debug.LogWarning("��� ���������� ������ ��� ������������� ����.");
        }
    }

    /// <summary>
    /// ����� ��� ���������� �������� � ����� �����.
    /// </summary>
    /// <param name="sceneName">�������� ������� �����.</param>
    /// <param name="spawnName">��� �����-������ � ������� ����� (�������� ������, ���� ��� ������).</param>
    public void Transition(string sceneName, string spawnName)
    {
        // ��������� ������� ����� � ������� ��������� ����� ���������
        SaveCurrentSceneData();

        // ������������� ������� ����� � �����-�����
        targetScene = sceneName;
        spawnPointName = spawnName;

        // ��������� ����� �����
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// ����� ��� �������� �� ���������� �����.
    /// </summary>
    public void ReturnToPreviousScene()
    {
        if (sceneHistory.Count > 0)
        {
            // ��������� ��������� ������ �� �������
            returnSceneData = sceneHistory[sceneHistory.Count - 1];

            // ������� � �� �������
            sceneHistory.RemoveAt(sceneHistory.Count - 1);

            // ������������� ������� ����� � ������� spawnPointName
            targetScene = returnSceneData.sceneName;
            spawnPointName = "";

            // ��������� �����
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("������� ���� �����. ������� ����������.");
        }
    }

    // ���������� ������� �������� �����
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;

        // ���������, ���� �� �������� � �����
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            // ���� �� ������������ �� ����� � ���� ���������� ������
            if (returnSceneData != null && currentScene == returnSceneData.sceneName)
            {
                // ��������������� ������� ���������
                playerObj.transform.position = returnSceneData.playerPosition;
                Debug.Log($"�������� ����������� ������� ({returnSceneData.playerPosition}) � ����� '{currentScene}'.");

                // �������� returnSceneData ����� �������������
                returnSceneData = null;
            }
            else if (!string.IsNullOrWhiteSpace(spawnPointName))
            {
                // ������������� ������� ��������� �� �����-�����
                SetPlayerPosition(playerObj.transform);
            }
            else
            {
                // ���� spawnPointName ���� ��� ������� ������ �� ��������, ��������� ��������� �� ��� ������� �������
                Debug.Log($"�������� ������� �� ������� ������� � ����� '{currentScene}'.");
            }

            // ��������� ������ ������ �� ���������
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.SetPlayer(playerObj.transform);
                Debug.Log("CameraController ������� ������ �� ���������.");
            }
            else
            {
                Debug.LogError("CameraController �� ������ �� Main Camera.");
            }
        }
        else
        {
            Debug.Log("�������� �� ������ � �����. ��������������, ��� ��� ����� ��� ���������.");
        }
    }



    /// <summary>
    /// ����� ��� ��������� ������� ��������� �� �����-�����.
    /// </summary>
    /// <param name="player">��������� ���������.</param>
    public void SetPlayerPosition(Transform player)
    {
        // ������� ������ ������ �� �����
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            player.position = spawnPoint.transform.position;
            Debug.Log($"�������� �������������� �� �����-����� '{spawnPointName}' � ����� '{targetScene}'.");
        }
        else
        {
            Debug.LogError($"�����-����� '{spawnPointName}' �� ������ � ����� '{targetScene}'.");
        }
    }

    /// <summary>
    /// ����� ��� ���������� ������� ����� � ������� ��������� ����� ���������.
    /// </summary>
    private void SaveCurrentSceneData()
    {
        // ������� ��������� � ������� �����
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Vector3 currentPosition = playerObj.transform.position;

            // ������ ����� ������ SceneData
            returnSceneData = new SceneData(currentScene, spawnPointName, currentPosition);

            // ��������� ��� � �������
            sceneHistory.Add(returnSceneData);

            // ������������ ������ �������
            if (sceneHistory.Count > maxHistorySize)
            {
                // ������� ����� ������ ������� (������ � ������)
                sceneHistory.RemoveAt(0);
            }

            Debug.Log($"����� '{currentScene}' ��������� � �������. ������� ������ �������: {sceneHistory.Count}");
        }
        else
        {
            Debug.LogWarning("�������� �� ������ � ������� �����. ������� �� ���������.");
        }
    }

    /// <summary>
    /// ����� ��� ������� ������� ����.
    /// </summary>
    public void ClearSceneHistory()
    {
        sceneHistory.Clear();
        returnSceneData = null;
        isPaused = false;
        Debug.Log("������� ���� �������.");
    }

}

/// <summary>
/// ����� ��� �������� ������ � ����� � ������� ���������.
/// </summary>
[System.Serializable]
public class SceneData
{
    public string sceneName;
    public string spawnPointName;
    public Vector3 playerPosition;

    public SceneData(string sceneName, string spawnPointName, Vector3 playerPosition)
    {
        this.sceneName = sceneName;
        this.spawnPointName = spawnPointName;
        this.playerPosition = playerPosition;
    }
}
