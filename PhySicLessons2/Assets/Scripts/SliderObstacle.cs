using UnityEngine;

// Гарантируем наличие Rigidbody на объекте, так как будем использовать MovePosition
[RequireComponent(typeof(Rigidbody))]
public class SliderObstacle : MonoBehaviour
{
    [Header("Настройки Движения")]
    [Tooltip("Расстояние, на которое объект сдвигается от начальной точки вдоль своей ЛОКАЛЬНОЙ оси Z (синяя стрелка).")]
    public float moveDistance = 3.0f;

    [Tooltip("Скорость движения объекта.")]
    public float moveSpeed = 2.0f;

    private Rigidbody rb;
    private Vector3 startPosition;      // Начальная позиция объекта
    private Vector3 targetPosition;     // Конечная позиция объекта (сдвинутая по Z)
    private Vector3 currentDestination; // Куда мы движемся сейчас (start или target)

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Важно: Rigidbody ДОЛЖЕН быть Kinematic для корректной работы MovePosition
        // и чтобы физика не влияла на наше движение напрямую.
        if (!rb.isKinematic)
        {
            Debug.LogWarning($"Rigidbody на '{gameObject.name}' не был Kinematic. Устанавливаю IsKinematic = true для скрипта SliderObstacle.", this);
            rb.isKinematic = true;
        }
        // Отключаем гравитацию на всякий случай, она не нужна для кинематика
        rb.useGravity = false;

        // Сохраняем начальную мировую позицию
        startPosition = rb.position;

        // Рассчитываем целевую мировую позицию.
        // transform.forward - это локальная ось Z объекта (синяя стрелка в редакторе).
        // Убедитесь, что ваш объект повернут в сцене так, чтобы его синяя ось Z
        // указывала в том направлении, куда он должен сдвигаться!
        targetPosition = startPosition + transform.forward * moveDistance;

        // Начинаем движение к рассчитанной целевой точке
        currentDestination = targetPosition;
    }

    // Движение Rigidbody (особенно Kinematic) следует выполнять в FixedUpdate
    void FixedUpdate()
    {
        // Если скорость или дистанция нулевые, не двигаемся
        if (moveSpeed <= 0f || moveDistance <= 0f)
        {
            // Можно опционально вернуть в начальную позицию, если стоит
            // rb.MovePosition(startPosition);
            return;
        }

        // Рассчитываем следующую позицию на пути к currentDestination
        // Vector3.MoveTowards двигает текущую точку к цели на максимальное расстояние speed * time
        Vector3 nextPosition = Vector3.MoveTowards(rb.position, currentDestination, moveSpeed * Time.fixedDeltaTime);

        // Применяем новую позицию к Rigidbody
        rb.MovePosition(nextPosition);

        // Проверяем, достигли ли мы текущей цели (с небольшой погрешностью)
        if (Vector3.Distance(rb.position, currentDestination) < 0.01f)
        {
            // Если достигли, меняем цель на противоположную
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
        // Определяем начальную и конечную точки для гизмо
        Vector3 startGizmoPos;
        Vector3 targetGizmoPos;

        // Если игра запущена и позиции рассчитаны, используем их
        if (Application.isPlaying && startPosition != Vector3.zero)
        {
            startGizmoPos = startPosition;
            targetGizmoPos = targetPosition;
        }
        // Иначе, рассчитываем на основе текущей позиции в редакторе
        else
        {
            startGizmoPos = transform.position;
            // Рассчитываем target относительно текущей позиции и локальной оси Z
            targetGizmoPos = startGizmoPos + transform.forward * moveDistance;
        }

        // Рисуем линию и сферы
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(startGizmoPos, targetGizmoPos);
        Gizmos.DrawWireSphere(startGizmoPos, 0.1f); // Сфера в начальной точке
        Gizmos.DrawWireSphere(targetGizmoPos, 0.1f); // Сфера в конечной точке
    }
}
