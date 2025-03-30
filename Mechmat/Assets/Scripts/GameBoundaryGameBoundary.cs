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
            Debug.LogError("BoxCollider2D �� ������ �� ������� " + gameObject.name);
        }
    }

    void OnValidate()
    {
        // ����������� boxCollider � ������ ���������
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D �� ������ �� ������� " + gameObject.name);
        }
    }

    /// <summary>
    /// ���������� ������� �������� ����.
    /// </summary>
    /// <returns>Bounds ������� BoxCollider2D.</returns>
    public Bounds GetBounds()
    {
        if (boxCollider == null)
        {
            //Debug.LogError("BoxCollider2D �� �������� � GameBoundary.");
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
