using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public string noteTextstr;
    public GameObject notice;   // "Press E"
    public GameObject notePanel; // панель с текстом
    public Text text;

    private bool playerInZone = false;

    void Start()
    {
        if (notePanel != null)
            notePanel.SetActive(false);

        if (notice != null)
            notice.SetActive(false);
    }

    void Update()
    {
        // Открытие записки
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Open note");

            text.text = noteTextstr;
            notePanel.SetActive(true);
        }

        // Закрытие
        if (Input.GetKeyDown(KeyCode.T))
        {
            notePanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered note zone");

            playerInZone = true;
            notice.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            notice.SetActive(false);
            notePanel.SetActive(false);
        }
    }
}