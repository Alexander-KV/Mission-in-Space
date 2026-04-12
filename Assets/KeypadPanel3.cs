using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class KeypadPanel3 : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text codeDisplayText;
    public GameObject panelObject;
    public TerminalTrigger terminalTrigger;   // ссылка на терминал

    [Header("Settings")]
    public string correctCode = "123";
    private string currentInput = "";

    [Header("Door Reference")]
    public SciFiDoor targetDoor;   // <-- ИСПРАВЛЕНО: тип SciFiDoor

    void Start()
    {
        if (panelObject != null)
            panelObject.SetActive(false);
    }

    public void OnNumberClick(string number)
    {
        if (currentInput.Length < 3)
        {
            currentInput += number;
            UpdateDisplay();
        }
    }

    public void OnEnterClick()
    {
        if (currentInput == correctCode)
        {
            Debug.Log("Код верный!");
            if (targetDoor != null)
                targetDoor.OpenDoor();

            if (terminalTrigger != null)
                terminalTrigger.OnCodeEnteredCorrectly();

            HidePanel();
        }
        else
        {
            Debug.Log("Неверный код!");
            currentInput = "";
            UpdateDisplay();
        }
    }

    public void OnClearClick()
    {
        currentInput = "";
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (codeDisplayText != null)
            codeDisplayText.text = currentInput;
    }

    public void ShowPanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(true);
            currentInput = "";
            UpdateDisplay();
        }
    }

    public void HidePanel()
    {
        if (panelObject != null)
            panelObject.SetActive(false);
    }
}