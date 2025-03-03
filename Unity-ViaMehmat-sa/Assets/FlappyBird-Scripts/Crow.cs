using UnityEngine;

public class Crow : MonoBehaviour
{
    public float flapForce = 5f; // ����, � ������� ������ ��������
    public float forwardSpeed = 2f; // �������� �������� ������
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // �������� ��������� Animator
    }

    void Update()
    {
        // ���������, ������ �� ������� ������� ��� ���
        if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            Flap();
        }

        // ������� ������ ������
        transform.Translate(Vector2.right * forwardSpeed * Time.deltaTime);
    }

    void Flap()
    {
        rb.velocity = Vector2.up * flapForce; // ��������� ���� � ������
        animator.SetTrigger("Fly"); // ��������� �������� ������
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������, ����������� �� ������ � �������� DeadZone
        if (other.CompareTag("DeadZone"))
        {
            Debug.Log("Game Over!"); // �������� ��������� ����
            FindObjectOfType<DeadZone>().ShowRestartMenu(); // �������� ����� �������� ����
        }
    }
}