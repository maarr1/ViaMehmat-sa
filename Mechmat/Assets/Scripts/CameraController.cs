using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Ссылка на трансформ персонажа")]
    public Transform player;

    [Tooltip("Ссылка на объект, определяющий границы игры")]
    public GameBoundary gameBoundary;

    [Header("Camera Settings")]
    [Tooltip("Смещение камеры относительно персонажа")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Tooltip("Скорость сглаживания камеры")]
    public float smoothSpeed = 0.125f;

    // Внутренние границы для камеры
    private float minCameraX;
    private float maxCameraX;
    private float minCameraY;
    private float maxCameraY;

    // Размеры камеры
    private float camHalfWidth;
    private float camHalfHeight;

    void Start()
    {
        // Если player не назначен, ищем его по тегу
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"Player найден автоматически: {player.name}");
            }
            else
            {
                Debug.LogError("Player не назначен в CameraController и не найден по тегу 'Player'.");
            }
        }

        if (gameBoundary == null)
        {
            Debug.LogError("GameBoundary не назначен в CameraController.");
        }

        // Получаем размеры камеры
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        // Получаем границы из GameBoundary
        if (gameBoundary != null)
        {
            Bounds bounds = gameBoundary.GetBounds();
            minCameraX = bounds.min.x + camHalfWidth;
            maxCameraX = bounds.max.x - camHalfWidth;

            minCameraY = bounds.min.y + camHalfHeight;
            maxCameraY = bounds.max.y - camHalfHeight;
        }
    }

    void LateUpdate()
    {
        // Проверяем, есть ли игрок и границы
        if (player == null || gameBoundary == null)
            return;

        Vector3 desiredPosition = player.position + offset;

        // Кламываем позицию камеры в пределах границ
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minCameraX, maxCameraX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minCameraY, maxCameraY);
        desiredPosition.z = offset.z; // Убедитесь, что камера остаётся на нужной оси Z

        // Плавное перемещение камеры
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void OnValidate()
    {
        // Обновляем размеры камеры при изменении параметров в инспекторе
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        if (gameBoundary != null)
        {
            Bounds bounds = gameBoundary.GetBounds();
            minCameraX = bounds.min.x + camHalfWidth;
            maxCameraX = bounds.max.x - camHalfWidth;

            minCameraY = bounds.min.y + camHalfHeight;
            maxCameraY = bounds.max.y - camHalfHeight;
        }
    }

    // Обновление границ камеры при изменении размера экрана во время игры
    void UpdateCameraBounds()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        if (gameBoundary != null)
        {
            Bounds bounds = gameBoundary.GetBounds();
            minCameraX = bounds.min.x + camHalfWidth;
            maxCameraX = bounds.max.x - camHalfWidth;

            minCameraY = bounds.min.y + camHalfHeight;
            maxCameraY = bounds.max.y - camHalfHeight;
        }
    }

    void Update()
    {
        // Проверяем, изменился ли размер экрана или ortho size
        // Можно добавить дополнительные условия при необходимости
        UpdateCameraBounds();

        // Дополнительно: обновляем ссылку на игрока, если он был изменён
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"Player найден автоматически в Update: {player.name}");
            }
        }
    }

    // Метод для обновления ссылки на игрока из других скриптов (например, из GameManager)
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        Debug.Log($"CameraController установил нового игрока: {player.name}");
    }
}
