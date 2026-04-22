using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerminalDownloadSystem : MonoBehaviour
{
    [Header("️ Настройки")]
    public float downloadDuration = 300f; // 5 минут (для теста ставьте 10-20)
    public GameObject door;
    public Transform enemySpawnPoint;
    public GameObject enemyPrefab;
    public float spawnInterval = 12f;

    [Header("🎨 UI")]
    public GameObject uiPanel;       // Родительская панель (SetActive true/false)
    public Image progressBar;        // Image тип: Filled
    public Text statusText;          // Текст прогресса
    public Text missionText;         // Текст миссии (если есть на сцене)

    [Header("🔒 Внутреннее")]
    private bool isPlayerInRange = false;
    private bool isDownloading = false;
    private bool isCompleted = false;
    private Coroutine downloadRoutine;
    private Coroutine spawnRoutine;

    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
    }

    void Update()
    {
        // Старая система ввода (Input Manager)
        if (isPlayerInRange && !isDownloading && !isCompleted)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log(" E нажата! Запуск загрузки...");
                StartDownload();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("✅ Игрок вошёл в зону терминала");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("❌ Игрок покинул зону терминала");
        }
    }

    void StartDownload()
    {
        isDownloading = true;
        if (uiPanel != null) uiPanel.SetActive(true);
        UpdateUI(0f, "Загрузка чертежей... 0%");

        OpenDoor();

        if (enemySpawnPoint != null && enemyPrefab != null)
            spawnRoutine = StartCoroutine(SpawnEnemiesRoutine());

        downloadRoutine = StartCoroutine(DownloadTimerRoutine());
    }

    IEnumerator DownloadTimerRoutine()
    {
        float elapsed = 0f;
        while (elapsed < downloadDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / downloadDuration);
            UpdateUI(progress, $"Загрузка чертежей... {Mathf.RoundToInt(progress * 100)}%");
            yield return null;
        }
        CompleteDownload();
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        yield return new WaitForSeconds(2f); // Задержка перед первым врагом
        while (isDownloading && !isCompleted)
        {
            Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void UpdateUI(float progress, string status)
    {
        if (progressBar != null) progressBar.fillAmount = progress;
        if (statusText != null) statusText.text = status;
    }

    void CompleteDownload()
    {
        isDownloading = false;
        isCompleted = true;

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);

        UpdateUI(1f, "✅ Чертежи загружены!");
        if (missionText != null) missionText.text = "Миссия: Взорвать реактор";

        StartCoroutine(HideUIAfterDelay(3f));
    }

    IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (uiPanel != null) uiPanel.SetActive(false);
    }

    void OpenDoor()
    {
        if (door == null) return;
        Animator anim = door.GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Open");
        // Если аниматора нет, раскомментируй строку ниже:
        // door.transform.Translate(Vector3.up * 2.5f, Space.World);
    }
}
