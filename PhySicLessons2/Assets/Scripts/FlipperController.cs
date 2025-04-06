using UnityEngine;

public class FlipperController : MonoBehaviour
{
    [Header("Flipper References")]
    public HingeJoint leftFlipperJoint;
    public HingeJoint rightFlipperJoint;

    [Header("Flipper Angles")]
    [Tooltip("����, ����� ������� � ����� (�����)")]
    public float leftRestAngle = -30f;
    [Tooltip("����, ����� ������� ����������� (������)")]
    public float leftActiveAngle = 45f;

    [Tooltip("����, ����� ������� � ����� (�����)")]
    public float rightRestAngle = 30f; // ���� ��� ������� ����� ���� �������
    [Tooltip("����, ����� ������� ����������� (������)")]
    public float rightActiveAngle = -45f;

    [Header("Flipper Physics")]
    [Tooltip("���� ������� (������ �� �������� �������� � ����)")]
    public float flipperSpringStrength = 500f;
    [Tooltip("������������� ������� (������� ���������)")]
    public float flipperSpringDamper = 5f;

    [Header("Input Keys")]
    public KeyCode leftFlipperKey = KeyCode.LeftArrow;
    public KeyCode rightFlipperKey = KeyCode.RightArrow;

    private JointSpring leftSpring;
    private JointSpring rightSpring;

    void Start()
    {
        // ���������, ��������� �� ��������
        if (leftFlipperJoint == null || rightFlipperJoint == null)
        {
            Debug.LogError("HingeJoints ��� ��������� �� ��������� � FlipperController!");
            enabled = false;
            return;
        }

        // --- ��������� ����� ������� ---
        // �������� ������� ��������� �������
        leftSpring = leftFlipperJoint.spring;
        // ������������� ���� � ������� �� ����������
        leftSpring.spring = flipperSpringStrength;
        leftSpring.damper = flipperSpringDamper;
        // ������������� ��������� ������� ������� - ���� �����
        leftSpring.targetPosition = leftRestAngle;
        // ��������� ����������� ��������� �������
        leftFlipperJoint.spring = leftSpring;
        // �������� ������������� ������� (�� ������ ������, ���� ������ � ����������)
        leftFlipperJoint.useSpring = true;
        leftFlipperJoint.useMotor = false; // ��������, ��� ����� ��������

        // --- ��������� ������ ������� ---
        rightSpring = rightFlipperJoint.spring;
        rightSpring.spring = flipperSpringStrength;
        rightSpring.damper = flipperSpringDamper;
        rightSpring.targetPosition = rightRestAngle;
        rightFlipperJoint.spring = rightSpring;
        rightFlipperJoint.useSpring = true;
        rightFlipperJoint.useMotor = false;

        // Debug.Log("Flipper springs configured.");
    }

    void Update()
    {
        // --- ����� ������� ---
        // ��� ������� �������
        if (Input.GetKeyDown(leftFlipperKey))
        {
            // Debug.Log("Left Flipper Activate");
            // ������������� ������� ���� ������� � �������� ���������
            leftSpring.targetPosition = leftActiveAngle;
            leftFlipperJoint.spring = leftSpring; // ��������� ���������
        }
        // ��� ���������� �������
        else if (Input.GetKeyUp(leftFlipperKey))
        {
            // Debug.Log("Left Flipper Deactivate");
            // ���������� ������� ���� ������� � ��������� �����
            leftSpring.targetPosition = leftRestAngle;
            leftFlipperJoint.spring = leftSpring; // ��������� ���������
        }

        // --- ������ ������� ---
        // ��� ������� �������
        if (Input.GetKeyDown(rightFlipperKey))
        {
            // Debug.Log("Right Flipper Activate");
            rightSpring.targetPosition = rightActiveAngle;
            rightFlipperJoint.spring = rightSpring;
        }
        // ��� ���������� �������
        else if (Input.GetKeyUp(rightFlipperKey))
        {
            // Debug.Log("Right Flipper Deactivate");
            rightSpring.targetPosition = rightRestAngle;
            rightFlipperJoint.spring = rightSpring;
        }
    }
}
