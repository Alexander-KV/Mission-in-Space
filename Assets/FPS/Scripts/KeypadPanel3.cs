using TMPro;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
    public GameObject crosshair;

    // ������ ��� �������� ��������� �����������, ������� �� ��������
    private List<MonoBehaviour> disabledComponents = new List<MonoBehaviour>();
    private List<GameObject> disabledWeapons = new List<GameObject>();
    private Rigidbody playerRigidbody;
    private bool wasKinematic;

    void Start()
    {
        if (panelObject != null)
            panelObject.SetActive(false);
    }

    void Update()
    {
        if (panelObject != null && panelObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);

            if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Escape))
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
            Debug.Log("��� ������!");
            if (targetDoor != null)
                targetDoor.OpenDoor();

            if (terminalTrigger != null)
                terminalTrigger.OnCodeEnteredCorrectly();

            HidePanel();
        }
        else
        {
            Debug.Log("�������� ���!");
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
        if (player == null) return;

        disabledComponents.Clear();
        disabledWeapons.Clear();

        // 1. ��������� ��� �������, ������� ������ �������� �� ����������
        MonoBehaviour[] allScripts = player.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in allScripts)
        {
            if (script == null || script == this) continue;

            // ��������� ������� ��������, ������, ������, ��������������
            string name = script.GetType().Name.ToLower();
            if (name.Contains("input") || name.Contains("character") ||
                name.Contains("weapon") || name.Contains("camera") ||
                name.Contains("interact") || name.Contains("motor"))
            {
                if (script.enabled)
                {
                    script.enabled = false;
                    disabledComponents.Add(script);
                }
            }
        }

        // 2. ��������� �������� ������� � ������� (���, � ���� � ����� ���� "Weapon" ��� "Gun")
        foreach (Transform child in player.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject != player &&
                (child.name.ToLower().Contains("weapon") || child.name.ToLower().Contains("gun")))
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                    disabledWeapons.Add(child.gameObject);
                }
            }
        }

        // 3. ��������� ������
        if (crosshair != null && crosshair.activeSelf)
        {
            crosshair.SetActive(false);
            disabledWeapons.Add(crosshair); // ����� �������� �������
        }

        // 4. ������������� ���������� ��������
        playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            wasKinematic = playerRigidbody.isKinematic;
            playerRigidbody.isKinematic = true;
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        // 5. ������������ ������
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void UnlockPlayer()
    {
        if (player == null) return;

        // �������� ������� ����������� �������
        foreach (var script in disabledComponents)
        {
            if (script != null)
                script.enabled = true;
        }
        disabledComponents.Clear();

        // �������� ������ � ������
        foreach (var obj in disabledWeapons)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        disabledWeapons.Clear();

        // ��������������� Rigidbody
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = wasKinematic;
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        // �������� ������ �������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}