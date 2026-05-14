using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BombSystem : MonoBehaviour
{
    [Header("?? Настройки")]
    public float bombTimer = 60f; // Время на установку и побег (в секундах)
    public string loseSceneName = "LoseScene"; // Имя сцены поражения
    public string winSceneName = "WinScene"; // Имя сцены победы

    [Header("?? Звук сирены")]
    public AudioSource sirenAudio;
    public AudioClip sirenClip;

    [Header("?? Метка бомбы")]
    public GameObject bombMarker; // Маркер места установки (иконка/стрелка)
    public GameObject bombPlantSpot; // Точка, куда ставить бомбу

    [Header("?? Звездолёт")]
    public GameObject starship; // Объект звездолёта
    public float escapeHoldTime = 3f; // Сколько держать E (секунды)

    [Header("?? UI")]
    public GameObject timerPanel; // Панель таймера
    public UnityEngine.UI.Text timerText; // Текст таймера
    public GameObject escapePrompt; // Подсказка "Удержи E"
    public UnityEngine.UI.Image holdProgressBar; // Полоска удержания

    private bool isBombPlanted = false;
    private bool isTimerRunning = false;
    private float currentTime;
    private bool isPlayerNearShip = false;
    private float holdProgress = 0f;

    void Start()
    {
        // Скрываем всё при старте
        if (bombMarker != null) bombMarker.SetActive(false);
        if (timerPanel != null) timerPanel.SetActive(false);
        if (escapePrompt != null) escapePrompt.SetActive(false);

        // Настраиваем звук
        if (sirenAudio != null && sirenClip != null)
        {
            sirenAudio.clip = sirenClip;
            sirenAudio.loop = true;
            sirenAudio.playOnAwake = false;
        }
    }

    void Update()
    {
        // Таймер бомбы
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0)
            {
                TriggerDefeat();
            }
        }

        // Удержание E у корабля
        if (isPlayerNearShip && isTimerRunning && !isBombPlanted)
        {
            if (Input.GetKey(KeyCode.E))
            {
                holdProgress += Time.deltaTime / escapeHoldTime;
                UpdateHoldProgress();

                if (holdProgress >= 1f)
                {
                    EscapeToWinScene();
                }
            }
            else
            {
                holdProgress = 0f;
                UpdateHoldProgress();
            }
        }
    }

    // Вызывается когда босс убит
    public void OnBossKilled()
    {
        Debug.Log("?? Босс убит! Активируем метку бомбы...");

        // Показываем метку где ставить бомбу
        if (bombMarker != null)
            bombMarker.SetActive(true);
    }

    // Вызывается когда игрок поставил бомбу
    public void PlantBomb()
    {
        if (isBombPlanted) return;

        Debug.Log("?? Бомба установлена! Таймер пошёл...");

        isBombPlanted = true;
        isTimerRunning = true;
        currentTime = bombTimer;

        // Скрываем метку
        if (bombMarker != null)
            bombMarker.SetActive(false);

        // Показываем таймер
        if (timerPanel != null)
            timerPanel.SetActive(true);

        // Включаем сирену
        if (sirenAudio != null)
            sirenAudio.Play();

        UpdateTimerUI();
    }

    // Игрок подошёл к кораблю
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isTimerRunning && !isBombPlanted)
        {
            isPlayerNearShip = true;
            if (escapePrompt != null)
                escapePrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearShip = false;
            holdProgress = 0f;
            if (escapePrompt != null)
                escapePrompt.SetActive(false);
            UpdateHoldProgress();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void UpdateHoldProgress()
    {
        if (holdProgressBar != null)
            holdProgressBar.fillAmount = holdProgress;
    }

    void TriggerDefeat()
    {
        Debug.Log("?? Время вышло! Поражение...");

        if (sirenAudio != null)
            sirenAudio.Stop();

        // Загружаем сцену поражения
        SceneManager.LoadScene(loseSceneName);
    }

    void EscapeToWinScene()
    {
        Debug.Log("?? Побег успешен! Победа!");

        if (sirenAudio != null)
            sirenAudio.Stop();

        // Загружаем сцену победы
        SceneManager.LoadScene(winSceneName);
    }
}