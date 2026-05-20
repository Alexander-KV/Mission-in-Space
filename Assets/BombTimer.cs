using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;   // <-- для SceneManager

public class BombTimer : MonoBehaviour
{
    [Header("Таймер")]
    public float timerDuration = 60f;

    [Header("UI")]
    public GameObject timerPanel;
    public TextMeshProUGUI timerText;
    public GameObject defeatPanel;

    [Header("Звук")]
    public AudioSource sirenAudioSource;
    public AudioClip sirenClip;

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

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            timerText.color = timeLeft < 15f ? Color.red : Color.white;
        }

        ShakeCamera();

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            isRunning = false;
            TriggerDefeat();
        }
    }

    void ShakeCamera()
    {
        float progress = 1f - (timeLeft / timerDuration);
        float currentIntensity = shakeIntensity * progress;

        float offsetX = Mathf.Sin(Time.time * shakeFrequency) * currentIntensity;
        float offsetY = Mathf.Cos(Time.time * shakeFrequency * 1.3f) * currentIntensity;

        playerCamera.transform.localPosition = originalCamPos + new Vector3(offsetX, offsetY, 0f);
    }

    void TriggerDefeat()
    {
        if (sirenAudioSource != null) sirenAudioSource.Stop();
        playerCamera.transform.localPosition = originalCamPos;

        if (defeatPanel != null) defeatPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Поражение — бомба взорвалась!");

        // Загрузка LoseScene через 1 секунду (чтобы игрок увидел надпись)
        Invoke(nameof(LoadLoseScene), 0f);
    }

    void LoadLoseScene()
    {
        Time.timeScale = 0f;   // обязательно разморозить игру перед загрузкой сцены
        SceneManager.LoadScene("LoseScene");
    }

    public void StopTimer()
    {
        isRunning = false;
        if (sirenAudioSource != null) sirenAudioSource.Stop();
        playerCamera.transform.localPosition = originalCamPos;
        if (timerPanel != null) timerPanel.SetActive(false);
    }

    public bool IsTimerRunning()
    {
        return isRunning;
    }
}