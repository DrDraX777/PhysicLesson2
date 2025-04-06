using UnityEngine;
using System.Collections; // ��� �������� ��������

public class SpringLauncher : MonoBehaviour
{
    [Header("��������� �������")]
    [Tooltip("������������ ����������, �� ������� ��������� ���������� ����")]
    public float maxChargeDistance = 1.0f;
    [Tooltip("����� � ��������, �� ������� ��������� ��������� ����������")]
    public float timeToFullCharge = 1.0f;
    [Tooltip("����������� ���� ������� (��� �������� �������)")]
    public float minLaunchForce = 10f;
    [Tooltip("������������ ���� ������� (��� ������ ������)")]
    public float maxLaunchForce = 100f;
    [Tooltip("��������, � ������� ��������� '������������' ����������� �����")]
    public float returnSpeed = 20f;
    [Tooltip("������� ��� ������� � �������")]
    public KeyCode launchKey = KeyCode.Space;

    [Header("�����������")]
    [Tooltip("������� ���������� ����� ����� ��� �������? �������� �������� '���������'.")]
    public bool addUpwardBoost = true;
    [Tooltip("���� ����, ������������ ����� ����� (0-1)")]
    [Range(0f, 1f)]
    public float upwardBoostFraction = 0.1f;

    // ��������� ����������
    private Rigidbody platformRigidbody;
    private Vector3 startPosition;           // ������� ������� ���������
    private Vector3 chargedPosition;         // ������ ������� ���������
    private float currentCharge = 0f;      // ������� ����� (0.0 �� 1.0)
    private bool isCharging = false;         // ����: ���� �� ������ �������?
    private bool isReturning = false;        // ����: ������������ �� ���������?

    private Rigidbody ballToLaunch = null; // ������ �� Rigidbody ������ �� ���������
    private Coroutine returnCoroutine = null; // ������ �� �������� ��������

    void Start()
    {
        platformRigidbody = GetComponent<Rigidbody>();
        if (platformRigidbody == null)
        {
            Debug.LogError("�� ������� SpringLauncher ����������� Rigidbody!", this);
            enabled = false;
            return;
        }
        if (!platformRigidbody.isKinematic)
        {
            Debug.LogWarning("Rigidbody �� SpringLauncher ������ ���� Kinematic! ������� IsKinematic.", this);
            platformRigidbody.isKinematic = true;
        }

        // ���������� ��������� (�������) �������
        startPosition = platformRigidbody.position;
        // ������������ ������ ������� (���� �� ��������� ��� Y)
        chargedPosition = startPosition - transform.up * maxChargeDistance;

        // ��������, ��� �������� � ���������� ���������
        platformRigidbody.position = startPosition;
        currentCharge = 0f;
        isCharging = false;
        isReturning = false;
    }

    void Update()
    {
        HandleInput();
        UpdatePlatformPosition();
    }

    void HandleInput()
    {
       
        // --- ������� ������� ---
        if (Input.GetKeyDown(launchKey))
        {
            // ���� �� �� ���������� � �� ������������ ��� ���� �� ��������� �������
            if (!isCharging) // ���������� ��������: �������� ���� �� ���������� ������
            {
                // Debug.Log("Start Charging requested");

                // ���� ��� ������� - ��������� ���
                if (isReturning && returnCoroutine != null)
                {
                    // Debug.Log("Interrupting return to start charging.");
                    StopCoroutine(returnCoroutine);
                    returnCoroutine = null;
                    isReturning = false; // ������ �� ������������
                    // ��� ���������� �������� ����� �������� ������,
                    // ����� ��������� �������� � ������� �����
                    platformRigidbody.position = startPosition;
                }
                // ���� �� ������������ � ���� � Idle, ���� ��� ��

                // �������� ����� �������
                isCharging = true;
                currentCharge = 0f; // <--- ��� �������� �����
                // Debug.Log($"Starting new charge. CurrentCharge set to {currentCharge}");
            }
            // ���� isCharging ��� true, �� ��������� ������� ������ �� ������
        }
        // --- ��������� ������� ---
        else if (Input.GetKey(launchKey) && isCharging)
        {
            // ����������� ����� �� ��������, ���� ������ ������
            if (timeToFullCharge > 0)
            {
                // ����������� ����� �� ����, ���������������� ������� �����
                currentCharge = Mathf.Clamp01(currentCharge + (Time.deltaTime / timeToFullCharge));
                // Debug.Log($"Charging: {currentCharge * 100f}%");
            }
            else
            {
                currentCharge = 1.0f; // ���������� �����, ���� ����� = 0
            }
        }
        // --- ���������� ������� ---
        else if (Input.GetKeyUp(launchKey))
        {
            // ���� �� ����������
            if (isCharging)
            {
                // Debug.Log($"Launch Triggered! Charge: {currentCharge * 100f}%");
                isCharging = false; // ���������� �������
                Launch();         // ��������� ����� (���� �� ����)
                StartReturn();    // �������� ������� ���������
            }
        }
    }

