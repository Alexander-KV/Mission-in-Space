using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BombTimer : MonoBehaviour
{
    [Header("Таймер")]
    public float timerDuration = 60f;         // 60 секунд

    [Header("UI")]
    public GameObject timerPanel;             // панель с таймером
    public TextMeshProUGUI timerText;         // текст таймера
    public GameObject defeatPanel;           // экран поражения

    [Header("Звук")]
    public AudioSource sirenAudioSource;     // источник звука сирены
    public AudioClip sirenClip;              // клип сирены

    [Header("Тряска камеры")]
    public float shakeIntensity = 0.05f;
    public float shakeFrequency = 10f;

    private float timeLeft;
    private bool isRunning = false;
    private Camera playerCamera;
    private Vector3 originalCamPos;

    void Start()
    {
        playerCamera = Camera.main;
        if (timerPanel != null) timerPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
    }

    public void StartTimer()
    {
        timeLeft = timerDuration;
        isRunning = true;

        if (timerPanel != null) timerPanel.SetActive(true);

        // Запускаем сирену
        if (sirenAudioSource != null && sirenClip != null)
        {
            sirenAudioSource.clip = sirenClip;
            sirenAudioSource.loop = true;
            sirenAudioSource.Play();
        }

        originalCamPos = playerCamera.transform.localPosition;
    }

    void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;

        // Обновляем текст таймера
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Текст краснеет когда < 15 сек
            timerText.color = timeLeft < 15f ? Color.red : Color.white;
        }

        // Тряска камеры
        ShakeCamera();

        // Время вышло — поражение
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            isRunning = false;
            TriggerDefeat();
        }
    }

    void ShakeCamera()
    {
        // Нарастающая тряска по мере убывания времени
        float progress = 1f - (timeLeft / timerDuration); // 0 → 1
        float currentIntensity = shakeIntensity * progress;

        float offsetX = Mathf.Sin(Time.time * shakeFrequency) * currentIntensity;
        float offsetY = Mathf.Cos(Time.time * shakeFrequency * 1.3f) * currentIntensity;

        playerCamera.transform.localPosition = originalCamPos + new Vector3(offsetX, offsetY, 0f);
    }

    void TriggerDefeat()
    {
        // Стоп сирена
        if (sirenAudioSource != null) sirenAudioSource.Stop();

        // Восстанавливаем камеру
        playerCamera.transform.localPosition = originalCamPos;

        // Показываем экран поражения
        if (defeatPanel != null) defeatPanel.SetActive(true);

        // Замораживаем игру
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Поражение — бомба взорвалась!");
    }

    // Вызывается ShipEscape при победе — останавливает таймер
    public void StopTimer()
    {
        isRunning = false;
        if (sirenAudioSource != null) sirenAudioSource.Stop();
        playerCamera.transform.localPosition = originalCamPos;
        if (timerPanel != null) timerPanel.SetActive(false);
    }
}
