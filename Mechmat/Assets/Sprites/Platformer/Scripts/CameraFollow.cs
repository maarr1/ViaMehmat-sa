using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowWithDeadZone : MonoBehaviour
{
    [Header("Что отслеживаем")]
    public Transform target;

    [Header("Смещение камеры")]
    public Vector2 offset = new Vector2(0f, 1f);

    [Header("Размер Dead-Zone (% экрана)")]
    [Range(0f, 1f)] public float deadZoneWidth = 0.4f;
    [Range(0f, 1f)] public float deadZoneHeight = 0.3f;

    [Header("Границы уровня (зелёный)")]
    public BoxCollider2D boundsCollider;

    [Header("Плавность")]
    [Range(0f, 0.5f)] public float smoothTime = 0.1f;

    private Camera cam;
    private Vector3 velocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam.orthographic)
            Debug.LogWarning("Камера должна быть Orthographic.");
        if (target == null)
            Debug.LogError("Не задан Target!");
        if (boundsCollider == null)
            Debug.LogError("Не задан Bounds Collider!");
    }

    void Start()
    {
        // Сразу в правильную позицию, без сглаживания
        transform.position = CalculateCameraPosition();
    }

    void LateUpdate()
    {
        Vector3 newPos = CalculateCameraPosition();
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    private Vector3 CalculateCameraPosition()
    {
        // 1) Текущая камера (с offset по цели)
        Vector3 camPos = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        // 2) Строим dead-zone в мировых coords
        Rect deadZone = new Rect(
            camPos.x - deadZoneWidth * halfW,
            camPos.y - deadZoneHeight * halfH,
            deadZoneWidth * 2f * halfW,
            deadZoneHeight * 2f * halfH
        );

        // 3) Если персонаж вышел за dead-zone, двигаем camPos
        Vector3 hero = target.position;
        if (hero.x < deadZone.xMin) camPos.x -= (deadZone.xMin - hero.x);
        else if (hero.x > deadZone.xMax) camPos.x += (hero.x - deadZone.xMax);

        if (hero.y < deadZone.yMin) camPos.y -= (deadZone.yMin - hero.y);
        else if (hero.y > deadZone.yMax) camPos.y += (hero.y - deadZone.yMax);

        // 4) Зажимаем camPos внутрь boundsCollider
        Bounds b = boundsCollider.bounds;
        float minX = b.min.x + halfW;
        float maxX = b.max.x - halfW;
        float minY = b.min.y + halfH;
        float maxY = b.max.y - halfH;

        camPos.x = Mathf.Clamp(camPos.x, minX, maxX);
        camPos.y = Mathf.Clamp(camPos.y, minY, maxY);

        return camPos;
    }

    void OnDrawGizmosSelected()
    {
        if (cam == null || boundsCollider == null) return;

        // Границы уровня
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boundsCollider.bounds.center, boundsCollider.bounds.size);

        // Текущий dead-zone
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 center = (Application.isPlaying ? transform.position : target != null
                         ? new Vector3(target.position.x + offset.x, target.position.y + offset.y, 0)
                         : Vector3.zero);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3(center.x, center.y, 0),
            new Vector3(deadZoneWidth * 2f * halfW, deadZoneHeight * 2f * halfH, 0)
        );
    }
}