    void UpdatePlatformPosition()
    {
        // ���� ���� �������, ������� ��������� ���� �������� ������
        if (isCharging)
        {
            // �������� ������������ ����� ������� � ������ ������
            Vector3 targetPos = Vector3.Lerp(startPosition, chargedPosition, currentCharge);
            platformRigidbody.MovePosition(targetPos);
        }
        // ���� ��������� �� ���������� � �� ������������ (����������� ���������),
        // ��� ������ ���������� �� ����� (� startPosition ��� ��� �� �������� ��������)
    }

    void Launch()
    {
        // ���� ����� �� ����� �� ���������, ������ �� ������
        if (ballToLaunch == null)
        {
            // Debug.Log("Launch attempt, but no ball detected.");
            return;
        }

        // ������������ ���� ������� �� ������ �������� ������
        float actualLaunchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, currentCharge);

        // ���������� ����������� ������� (����� �� ��������� ��� Y ���������)
        Vector3 launchDirection = transform.up;

        // ��������� ������������ ������������ ����
        if (addUpwardBoost)
        {
            launchDirection = Vector3.Lerp(launchDirection, Vector3.up, upwardBoostFraction).normalized;
        }

        // ��������� ���� � ������
        ballToLaunch.isKinematic = false; // ��������, ��� ����� �� ���������
        ballToLaunch.AddForce(launchDirection * actualLaunchForce, ForceMode.Impulse);

        // Debug.Log($"Applied Force: {actualLaunchForce} in direction {launchDirection}");

        // ���������� ����� � ������ �� �����
        currentCharge = 0f;
        // ballToLaunch = null; // �� ���������� �����! ������� � OnCollisionExit, ����� �� ������
    }

    void StartReturn()
    {
        // ���� ������� ��� ����, �� ��������� �����
        if (isReturning || returnCoroutine != null) return;

        // Debug.Log("Starting return coroutine.");
        isReturning = true;
        returnCoroutine = StartCoroutine(ReturnPlatformCoroutine());
    }

    // �������� ��� �������� (��� ��������) �������� ��������� �����
    IEnumerator ReturnPlatformCoroutine()
    {
        Vector3 currentPos = platformRigidbody.position;
        float distanceToCover = Vector3.Distance(currentPos, startPosition);
        float distanceCovered = 0f;

        while (distanceCovered < distanceToCover)
        {
            // ������� ��������� ����� �� ��������� returnSpeed
            distanceCovered = Mathf.MoveTowards(distanceCovered, distanceToCover, returnSpeed * Time.deltaTime);
            float fraction = (distanceToCover > 0.001f) ? distanceCovered / distanceToCover : 1f;
            platformRigidbody.MovePosition(Vector3.Lerp(currentPos, startPosition, fraction));
            yield return null; // ���� ��������� ����
        }

        // ����������, ��� ��������� ����� �������
        platformRigidbody.MovePosition(startPosition);
        // Debug.Log("Return finished.");
        isReturning = false;
        returnCoroutine = null; // ���������� ������ �� ��������
    }

    // --- ����������� ������ ---

    // ����������, ����� ������ ��������� ������ � ������� � �����
    void OnCollisionEnter(Collision collision)
    {
        CheckForBall(collision.collider);
    }

    // ���������� ������ ����, ���� ������ ��������� �������� ������
    void OnCollisionStay(Collision collision)
    {
        // ��� �����, ���� ����� �������� �� ���������, �� Enter �� ��������,
        // ��� ���� �� ����� ��������� ���������.
        if (ballToLaunch == null) // ��������� ������ ���� ����� ��� �� ��������
        {
            CheckForBall(collision.collider);
        }
    }

    // ����������, ����� ������ ��������� ��������� �������� ������
    void OnCollisionExit(Collision collision)
    {
        // ���� ���������/����������� ������ - ��� ��� �����, ��� �� ���������� ���������
        if (ballToLaunch != null && collision.rigidbody == ballToLaunch)
        {
            // Debug.Log("Ball exited the launcher.");
            ballToLaunch = null; // ������ ������ �� �����
        }
    }

    // ��������������� ����� ��� ��������, �������� �� ��������� �������
    void CheckForBall(Collider otherCollider)
    {
        // ��������� ��� ������� � ���� �� � ���� Rigidbody
        if (otherCollider.CompareTag("Pinball")) // ���������, ��� ��� ����� ����� ��� "Pinball"
        {
            Rigidbody rb = otherCollider.attachedRigidbody;
            if (rb != null)
            {
                // ���� �� ��� �� ���������� �����, ��������� ������
                if (ballToLaunch == null)
                {
                    // Debug.Log($"Ball detected and registered: {otherCollider.gameObject.name}");
                    ballToLaunch = rb;
                    
                }
            }
        }
    }
}
