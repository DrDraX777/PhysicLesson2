using UnityEngine;
using System.Collections; // Для корутины возврата

public class SpringLauncher : MonoBehaviour
{
    [Header("Настройки Пружины")]
    [Tooltip("Максимальное расстояние, на которое платформа опускается вниз")]
    public float maxChargeDistance = 1.0f;
    [Tooltip("Время в секундах, за которое платформа полностью опускается")]
    public float timeToFullCharge = 1.0f;
    [Tooltip("Минимальная сила запуска (при коротком нажатии)")]
    public float minLaunchForce = 10f;
    [Tooltip("Максимальная сила запуска (при полном заряде)")]
    public float maxLaunchForce = 100f;
    [Tooltip("Скорость, с которой платформа 'выстреливает' возвращаясь вверх")]
    public float returnSpeed = 20f;
    [Tooltip("Клавиша для зарядки и запуска")]
    public KeyCode launchKey = KeyCode.Space;

    [Header("Опционально")]
    [Tooltip("Немного подбросить шарик вверх при запуске? Помогает избежать 'залипания'.")]
    public bool addUpwardBoost = true;
    [Tooltip("Доля силы, направленная чисто вверх (0-1)")]
    [Range(0f, 1f)]
    public float upwardBoostFraction = 0.1f;

    // Приватные переменные
    private Rigidbody platformRigidbody;
    private Vector3 startPosition;           // Верхняя позиция платформы
    private Vector3 chargedPosition;         // Нижняя позиция платформы
    private float currentCharge = 0f;      // Текущий заряд (0.0 до 1.0)
    private bool isCharging = false;         // Флаг: идет ли сейчас зарядка?
    private bool isReturning = false;        // Флаг: возвращается ли платформа?

    private Rigidbody ballToLaunch = null; // Ссылка на Rigidbody шарика на платформе
    private Coroutine returnCoroutine = null; // Ссылка на корутину возврата

    void Start()
    {
        platformRigidbody = GetComponent<Rigidbody>();
        if (platformRigidbody == null)
        {
            Debug.LogError("На объекте SpringLauncher отсутствует Rigidbody!", this);
            enabled = false;
            return;
        }
        if (!platformRigidbody.isKinematic)
        {
            Debug.LogWarning("Rigidbody на SpringLauncher должен быть Kinematic! Включаю IsKinematic.", this);
            platformRigidbody.isKinematic = true;
        }

        // Запоминаем начальную (верхнюю) позицию
        startPosition = platformRigidbody.position;
        // Рассчитываем нижнюю позицию (вниз по ЛОКАЛЬНОЙ оси Y)
        chargedPosition = startPosition - transform.up * maxChargeDistance;

        // Убедимся, что начинаем в правильном состоянии
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
       
        // --- НАЖАТИЕ КЛАВИШИ ---
        if (Input.GetKeyDown(launchKey))
        {
            // Если мы не заряжались и не возвращались ИЛИ если мы прерываем возврат
            if (!isCharging) // Упрощенная проверка: начинаем если не заряжаемся сейчас
            {
                // Debug.Log("Start Charging requested");

                // Если шел возврат - прерываем его
                if (isReturning && returnCoroutine != null)
                {
                    // Debug.Log("Interrupting return to start charging.");
                    StopCoroutine(returnCoroutine);
                    returnCoroutine = null;
                    isReturning = false; // Больше не возвращаемся
                    // При прерывании возврата важно снепнуть наверх,
                    // чтобы опускание началось с верхней точки
                    platformRigidbody.position = startPosition;
                }
                // Если не возвращались и были в Idle, тоже все ок

                // Начинаем новую зарядку
                isCharging = true;
                currentCharge = 0f; // <--- Вот КЛЮЧЕВОЙ СБРОС
                // Debug.Log($"Starting new charge. CurrentCharge set to {currentCharge}");
            }
            // Если isCharging уже true, то повторное нажатие ничего не делает
        }
        // --- УДЕРЖАНИЕ КЛАВИШИ ---
        else if (Input.GetKey(launchKey) && isCharging)
        {
            // Увеличиваем заряд со временем, пока кнопка зажата
            if (timeToFullCharge > 0)
            {
                // Увеличиваем заряд на долю, пропорциональную времени кадра
                currentCharge = Mathf.Clamp01(currentCharge + (Time.deltaTime / timeToFullCharge));
                // Debug.Log($"Charging: {currentCharge * 100f}%");
            }
            else
            {
                currentCharge = 1.0f; // Мгновенный заряд, если время = 0
            }
        }
        // --- ОТПУСКАНИЕ КЛАВИШИ ---
        else if (Input.GetKeyUp(launchKey))
        {
            // Если мы заряжались
            if (isCharging)
            {
                // Debug.Log($"Launch Triggered! Charge: {currentCharge * 100f}%");
                isCharging = false; // Прекращаем зарядку
                Launch();         // Запускаем шарик (если он есть)
                StartReturn();    // Начинаем возврат платформы
            }
        }
    }

