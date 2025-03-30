using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GameBoundary : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D не найден на объекте " + gameObject.name);
        }
    }

    void OnValidate()
    {
        // Присваиваем boxCollider в режиме редактора
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D не найден на объекте " + gameObject.name);
        }
    }

    /// <summary>
    /// Возвращает границы игрового поля.
    /// </summary>
    /// <returns>Bounds объекта BoxCollider2D.</returns>
    public Bounds GetBounds()
    {
        if (boxCollider == null)
        {
            //Debug.LogError("BoxCollider2D не присвоен в GameBoundary.");
            return new Bounds();
        }
        return boxCollider.bounds;
    }

    void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }
    }
}
