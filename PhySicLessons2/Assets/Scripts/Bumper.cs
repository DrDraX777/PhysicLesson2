using UnityEngine;


public class Bumper : MonoBehaviour
{
    [Header("��������� �������")]
    [Tooltip("����, � ������� ������ ����������� �����. ����������� ������ ��� ��������.")]
    public float bounceForce = 30f; // �������� �������� �� ��������� ��� ����� ��������� �������

    [Tooltip("��������� �� ��������� ������������ ������� ��� �������, ����� '����������' �����?")]
    public bool addVerticalBoost = true;
    [Tooltip("�������� ������������� �������� (���� addVerticalBoost �������)")]
    public float verticalBoostAmount = 0.1f;


   

    void Start()
    {
        // ��� ��� ��������� Renderer � ����� ������
        // ---
        // ���������, ��� � ������ ������� ������ (Pinball) ���������� ��� "Pinball"!
    }

    // ���������� ��� ������ ������� �����������
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision Enter: {collision.gameObject.name}"); // � ��� �����������?

        // ��������� ��� ������
        if (collision.gameObject.CompareTag("Pinball"))
        {
            Debug.Log("Pinball Tag Confirmed!");

            Rigidbody ballRigidbody = collision.rigidbody;

            if (ballRigidbody != null)
            {
                // --- �������� ����� ����������� ���� ---
                Debug.Log($"Ball Rigidbody Found. IsKinematic: {ballRigidbody.isKinematic}, Constraints: {ballRigidbody.constraints}, Velocity BEFORE: {ballRigidbody.velocity}");

                // ���������, �� ��������� �� �� ��������
                if (ballRigidbody.isKinematic)
                {
                    Debug.LogError("������: Rigidbody ������ ��������������! ���� �� ����� �����������.", ballRigidbody);
                    return; // �������, ��� ������ ��������� ����
                }

                // --- ������ ����������� ---
                Vector3 bounceDirection = (collision.transform.position - transform.position).normalized;

                // �������� �� ������� ������ (������������, �� ��� ��)
                if (bounceDirection == Vector3.zero)
                {
                    Debug.LogWarning("����������� ������� ����� ����! ��������, ������ ���������?", this);
                    bounceDirection = Vector3.up; // �������� ������� - ������ �����
                }

                if (addVerticalBoost)
                {
                    bounceDirection += Vector3.up * verticalBoostAmount;
                    bounceDirection.Normalize();
                }
                Debug.Log($"Calculated Bounce Direction: {bounceDirection}, Bounce Force: {bounceForce}");

                // --- ��������� ���� ---
                ballRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.VelocityChange);
                Debug.Log($"AddForce Called with VelocityChange. Velocity IMMEDIATELY AFTER: {ballRigidbody.velocity}"); // ������� �������� ����� ����� AddForce
            }
            else
            {
                Debug.LogWarning($"������ '{collision.gameObject.name}' � ����� 'Pinball' ����������, �� �� ����� Rigidbody!", collision.gameObject);
            }
        }
        else
        {
            Debug.Log($"Collision with object '{collision.gameObject.name}' - Tag is NOT 'Pinball'. Ignoring.");
        }
    }


}