    void UpdatePlatformPosition()
    {
        // Если идет зарядка, двигаем платформу вниз согласно заряду
        if (isCharging)
        {
            // Линейная интерполяция между верхней и нижней точкой
            Vector3 targetPos = Vector3.Lerp(startPosition, chargedPosition, currentCharge);
            platformRigidbody.MovePosition(targetPos);
        }
        // Если платформа не заряжается и не возвращается (управляется корутиной),
        // она должна оставаться на месте (в startPosition или где ее оставила корутина)
    }

    void Launch()
    {
        // Если шарик не лежит на платформе, ничего не делаем
        if (ballToLaunch == null)
        {
            // Debug.Log("Launch attempt, but no ball detected.");
            return;
        }

        // Рассчитываем силу запуска на основе текущего заряда
        float actualLaunchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, currentCharge);

        // Определяем направление запуска (вверх по ЛОКАЛЬНОЙ оси Y платформы)
        Vector3 launchDirection = transform.up;

        // Добавляем опциональный вертикальный буст
        if (addUpwardBoost)
        {
            launchDirection = Vector3.Lerp(launchDirection, Vector3.up, upwardBoostFraction).normalized;
        }

        // Применяем силу к шарику
        ballToLaunch.isKinematic = false; // Убедимся, что шарик не кинематик
        ballToLaunch.AddForce(launchDirection * actualLaunchForce, ForceMode.Impulse);

        // Debug.Log($"Applied Force: {actualLaunchForce} in direction {launchDirection}");

        // Сбрасываем заряд и ссылку на шарик
        currentCharge = 0f;
        // ballToLaunch = null; // НЕ СБРАСЫВАЕМ ЗДЕСЬ! Сбросим в OnCollisionExit, когда он улетит
    }

    void StartReturn()
    {
        // Если возврат уже идет, не запускаем новый
        if (isReturning || returnCoroutine != null) return;

        // Debug.Log("Starting return coroutine.");
        isReturning = true;
        returnCoroutine = StartCoroutine(ReturnPlatformCoroutine());
    }

    // Корутина для плавного (или быстрого) возврата платформы вверх
    IEnumerator ReturnPlatformCoroutine()
    {
        Vector3 currentPos = platformRigidbody.position;
        float distanceToCover = Vector3.Distance(currentPos, startPosition);
        float distanceCovered = 0f;

        while (distanceCovered < distanceToCover)
        {
            // Двигаем платформу вверх со скоростью returnSpeed
            distanceCovered = Mathf.MoveTowards(distanceCovered, distanceToCover, returnSpeed * Time.deltaTime);
            float fraction = (distanceToCover > 0.001f) ? distanceCovered / distanceToCover : 1f;
            platformRigidbody.MovePosition(Vector3.Lerp(currentPos, startPosition, fraction));
            yield return null; // Ждем следующий кадр
        }

        // Убеждаемся, что платформа точно наверху
        platformRigidbody.MovePosition(startPosition);
        // Debug.Log("Return finished.");
        isReturning = false;
        returnCoroutine = null; // Сбрасываем ссылку на корутину
    }

    // --- Обнаружение Шарика ---

    // Вызывается, когда другой коллайдер входит в контакт с нашим
    void OnCollisionEnter(Collision collision)
    {
        CheckForBall(collision.collider);
    }

    // Вызывается каждый кадр, пока другой коллайдер касается нашего
    void OnCollisionStay(Collision collision)
    {
        // Это нужно, если шарик оказался на платформе, но Enter не сработал,
        // или если мы хотим постоянно проверять.
        if (ballToLaunch == null) // Проверяем только если шарик еще не захвачен
        {
            CheckForBall(collision.collider);
        }
    }

    // Вызывается, когда другой коллайдер перестает касаться нашего
    void OnCollisionExit(Collision collision)
    {
        // Если улетевший/укатившийся объект - это тот шарик, что мы собирались запустить
        if (ballToLaunch != null && collision.rigidbody == ballToLaunch)
        {
            // Debug.Log("Ball exited the launcher.");
            ballToLaunch = null; // Теряем ссылку на шарик
        }
    }

    // Вспомогательный метод для проверки, является ли коллайдер шариком
    void CheckForBall(Collider otherCollider)
    {
        // Проверяем тег объекта и есть ли у него Rigidbody
        if (otherCollider.CompareTag("Pinball")) // Убедитесь, что ваш шарик имеет тег "Pinball"
        {
            Rigidbody rb = otherCollider.attachedRigidbody;
            if (rb != null)
            {
                // Если мы еще не удерживаем шарик, сохраняем ссылку
                if (ballToLaunch == null)
                {
                    // Debug.Log($"Ball detected and registered: {otherCollider.gameObject.name}");
                    ballToLaunch = rb;
                    
                }
            }
        }
    }
}
