using UnityEngine;

public class TerminalTrigger : MonoBehaviour
{
    public GameObject pressEPrompt;
    public KeypadPanel3 keypadPanel;
    public SciFiDoor targetDoor;          // дверь, которой управляет этот терминал

    private bool playerInRange = false;
    private bool codeAlreadyEntered = false;   // был ли уже введён верный код?

    void Start()
    {
        if (pressEPrompt != null)
            pressEPrompt.SetActive(false);

        // Если дверь уже разблокирована (например, после загрузки игры), считаем, что код введён
        if (targetDoor != null && !targetDoor.isLocked)
            codeAlreadyEntered = true;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (codeAlreadyEntered)
            {
                // Код уже был введён – просто переключаем дверь (открыть/закрыть)
                if (targetDoor != null)
                    targetDoor.ToggleDoor();
            }
            else
            {
                // Первый раз – показываем панель ввода
                if (keypadPanel != null)
                {
                    keypadPanel.ShowPanel();
                    if (pressEPrompt != null)
                        pressEPrompt.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressEPrompt != null && !codeAlreadyEntered) // подсказка только если код ещё не введён
                pressEPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressEPrompt != null)
                pressEPrompt.SetActive(false);
        }
    }

    // Вызывается из KeypadPanel3, когда код введён верно
    public void OnCodeEnteredCorrectly()
    {
        codeAlreadyEntered = true;
        if (pressEPrompt != null)
            pressEPrompt.SetActive(false); // убираем подсказку, т.к. больше не нужна
    }
}