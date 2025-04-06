using UnityEngine;

public class BallResetter : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Объект шарика, который нужно сбрасывать")]
    public GameObject pinballObject; // Сюда по-прежнему нужно перетащить шарик

    [Header("Настройки")]
    [Tooltip("Клавиша для сброса позиции шарика")]
    public KeyCode resetKey = KeyCode.R;

    // Переменные для хранения начальной позиции и вращения
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody ballRigidbody; // Ссылка на Rigidbody шарика

    void Start()
    {
        // Проверяем, назначен ли шарик
        if (pinballObject == null)
        {
            Debug.LogError("Объект шарика (Pinball Object) не назначен в BallResetter!", this);
            enabled = false; // Выключаем скрипт, если шарик не задан
            return;
        }

        // --- Запоминаем начальное состояние ---
        initialPosition = pinballObject.transform.position;
        initialRotation = pinballObject.transform.rotation;
        Debug.Log($"BallResetter: Начальная позиция шарика '{pinballObject.name}' сохранена: {initialPosition}");

        // Пытаемся получить Rigidbody с шарика
        ballRigidbody = pinballObject.GetComponent<Rigidbody>();
        if (ballRigidbody == null)
        {
            Debug.LogWarning("На объекте шарика (Pinball Object) не найден компонент Rigidbody! Сброс скорости/вращения не будет работать.", this);
        }

       
    }

    void Update()
    {
        // Проверяем нажатие клавиши сброса
        if (Input.GetKeyDown(resetKey))
        {
            ResetBallToInitialPosition(); // Вызываем наш метод сброса
        }
    }

    // Метод для сброса шарика в сохраненную начальную позицию
    public void ResetBallToInitialPosition()
    {
        // Проверяем наличие шарика
        if (pinballObject == null)
        {
            Debug.LogError("Невозможно сбросить позицию: ссылка на шарик потеряна.", this);
            return;
        }

        // 1. Обнуляем скорость и вращение шарика (если есть Rigidbody)
        if (ballRigidbody != null)
        {
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
        }
        

        // 2. Перемещаем шарик в сохраненную начальную позицию и вращение
        pinballObject.transform.position = initialPosition;
        pinballObject.transform.rotation = initialRotation;

        Debug.Log($"Шарик '{pinballObject.name}' сброшен в начальную позицию {initialPosition}");
    }

}
