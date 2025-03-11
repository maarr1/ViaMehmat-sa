using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Цель, за которой будет следить камера
    public float smoothSpeed = 0.125f; // Скорость сглаживания движения камеры
    public Vector3 offset; // Смещение камеры относительно цели

    void LateUpdate()
    {
        if (target != null)
        {
            // Вычисляем желаемую позицию камеры
            Vector3 desiredPosition = target.position + offset;
            // Сглаживаем движение камеры
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            // Обновляем позицию камеры
            transform.position = new Vector3(smoothedPosition.x, transform.position.y, transform.position.z);
        }
    }
}