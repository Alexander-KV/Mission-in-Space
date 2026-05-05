using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour
{
    [Header("Управление")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI элементы")]
    public GameObject notice;
    public GameObject notePanel;

    private bool playerInZone = false;

    void Awake()
    {
        if (notePanel != null) notePanel.SetActive(false);
        if (notice != null) notice.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(ForceHideAfterFirstFrame());
    }

    IEnumerator ForceHideAfterFirstFrame()
    {
        yield return new WaitForEndOfFrame();
        if (notePanel != null) notePanel.SetActive(false);
        if (notice != null) notice.SetActive(false);
    }

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(interactKey))
        {
            if (notePanel != null)
            {
                bool isActive = notePanel.activeSelf;
                notePanel.SetActive(!isActive);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            if (notice != null) notice.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (notice != null) notice.SetActive(false);
            if (notePanel != null) notePanel.SetActive(false);
        }
    }
}