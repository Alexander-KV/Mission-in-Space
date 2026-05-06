using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;   // для Timeline катсцены
using TMPro;

public class ShipEscape : MonoBehaviour
{
    [Header("Настройки")]
    public float interactDistance = 3f;
    public float holdDuration = 3f;           // секунды удержания E

    [Header("Ссылки")]
    public BombTimer bombTimer;
    public PlayableDirector cutsceneDirector; // Timeline с катсценой

    [Header("UI")]
    public GameObject escapePrompt;          // "Удержи E чтобы улететь"
    public GameObject holdProgressPanel;     // панель с прогрессом
    public Image holdProgressBar;            // Image (Fill) прогресс-бар
    public GameObject victoryPanel;          // экран победы

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool escaped = false;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        if (escapePrompt != null) escapePrompt.SetActive(false);
        if (holdProgressPanel != null) holdProgressPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        if (escaped) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        bool looking = Physics.Raycast(ray, out hit, interactDistance)
                       && hit.transform == this.transform;

        if (escapePrompt != null) escapePrompt.SetActive(looking && !isHolding);

        if (looking && Input.GetKey(KeyCode.E))
        {
            // Начали/продолжаем удержание
            if (!isHolding)
            {
                isHolding = true;
                if (holdProgressPanel != null) holdProgressPanel.SetActive(true);
                if (escapePrompt != null) escapePrompt.SetActive(false);
            }

            holdTimer += Time.deltaTime;

            if (holdProgressBar != null)
                holdProgressBar.fillAmount = holdTimer / holdDuration;

            if (holdTimer >= holdDuration)
            {
                TriggerEscape();
            }
        }
        else if (isHolding)
        {
            // Отпустил — сбрасываем
            isHolding = false;
            holdTimer = 0f;
            if (holdProgressPanel != null) holdProgressPanel.SetActive(false);
        }
    }

    void TriggerEscape()
    {
        escaped = true;
        isHolding = false;

        if (holdProgressPanel != null) holdProgressPanel.SetActive(false);
        if (escapePrompt != null) escapePrompt.SetActive(false);

        // Останавливаем таймер бомбы
        if (bombTimer != null) bombTimer.StopTimer();

        // Запускаем катсцену
        if (cutsceneDirector != null)
        {
            cutsceneDirector.Play();
            cutsceneDirector.stopped += OnCutsceneFinished;
        }
        else
        {
            // Если катсцены нет — сразу победа
            ShowVictory();
        }

        Debug.Log("Побег начат!");
    }

    void OnCutsceneFinished(PlayableDirector director)
    {
        ShowVictory();
    }

    void ShowVictory()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Победа!");
    }
}
