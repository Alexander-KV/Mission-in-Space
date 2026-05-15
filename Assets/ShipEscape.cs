using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.FPS.Gameplay;

public class SimpleWinTrigger : MonoBehaviour
{
    [Header("Настройки")]
    public string winSceneName = "WinScene";
    public GameObject promptText;

    [Header("🎯 Миссия")]
    public ObjectiveKillEnemies missionObjective;

    private bool isPlayerNear = false;

    void Start()
    {
        if (promptText != null) promptText.SetActive(false);
    }

    void Update()
    {
        // 1. Показываем надпись, если игрок в зоне И босс убит
        if (isPlayerNear && missionObjective != null && missionObjective.isBossKilled)
        {
            if (promptText != null && !promptText.activeSelf)
                promptText.SetActive(true);
        }

        // 2. Обрабатываем нажатие E
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (missionObjective != null && missionObjective.isBossKilled)
            {
                missionObjective.CompleteMissionAfterEscape();
                Debug.Log("🚀 Победа! Загрузка сцены...");
                Invoke(nameof(LoadWinScene), 1f);
            }
        }
    }

    void LoadWinScene()
    {
        SceneManager.LoadScene(winSceneName);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            // Проверяем сразу при входе
            if (missionObjective != null && missionObjective.isBossKilled && promptText != null)
                promptText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (promptText != null) promptText.SetActive(false);
        }
    }
}