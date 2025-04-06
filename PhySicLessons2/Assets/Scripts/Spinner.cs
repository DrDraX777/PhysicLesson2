using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Header("Настройки Вращения")]
    [Tooltip("Скорость вращения в градусах в секунду.")]
    public float rotationSpeed = 180f; // Скорость по умолчанию (пол-оборота в секунду)

    [Tooltip("Направление вращения. Отметьте для вращения по часовой стрелке (вокруг положительной оси Y), снимите для вращения против часовой.")]
    public bool rotateClockwise = true;

    // Ось вращения (локальная ось Y объекта)
    private Vector3 rotationAxis = Vector3.up;

    

    // Update вызывается один раз за кадр
    void Update()
    {
        // 1. Определяем направление вращения (множитель)
        // Если clockwise = true, множитель = 1, если false, множитель = -1
        float directionMultiplier = rotateClockwise ? 1f : -1f;

        // 2. Рассчитываем угол поворота для этого кадра
        // Умножаем скорость на Time.deltaTime, чтобы вращение было плавным
        // и не зависело от частоты кадров (FPS).
        float angleThisFrame = rotationSpeed * directionMultiplier * Time.deltaTime;

        // 3. Применяем вращение к объекту
        // transform.Rotate(ось, угол, система_координат)
        // Используем локальную ось Y (Vector3.up) и применяем рассчитанный угол.
        // По умолчанию Rotate с Vector3 осью работает в локальных координатах (Space.Self),
        // что нам и нужно для вращения объекта вокруг *своей* оси Y.
        transform.Rotate(rotationAxis, angleThisFrame);

     
    }
}
