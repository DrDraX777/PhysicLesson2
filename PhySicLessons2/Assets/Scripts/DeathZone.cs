using UnityEngine;

// Автоматически требуем наличие Collider на объекте, к которому прикреплен скрипт
[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    // Ссылка на скрипт сброса шарика.
    // Мы будем искать его автоматически, но можно сделать public для ручного назначения.
    // public BallResetter ballResetter;

    void Start()
    {
        // Убедимся, что коллайдер на этом объекте действительно является триггером.
        // Это подстраховка на случай, если забыли поставить галочку в редакторе.
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Collider на объекте '{gameObject.name}' не был установлен как Trigger. Устанавливаю isTrigger = true.", this);
            col.isTrigger = true;
        }
    }

    // Этот метод Unity автоматически вызывает, когда ДРУГОЙ Collider ВХОДИТ в наш триггер.
    // 'other' - это информация о коллайдере, который вошел.
    void OnTriggerEnter(Collider other)
    {
        // 1. Проверяем, имеет ли вошедший объект тег "Pinball"
        if (other.CompareTag("Pinball"))
        {
            Debug.Log($"Объект '{other.gameObject.name}' (Pinball) вошел в {this.name}. Сброс позиции.");

            // 2. Ищем в сцене объект со скриптом BallResetter
            //    FindObjectOfType находит ПЕРВЫЙ попавшийся активный компонент этого типа.
            //    Подходит, если у вас только один BallResetter в сцене.
            BallResetter resetter = FindObjectOfType<BallResetter>();

            // 3. Проверяем, нашли ли мы скрипт сброса
            if (resetter != null)
            {
                // 4. Вызываем публичный метод сброса из скрипта BallResetter
                resetter.ResetBallToInitialPosition();
            }
            else
            {
                // Если скрипт BallResetter не найден, выводим ошибку
                Debug.LogError($"Не найден скрипт BallResetter в сцене! Не могу сбросить шарик из {this.name}.", this);
            }
        }
        
    }

    
}
