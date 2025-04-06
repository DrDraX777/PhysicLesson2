using UnityEngine;

// ������������� ������� ������� Collider �� �������, � �������� ���������� ������
[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    // ������ �� ������ ������ ������.
    // �� ����� ������ ��� �������������, �� ����� ������� public ��� ������� ����������.
    // public BallResetter ballResetter;

    void Start()
    {
        // ��������, ��� ��������� �� ���� ������� ������������� �������� ���������.
        // ��� ������������ �� ������, ���� ������ ��������� ������� � ���������.
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Collider �� ������� '{gameObject.name}' �� ��� ���������� ��� Trigger. ������������ isTrigger = true.", this);
            col.isTrigger = true;
        }
    }

    // ���� ����� Unity ������������� ��������, ����� ������ Collider ������ � ��� �������.
    // 'other' - ��� ���������� � ����������, ������� �����.
    void OnTriggerEnter(Collider other)
    {
        // 1. ���������, ����� �� �������� ������ ��� "Pinball"
        if (other.CompareTag("Pinball"))
        {
            Debug.Log($"������ '{other.gameObject.name}' (Pinball) ����� � {this.name}. ����� �������.");

            // 2. ���� � ����� ������ �� �������� BallResetter
            //    FindObjectOfType ������� ������ ���������� �������� ��������� ����� ����.
            //    ��������, ���� � ��� ������ ���� BallResetter � �����.
            BallResetter resetter = FindObjectOfType<BallResetter>();

            // 3. ���������, ����� �� �� ������ ������
            if (resetter != null)
            {
                // 4. �������� ��������� ����� ������ �� ������� BallResetter
                resetter.ResetBallToInitialPosition();
            }
            else
            {
                // ���� ������ BallResetter �� ������, ������� ������
                Debug.LogError($"�� ������ ������ BallResetter � �����! �� ���� �������� ����� �� {this.name}.", this);
            }
        }
        
    }

    
}
