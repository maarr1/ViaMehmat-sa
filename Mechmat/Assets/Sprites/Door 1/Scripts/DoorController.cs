using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [Tooltip("���������� ����� �����")]
    public int doorID;

    [Tooltip("����� ����������� ������������� ��� �����������")]
    public bool opensAutomatically = false;

    [Tooltip("����� ����� ���� ������� ����������")]
    public bool isOpenable = true;

    [Header("Teleport Settings")]
    [Tooltip("�������� ����� ��� ��������")]
    public string targetSceneName;

    [Tooltip("��� �����-������ � ������� ����� (�������� ������, ���� � ����� ��� ���������)")]
    public string spawnPointName;

    private Animator animator;
    private bool isOpen = false;
    private bool isPlayerNear = false;

    void Start()
    {
        // �������� ��������� Animator
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"Animator �� ������ �� ������� {gameObject.name}. ���������, ��� ��������� Animator ���������.");
        }
        else
        {
            Debug.Log($"Animator ������� ������ �� ����� {doorID} ({gameObject.name}).");
        }

        // ��������, ��� BoxCollider2D �������� ��� �������
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null && !boxCollider.isTrigger)
        {
            boxCollider.isTrigger = true;
            Debug.LogWarning($"BoxCollider2D �� ����� {doorID} ({gameObject.name}) ��� �������� ��� ��-�������. �������� �� �������.");
        }

        // ������������� ��������� �����
        SetDoorState(isOpen);
    }

    // ����� ����������, ����� ������ ��������� ������ � ������� �����
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log($"�������� ����� � ���� ����� {doorID} ({gameObject.name}).");

            if (opensAutomatically && isOpenable)
            {
                OpenDoor();
            }
        }
    }

    // ����� ����������, ����� ������ ��������� �������� ������� �����
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            Debug.Log($"�������� ������� ���� ����� {doorID} ({gameObject.name}).");

            if (opensAutomatically && isOpenable)
            {
                CloseDoor();
            }
        }
    }

    // ����� ��� ��������� ��������� �����
    private void SetDoorState(bool open)
    {
        if (animator != null)
        {
            animator.SetBool("Is_open", open);
            Debug.Log($"����� {doorID} ({gameObject.name}) ����������� ��������� 'Is_open' = {open}.");
        }
        else
        {
            Debug.LogWarning($"Animator �� ������ �� ����� {doorID} ({gameObject.name}). �� ������� �������� ��������� �����.");
        }
    }

    // �������� �����
    public void OpenDoor()
    {
        if (!isOpen && isOpenable)
        {
            SetDoorState(true);
            isOpen = true;
            Debug.Log($"����� {doorID} ({gameObject.name}) �������.");
        }
    }

    // �������� �����
    public void CloseDoor()
    {
        if (isOpen)
        {
            SetDoorState(false);
            isOpen = false;
            Debug.Log($"����� {doorID} ({gameObject.name}) �������.");
        }
    }

    // ���������� ������ �� ���������� IInteractable
    public void Interact()
    {
        if (isOpenable)
        {
            if (animator != null)
            {
                if (isOpen)
                {
                    Debug.Log($"�������������� � ������ {doorID} ({gameObject.name}) ��� ��������.");
                    TeleportPlayer();
                }
                else
                {
                    Debug.Log($"�������������� � ������ {doorID} ({gameObject.name}) ��� ��������.");
                    ToggleDoor();
                }
            }
            else
            {
                Debug.LogWarning($"Animator �� ������ �� ����� {doorID} ({gameObject.name}). �� ������� ����������������� � ������.");
            }
        }
        else
        {
            Debug.Log($"����� {doorID} ({gameObject.name}) �� ����������. �������������� ����������.");
        }
    }

    // ����� ��� ������������ ��������� �����
    private void ToggleDoor()
    {
        if (isOpenable)
        {
            isOpen = !isOpen;
            SetDoorState(isOpen);
            Debug.Log($"����� {doorID} ({gameObject.name}) ����������� ��������� 'Is_open' = {isOpen}.");
        }
    }

    // ����� ��� ������������ ��������� ����� GameManager
    private void TeleportPlayer()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"��������� �������� � ����� '{targetSceneName}' � �����-������� '{spawnPointName}'.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Transition(targetSceneName, spawnPointName);
            }
            else
            {
                Debug.LogError("GameManager �� ������. ���������, ��� ������ GameManager ������������ � �����.");
            }
        }
        else
        {
            Debug.LogError($"����� {doorID} ({gameObject.name}) �� ��������� ��� ��������. ���������, ��� 'targetSceneName' �����.");
        }
    }
}
