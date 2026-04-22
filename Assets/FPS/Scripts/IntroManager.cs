using UnityEngine;
using System.Collections.Generic;

public class IntroManager : MonoBehaviour
{
    [Header("Ссылки")]
    public GameObject backstoryPanel;
    public GameObject introPanel;
    public GameObject player;
    public Canvas gameCanvas;

    [Header("Настройки")]
    public float backstoryDuration = 10f;
    public float missionDuration = 5f;

    private List<GameObject> hiddenUI = new List<GameObject>();
    private List<MonoBehaviour> disabledScripts = new List<MonoBehaviour>();
    private float timer;
    private int state = 0;

    void Start()
    {
        // Находим игрока
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        // Блокируем игрока
        if (player != null)
        {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script.enabled && script.GetType().Name != "IntroManager")
                {
                    disabledScripts.Add(script);
                    script.enabled = false;
                }
            }
        }

        // Скрываем ВСЕ Canvas в сцене кроме backstoryPanel
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            foreach (Transform child in canvas.transform)
            {
                if (child.gameObject != backstoryPanel && !IsChildOf(child, backstoryPanel))
                {
                    hiddenUI.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }

        // Запускаем таймер
        timer = backstoryDuration;
        state = 0;

        if (backstoryPanel != null)
            backstoryPanel.SetActive(true);
    }

    bool IsChildOf(Transform child, GameObject parent)
    {
        if (child == null || parent == null) return false;
        Transform current = child;
        while (current != null)
        {
            if (current.gameObject == parent) return true;
            current = current.parent;
        }
        return false;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (state == 0 && timer <= 0)
        {
            state = 1;
            timer = missionDuration;

            if (backstoryPanel != null)
                backstoryPanel.SetActive(false);

            // Возвращаем весь UI
            foreach (GameObject ui in hiddenUI)
            {
                if (ui != null)
                    ui.SetActive(true);
            }
            hiddenUI.Clear();

            // Разблокируем игрока
            foreach (MonoBehaviour script in disabledScripts)
            {
                if (script != null)
                    script.enabled = true;
            }
            disabledScripts.Clear();

            // Показываем миссию
            if (introPanel != null)
                introPanel.SetActive(true);
        }
        else if (state == 1 && timer <= 0)
        {
            state = 2;

            if (introPanel != null)
                introPanel.SetActive(false);
        }
    }
}