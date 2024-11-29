using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Объект, за которым будет следовать камера
    public Vector3 offset; // Смещение камеры
    public float smoothSpeed = 0.125f; // Скорость сглаживания движения камеры

    private void LateUpdate()
    {
        if (target != null)
        {
            // Вычисляем желаемую позицию камеры с учетом смещения
            Vector3 desiredPosition = target.position + offset;
            // Плавно перемещаем камеру к желаемой позиции
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}