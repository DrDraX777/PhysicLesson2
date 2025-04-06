using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Header("��������� ��������")]
    [Tooltip("�������� �������� � �������� � �������.")]
    public float rotationSpeed = 180f; // �������� �� ��������� (���-������� � �������)

    [Tooltip("����������� ��������. �������� ��� �������� �� ������� ������� (������ ������������� ��� Y), ������� ��� �������� ������ �������.")]
    public bool rotateClockwise = true;

    // ��� �������� (��������� ��� Y �������)
    private Vector3 rotationAxis = Vector3.up;

    

    // Update ���������� ���� ��� �� ����
    void Update()
    {
        // 1. ���������� ����������� �������� (���������)
        // ���� clockwise = true, ��������� = 1, ���� false, ��������� = -1
        float directionMultiplier = rotateClockwise ? 1f : -1f;

        // 2. ������������ ���� �������� ��� ����� �����
        // �������� �������� �� Time.deltaTime, ����� �������� ���� �������
        // � �� �������� �� ������� ������ (FPS).
        float angleThisFrame = rotationSpeed * directionMultiplier * Time.deltaTime;

        // 3. ��������� �������� � �������
        // transform.Rotate(���, ����, �������_���������)
        // ���������� ��������� ��� Y (Vector3.up) � ��������� ������������ ����.
        // �� ��������� Rotate � Vector3 ���� �������� � ��������� ����������� (Space.Self),
        // ��� ��� � ����� ��� �������� ������� ������ *�����* ��� Y.
        transform.Rotate(rotationAxis, angleThisFrame);

     
    }
}
