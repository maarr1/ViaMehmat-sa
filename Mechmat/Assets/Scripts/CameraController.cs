using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("������ �� ��������� ���������")]
    public Transform player;

    [Tooltip("������ �� ������, ������������ ������� ����")]
    public GameBoundary gameBoundary;

    [Header("Camera Settings")]
    [Tooltip("�������� ������ ������������ ���������")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Tooltip("�������� ����������� ������")]
    public float smoothSpeed = 0.125f;

    // ���������� ������� ��� ������
    private float minCameraX;
    private float maxCameraX;
    private float minCameraY;
    private float maxCameraY;

    // ������� ������
    private float camHalfWidth;
    private float camHalfHeight;

    void Start()
    {
        // ���� player �� ��������, ���� ��� �� ����
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"Player ������ �������������: {player.name}");
            }
            else
            {
                Debug.LogError("Player �� �������� � CameraController � �� ������ �� ���� 'Player'.");
            }
        }

        if (gameBoundary == null)
        {
            Debug.LogError("GameBoundary �� �������� � CameraController.");
        }

        // �������� ������� ������
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        // �������� ������� �� GameBoundary
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
        // ���������, ���� �� ����� � �������
        if (player == null || gameBoundary == null)
            return;

        Vector3 desiredPosition = player.position + offset;

        // ��������� ������� ������ � �������� ������
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minCameraX, maxCameraX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minCameraY, maxCameraY);
        desiredPosition.z = offset.z; // ���������, ��� ������ ������� �� ������ ��� Z

        // ������� ����������� ������
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void OnValidate()
    {
        // ��������� ������� ������ ��� ��������� ���������� � ����������
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

    // ���������� ������ ������ ��� ��������� ������� ������ �� ����� ����
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
        // ���������, ��������� �� ������ ������ ��� ortho size
        // ����� �������� �������������� ������� ��� �������������
        UpdateCameraBounds();

        // �������������: ��������� ������ �� ������, ���� �� ��� ������
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"Player ������ ������������� � Update: {player.name}");
            }
        }
    }

    // ����� ��� ���������� ������ �� ������ �� ������ �������� (��������, �� GameManager)
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        Debug.Log($"CameraController ��������� ������ ������: {player.name}");
    }
}
