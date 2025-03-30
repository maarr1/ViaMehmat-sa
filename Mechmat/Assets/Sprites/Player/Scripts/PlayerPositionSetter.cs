using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionSetter : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerPosition(transform);
        }
        else
        {
            Debug.LogError("GameManager не найден при загрузке сцены.");
        }
    }
}
