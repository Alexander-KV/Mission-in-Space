using UnityEngine;
using UnityEngine.UI;

public class SimpleNote : MonoBehaviour
{
    [Header("Текст подсказки")]
    public string interactMessage = "Нажмите E чтобы прочитать записку";

    [Header("UI элементы")]
    public GameObject notePanel;
    public Text interactionUIText;

    private bool playerInRange = false;

    void Start()
    {
        // Принудительное выключение в первом кадре
        if (notePanel != null)
        {
            notePanel.SetActive(false);
        }

        // Дополнительная страховка - выключаем через кадр
        StartCoroutine(ForceHidePanelNextFrame());

        // Остальной код...
    }

    System.Collections.IEnumerator ForceHidePanelNextFrame()
    {
        yield return null; // Ждём один кадр
        if (notePanel != null)
        {
            notePanel.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowMessage(interactMessage);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideMessage();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenNote();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (notePanel != null && notePanel.activeSelf)
            {
                CloseNote();
            }
        }
    }

    void OpenNote()
    {
        if (notePanel != null)
        {
            notePanel.SetActive(true);
        }

        HideMessage();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseNote()
    {
        if (notePanel != null)
        {
            notePanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerInRange)
        {
            ShowMessage(interactMessage);
        }
    }

    void ShowMessage(string msg)
    {
        if (interactionUIText != null)
        {
            interactionUIText.text = msg;
            interactionUIText.gameObject.SetActive(true);
        }
    }

    void HideMessage()
    {
        if (interactionUIText != null)
        {
            interactionUIText.gameObject.SetActive(false);
            interactionUIText.text = "";
        }
    }
}