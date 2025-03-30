using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("�������� �������� ��������� �� ��� X")]
    public float speed = 7f;

    [Header("Boundary Settings")]
    [Tooltip("������ �� ������, ������������ ������� ����")]
    public GameBoundary gameBoundary;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isMoving = false;

    // ���������� ��� �������� ��������� ��������
    private Vector3 originalScale;

    // ������ �� CapsuleCollider2D
    private CapsuleCollider2D capsuleCollider;

    // ������ ��������� ������������� ��������
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();

    void Start()
    {
        // �������� ���������� Animator � Rigidbody2D, ������������ � ���������
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // �������� CapsuleCollider2D
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider == null)
        {
            Debug.LogError($"CapsuleCollider2D �� ������ �� ������� {gameObject.name}.");
        }

        // ��������� �������� ������� ���������
        originalScale = transform.localScale;

        // �������� �� ������� �����������
        if (animator == null)
        {
            Debug.LogError($"��������� Animator �� ������ �� ������� {gameObject.name}.");
        }

        if (rb == null)
        {
            Debug.LogError($"��������� Rigidbody2D �� ������ �� ������� {gameObject.name}.");
        }

        // �������� �� ������� GameBoundary
        if (gameBoundary == null)
        {
            Debug.LogError($"GameBoundary �� �������� � PlayerController.");
        }
    }

    void Update()
    {
        // �������� ���� ������������ �� ��� X (������� A/D ��� ������� �����/������)
        float inputX = Input.GetAxisRaw("Horizontal");
        movement = new Vector2(inputX, 0f);

        // ����������, �������� �� ��������
        isMoving = Mathf.Abs(inputX) > 0.1f;

        // ��������� �������� Animator
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }

        // ������������ ��������� � ����������� �� ����������� ��������
        if (inputX > 0)
        {
            // �������� ������: ����������� ������� �� X ��� ��������
            transform.localScale = new Vector3(originalScale.x * -1, originalScale.y, originalScale.z);
        }
        else if (inputX < 0)
        {
            // �������� �����: ������������� ���������� ������� �� X
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        }

        // �������� �� ������� ������ �������������� (��������, E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractWithNearest();
        }
    }

    void FixedUpdate()
    {
        if (gameBoundary == null || capsuleCollider == null)
            return;

        // �������� ������� �� GameBoundary
        Bounds bounds = gameBoundary.GetBounds();

        // �������� ������� ����������
        Vector2 colliderSize = capsuleCollider.size;
        // ���� �������� ���������
        Vector2 scaledColliderSize = Vector2.Scale(colliderSize, new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y)));

        // ������������ ����� ��������� ���������
        Vector2 newPosition = rb.position + movement * speed * Time.fixedDeltaTime;

        // ������������ ������� ��������� �� ���� X � Y �� ������ ������ � �������� ����������
        float clampedX = Mathf.Clamp(newPosition.x, bounds.min.x + scaledColliderSize.x / 2, bounds.max.x - scaledColliderSize.x / 2);
        float clampedY = Mathf.Clamp(newPosition.y, bounds.min.y + scaledColliderSize.y / 2, bounds.max.y - scaledColliderSize.y / 2);

        newPosition = new Vector2(clampedX, clampedY);

        // ���������� ���������
        rb.MovePosition(newPosition);
    }

    // ����� ��� �������������� � ��������� ������������� ��������
    private void InteractWithNearest()
    {
        if (nearbyInteractables.Count > 0)
        {
            // ��� �������� ��������������� � ������ � ������
            IInteractable interactable = nearbyInteractables[0];
            interactable.Interact();
            Debug.Log($"�������������� � �������� ���� {interactable.GetType().Name}.");
        }
        else
        {
            Debug.Log("��� �������� ��� ��������������.");
        }
    }

    // ���������� ������� � ������ ��� ����� � �������
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Door") || other.CompareTag("Interactable"))
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null && !nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Add(interactable);
                Debug.Log($"�������� ������ ��� ��������������: {other.gameObject.name}.");
            }
        }
    }

    // �������� ������� �� ������ ��� ������ �� ��������
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Door") || other.CompareTag("Interactable"))
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null && nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Remove(interactable);
                Debug.Log($"����� ������ ��� ��������������: {other.gameObject.name}.");
            }
        }
    }
}
