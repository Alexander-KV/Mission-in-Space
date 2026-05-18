using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Настройки UI")]
    public GameObject pauseMenuUI;
    public GameObject crosshair;

    private bool isPaused = false;

    void Awake()
    {
        // ГАРАНТИРОВАННО выключаем ещё до Start()
        if (pauseMenuUI != null && pauseMenuUI.activeSelf)
        {
            pauseMenuUI.SetActive(false);
            Debug.Log("⚠️ PauseMenu был включён в Awake() - выключаю!");
        }
    }

    void Start()
    {
        // Двойная проверка в Start()
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Debug.Log("✅ PauseMenu выключен в Start()");
        }

        Time.timeScale = 1f;

        if (crosshair != null)
            crosshair.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }

    void Update()
    {
        // Постоянно следим чтобы меню не включилось само
        if (!isPaused && pauseMenuUI != null && pauseMenuUI.activeSelf)
        {
            Debug.LogWarning("⚠️ PauseMenu включился сам по себе! Выключаю...");
            pauseMenuUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (crosshair != null)
            crosshair.SetActive(false);

        Debug.Log("▶️ Игра на паузе");
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);

        // 🔥 ФИКС: ждём 1 кадр, чтобы FPS-контроллер обновил состояние ввода
        StartCoroutine(RefreshFPSInput());
    }

    System.Collections.IEnumerator RefreshFPSInput()
    {
        yield return null; // Пропускаем 1 кадр (обязательно!)

        // Повторно блокируем курсор (это "будит" контроллер движения)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Сбрасываем стандартный курсор Windows (фикс для старых шаблонов)
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("IntroMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Кнопка выход нажата!"); // Это для проверки в консоли

#if UNITY_EDITOR
        // Эта строка останавливает игру в редакторе (нажми Play, чтобы вернуть)
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Эта строка закрывает игру в готовом файле .exe
            Application.Quit();
#endif
    }
}