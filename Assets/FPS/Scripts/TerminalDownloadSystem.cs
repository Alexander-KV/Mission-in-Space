using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TerminalDownloadSystem : MonoBehaviour
{
    [Header("Настройки")]
    public float downloadDuration = 120f;
    public GameObject door;
    public Transform enemySpawnPoint;
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;

    [Header("UI")]
    public GameObject uiPanel;
    public Image progressBar;
    public TextMeshProUGUI statusText;   // ← заменён на TextMeshProUGUI
    public GameObject promptUI;

    private bool isPlayerInRange = false;
    private bool isDownloading = false;
    private bool isCompleted = false;
    private Coroutine downloadRoutine;
    private Coroutine spawnRoutine;

    void Awake()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(ForceHidePromptAfterFirstFrame());
    }

    IEnumerator ForceHidePromptAfterFirstFrame()
    {
        yield return new WaitForEndOfFrame();
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (promptUI != null)
        {
            bool shouldShow = isPlayerInRange && !isDownloading && !isCompleted;
            promptUI.SetActive(shouldShow);
        }

        if (isPlayerInRange && !isDownloading && !isCompleted)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartDownload();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
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
        yield return new WaitForSeconds(2f);
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

        UpdateUI(1f, "Чертежи загружены!");

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

        var sciFiDoor = door.GetComponent<SciFiDoor>();
        if (sciFiDoor != null)
        {
            sciFiDoor.OpenDoor();
            return;
        }

        var verticalDoor = door.GetComponent<VerticalDoor>();
        if (verticalDoor != null)
        {
            verticalDoor.OpenDoor();
            return;
        }

        Animator anim = door.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Open");
    }
}