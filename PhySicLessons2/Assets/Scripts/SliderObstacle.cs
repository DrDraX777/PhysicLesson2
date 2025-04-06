using UnityEngine;

// ����������� ������� Rigidbody �� �������, ��� ��� ����� ������������ MovePosition
[RequireComponent(typeof(Rigidbody))]
public class SliderObstacle : MonoBehaviour
{
    [Header("��������� ��������")]
    [Tooltip("����������, �� ������� ������ ���������� �� ��������� ����� ����� ����� ��������� ��� Z (����� �������).")]
    public float moveDistance = 3.0f;

    [Tooltip("�������� �������� �������.")]
    public float moveSpeed = 2.0f;

    private Rigidbody rb;
    private Vector3 startPosition;      // ��������� ������� �������
    private Vector3 targetPosition;     // �������� ������� ������� (��������� �� Z)
    private Vector3 currentDestination; // ���� �� �������� ������ (start ��� target)

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // �����: Rigidbody ������ ���� Kinematic ��� ���������� ������ MovePosition
        // � ����� ������ �� ������ �� ���� �������� ��������.
        if (!rb.isKinematic)
        {
            Debug.LogWarning($"Rigidbody �� '{gameObject.name}' �� ��� Kinematic. ������������ IsKinematic = true ��� ������� SliderObstacle.", this);
            rb.isKinematic = true;
        }
        // ��������� ���������� �� ������ ������, ��� �� ����� ��� ����������
        rb.useGravity = false;

        // ��������� ��������� ������� �������
        startPosition = rb.position;

        // ������������ ������� ������� �������.
        // transform.forward - ��� ��������� ��� Z ������� (����� ������� � ���������).
        // ���������, ��� ��� ������ �������� � ����� ���, ����� ��� ����� ��� Z
        // ��������� � ��� �����������, ���� �� ������ ����������!
        targetPosition = startPosition + transform.forward * moveDistance;

        // �������� �������� � ������������ ������� �����
        currentDestination = targetPosition;
    }

    // �������� Rigidbody (�������� Kinematic) ������� ��������� � FixedUpdate
    void FixedUpdate()
    {
        // ���� �������� ��� ��������� �������, �� ���������
        if (moveSpeed <= 0f || moveDistance <= 0f)
        {
            // ����� ����������� ������� � ��������� �������, ���� �����
            // rb.MovePosition(startPosition);
            return;
        }

        // ������������ ��������� ������� �� ���� � currentDestination
        // Vector3.MoveTowards ������� ������� ����� � ���� �� ������������ ���������� speed * time
        Vector3 nextPosition = Vector3.MoveTowards(rb.position, currentDestination, moveSpeed * Time.fixedDeltaTime);

        // ��������� ����� ������� � Rigidbody
        rb.MovePosition(nextPosition);

        // ���������, �������� �� �� ������� ���� (� ��������� ������������)
        if (Vector3.Distance(rb.position, currentDestination) < 0.01f)
        {
            // ���� ��������, ������ ���� �� ���������������
            if (currentDestination == targetPosition)
            {
                currentDestination = startPosition;
            }
            else // currentDestination == startPosition
            {
                currentDestination = targetPosition;
            }
          
        }
    }

    void OnDrawGizmosSelected()
    {
        // ���������� ��������� � �������� ����� ��� �����
        Vector3 startGizmoPos;
        Vector3 targetGizmoPos;

        // ���� ���� �������� � ������� ����������, ���������� ��
        if (Application.isPlaying && startPosition != Vector3.zero)
        {
            startGizmoPos = startPosition;
            targetGizmoPos = targetPosition;
        }
        // �����, ������������ �� ������ ������� ������� � ���������
        else
        {
            startGizmoPos = transform.position;
            // ������������ target ������������ ������� ������� � ��������� ��� Z
            targetGizmoPos = startGizmoPos + transform.forward * moveDistance;
        }

        // ������ ����� � �����
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(startGizmoPos, targetGizmoPos);
        Gizmos.DrawWireSphere(startGizmoPos, 0.1f); // ����� � ��������� �����
        Gizmos.DrawWireSphere(targetGizmoPos, 0.1f); // ����� � �������� �����
    }
}
