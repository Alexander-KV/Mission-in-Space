using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipEscape : MonoBehaviour
{
    [Header("🔗 Ссылки")]
    public BombTimer bombTimer;           // Перетащи объект с BombTimer
    public ObjectiveKillEnemies mission;  // Перетащи объект миссии
    public GameObject escapePrompt;       // Текст "Нажми E чтобы улететь"

    [Header("⚙️ Настройки")]
    public float escapeHoldTime = 0f;     // 0 = мгновенно, >0 = нужно держать
    public string winSceneName = "WinScene";

    private bool isPlayerNear = false;
    private float holdProgress = 0f;

    void Start()
    {
        if (escapePrompt != null) escapePrompt.SetActive(false);
    }

    void Update()
    {
        // Проверяем условия для показа надписи:
        // 1. Игрок рядом
        // 2. Таймер существует и запущен (бомба стоит)
        bool canEscape = isPlayerNear && bombTimer != null && bombTimer.IsTimerRunning();

        if (escapePrompt != null)
            escapePrompt.SetActive(canEscape);

        // Обработка нажатия/удержания E
        if (canEscape && Input.GetKey(KeyCode.E))
        {
            if (escapeHoldTime <= 0f)
            {
                // Мгновенный побег
                EscapeSuccess();
            }
            else
            {
                // Прогресс удержания
                holdProgress += Time.deltaTime / escapeHoldTime;
                // Здесь можно добавить UI полоску, если нужно
                if (holdProgress >= 1f)
                    EscapeSuccess();
            }
        }
        else if (canEscape && escapeHoldTime > 0f)
        {
            // Сброс прогресса если отпустили E
            holdProgress = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            holdProgress = 0f;
        }
    }

    void EscapeSuccess()
    {
        Debug.Log("🚀 ПОБЕГ УДАЛСЯ!");

        // Останавливаем таймер и сирену
        if (bombTimer != null)
            bombTimer.StopTimer();

        // Завершаем миссию
        if (mission != null)
            mission.CompleteMissionAfterEscape();

        // Загружаем сцену победы
        Time.timeScale = 1f; // На всякий случай
        SceneManager.LoadScene(winSceneName);
    }
}