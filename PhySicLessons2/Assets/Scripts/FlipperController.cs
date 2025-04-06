using UnityEngine;

public class FlipperController : MonoBehaviour
{
    [Header("Flipper References")]
    public HingeJoint leftFlipperJoint;
    public HingeJoint rightFlipperJoint;

    [Header("Flipper Angles")]
    [Tooltip("Угол, когда флиппер в покое (внизу)")]
    public float leftRestAngle = -30f;
    [Tooltip("Угол, когда флиппер активирован (вверху)")]
    public float leftActiveAngle = 45f;

    [Tooltip("Угол, когда флиппер в покое (внизу)")]
    public float rightRestAngle = 30f; // Углы для правого могут быть другими
    [Tooltip("Угол, когда флиппер активирован (вверху)")]
    public float rightActiveAngle = -45f;

    [Header("Flipper Physics")]
    [Tooltip("Сила пружины (влияет на скорость движения к цели)")]
    public float flipperSpringStrength = 500f;
    [Tooltip("Демпфирование пружины (убирает колебания)")]
    public float flipperSpringDamper = 5f;

    [Header("Input Keys")]
    public KeyCode leftFlipperKey = KeyCode.LeftArrow;
    public KeyCode rightFlipperKey = KeyCode.RightArrow;

    private JointSpring leftSpring;
    private JointSpring rightSpring;

    void Start()
    {
        // Проверяем, назначены ли флипперы
        if (leftFlipperJoint == null || rightFlipperJoint == null)
        {
            Debug.LogError("HingeJoints для флипперов не назначены в FlipperController!");
            enabled = false;
            return;
        }

        // --- Настройка Левой Пружины ---
        // Получаем текущую структуру пружины
        leftSpring = leftFlipperJoint.spring;
        // Устанавливаем силу и демпфер из инспектора
        leftSpring.spring = flipperSpringStrength;
        leftSpring.damper = flipperSpringDamper;
        // Устанавливаем начальную целевую позицию - угол покоя
        leftSpring.targetPosition = leftRestAngle;
        // Применяем настроенную структуру обратно
        leftFlipperJoint.spring = leftSpring;
        // Включаем использование пружины (на всякий случай, если забыли в инспекторе)
        leftFlipperJoint.useSpring = true;
        leftFlipperJoint.useMotor = false; // Убедимся, что мотор выключен

        // --- Настройка Правой Пружины ---
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
        // --- Левый Флиппер ---
        // При НАЖАТИИ клавиши
        if (Input.GetKeyDown(leftFlipperKey))
        {
            // Debug.Log("Left Flipper Activate");
            // Устанавливаем целевой угол пружины в активное положение
            leftSpring.targetPosition = leftActiveAngle;
            leftFlipperJoint.spring = leftSpring; // Применяем изменение
        }
        // При ОТПУСКАНИИ клавиши
        else if (Input.GetKeyUp(leftFlipperKey))
        {
            // Debug.Log("Left Flipper Deactivate");
            // Возвращаем целевой угол пружины в положение покоя
            leftSpring.targetPosition = leftRestAngle;
            leftFlipperJoint.spring = leftSpring; // Применяем изменение
        }

        // --- Правый Флиппер ---
        // При НАЖАТИИ клавиши
        if (Input.GetKeyDown(rightFlipperKey))
        {
            // Debug.Log("Right Flipper Activate");
            rightSpring.targetPosition = rightActiveAngle;
            rightFlipperJoint.spring = rightSpring;
        }
        // При ОТПУСКАНИИ клавиши
        else if (Input.GetKeyUp(rightFlipperKey))
        {
            // Debug.Log("Right Flipper Deactivate");
            rightSpring.targetPosition = rightRestAngle;
            rightFlipperJoint.spring = rightSpring;
        }
    }
}
