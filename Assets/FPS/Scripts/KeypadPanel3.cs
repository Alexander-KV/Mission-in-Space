using TMPro;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class KeypadPanel3 : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text codeDisplayText;
    public GameObject panelObject;
    public TerminalTrigger terminalTrigger;

    [Header("Settings")]
    public string correctCode = "123";
    private string currentInput = "";

    [Header("Door Reference")]
    public SciFiDoor targetDoor;

    [Header("Player Control")]
    public GameObject player;
    public GameObject crosshair;  // перетащите сюда объект прицела (например, Crosshair)

    void Start()
    {
        if (panelObject != null)
            panelObject.SetActive(false);
    }

    void Update()
    {
        if (panelObject != null && panelObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                HidePanel();
            }
        }
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
            LockPlayer();
        }
    }

    public void HidePanel()
    {
        if (panelObject != null)
            panelObject.SetActive(false);
        UnlockPlayer();
    }

    void LockPlayer()
    {
        if (player != null)
        {
            // Отключаем ввод
            var input = player.GetComponent<PlayerInputHandler>();
            if (input != null) input.enabled = false;

            // Отключаем объект с оружием
            Transform weapon = player.transform.Find("Weapon_Blaster");
            if (weapon != null)
                weapon.gameObject.SetActive(false);
            else
                Debug.LogWarning("Weapon_Blaster not found on player");

            // Отключаем прицел, если он указан
            if (crosshair != null)
                crosshair.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void UnlockPlayer()
    {
        if (player != null)
        {
            var input = player.GetComponent<PlayerInputHandler>();
            if (input != null) input.enabled = true;

            Transform weapon = player.transform.Find("Weapon_Blaster");
            if (weapon != null)
                weapon.gameObject.SetActive(true);

            if (crosshair != null)
                crosshair.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}