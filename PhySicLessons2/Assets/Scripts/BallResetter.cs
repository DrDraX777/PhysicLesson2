using UnityEngine;

public class BallResetter : MonoBehaviour
{
    [Header("������")]
    [Tooltip("������ ������, ������� ����� ����������")]
    public GameObject pinballObject; // ���� ��-�������� ����� ���������� �����

    [Header("���������")]
    [Tooltip("������� ��� ������ ������� ������")]
    public KeyCode resetKey = KeyCode.R;

    // ���������� ��� �������� ��������� ������� � ��������
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody ballRigidbody; // ������ �� Rigidbody ������

    void Start()
    {
        // ���������, �������� �� �����
        if (pinballObject == null)
        {
            Debug.LogError("������ ������ (Pinball Object) �� �������� � BallResetter!", this);
            enabled = false; // ��������� ������, ���� ����� �� �����
            return;
        }

        // --- ���������� ��������� ��������� ---
        initialPosition = pinballObject.transform.position;
        initialRotation = pinballObject.transform.rotation;
        Debug.Log($"BallResetter: ��������� ������� ������ '{pinballObject.name}' ���������: {initialPosition}");

        // �������� �������� Rigidbody � ������
        ballRigidbody = pinballObject.GetComponent<Rigidbody>();
        if (ballRigidbody == null)
        {
            Debug.LogWarning("�� ������� ������ (Pinball Object) �� ������ ��������� Rigidbody! ����� ��������/�������� �� ����� ��������.", this);
        }

       
    }

    void Update()
    {
        // ��������� ������� ������� ������
        if (Input.GetKeyDown(resetKey))
        {
            ResetBallToInitialPosition(); // �������� ��� ����� ������
        }
    }

    // ����� ��� ������ ������ � ����������� ��������� �������
    public void ResetBallToInitialPosition()
    {
        // ��������� ������� ������
        if (pinballObject == null)
        {
            Debug.LogError("���������� �������� �������: ������ �� ����� ��������.", this);
            return;
        }

        // 1. �������� �������� � �������� ������ (���� ���� Rigidbody)
        if (ballRigidbody != null)
        {
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
        }
        

        // 2. ���������� ����� � ����������� ��������� ������� � ��������
        pinballObject.transform.position = initialPosition;
        pinballObject.transform.rotation = initialRotation;

        Debug.Log($"����� '{pinballObject.name}' ������� � ��������� ������� {initialPosition}");
    }

}
