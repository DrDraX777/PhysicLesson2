using UnityEngine;


public class Bumper : MonoBehaviour
{
    [Header("Настройки Отскока")]
    [Tooltip("Сила, с которой бампер отталкивает шарик. Применяется всегда при контакте.")]
    public float bounceForce = 30f; // Увеличил значение по умолчанию для более заметного эффекта

    [Tooltip("Добавлять ли небольшой вертикальный импульс при отскоке, чтобы 'подбросить' шарик?")]
    public bool addVerticalBoost = true;
    [Tooltip("Величина вертикального импульса (если addVerticalBoost включен)")]
    public float verticalBoostAmount = 0.1f;


   

    void Start()
    {
        // Код для получения Renderer и цвета удален
        // ---
        // Убедитесь, что у вашего объекта шарика (Pinball) установлен тег "Pinball"!
    }

    // Вызывается при первом касании коллайдеров
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision Enter: {collision.gameObject.name}"); // С кем столкнулись?

        // Проверяем тег СТРОГО
        if (collision.gameObject.CompareTag("Pinball"))
        {
            Debug.Log("Pinball Tag Confirmed!");

            Rigidbody ballRigidbody = collision.rigidbody;

            if (ballRigidbody != null)
            {
                // --- ПРОВЕРКИ ПЕРЕД ПРИМЕНЕНИЕМ СИЛЫ ---
                Debug.Log($"Ball Rigidbody Found. IsKinematic: {ballRigidbody.isKinematic}, Constraints: {ballRigidbody.constraints}, Velocity BEFORE: {ballRigidbody.velocity}");

                // Проверяем, не кинематик ли он случайно
                if (ballRigidbody.isKinematic)
                {
                    Debug.LogError("ОШИБКА: Rigidbody шарика КИНЕМАТИЧЕСКИЙ! Силы не будут действовать.", ballRigidbody);
                    return; // Выходим, нет смысла применять силу
                }

                // --- Расчет Направления ---
                Vector3 bounceDirection = (collision.transform.position - transform.position).normalized;

                // Проверка на нулевой вектор (маловероятно, но все же)
                if (bounceDirection == Vector3.zero)
                {
                    Debug.LogWarning("Направление отскока равно нулю! Возможно, центры совпадают?", this);
                    bounceDirection = Vector3.up; // Запасной вариант - отскок вверх
                }

                if (addVerticalBoost)
                {
                    bounceDirection += Vector3.up * verticalBoostAmount;
                    bounceDirection.Normalize();
                }
                Debug.Log($"Calculated Bounce Direction: {bounceDirection}, Bounce Force: {bounceForce}");

                // --- Применяем Силу ---
                ballRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.VelocityChange);
                Debug.Log($"AddForce Called with VelocityChange. Velocity IMMEDIATELY AFTER: {ballRigidbody.velocity}"); // Смотрим скорость сразу после AddForce
            }
            else
            {
                Debug.LogWarning($"Объект '{collision.gameObject.name}' с тегом 'Pinball' столкнулся, но НЕ имеет Rigidbody!", collision.gameObject);
            }
        }
        else
        {
            Debug.Log($"Collision with object '{collision.gameObject.name}' - Tag is NOT 'Pinball'. Ignoring.");
        }
    }


}
