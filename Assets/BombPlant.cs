using UnityEngine;
using TMPro;

public class BombPlant : MonoBehaviour
{
    [Header("Настройки")]
    public float interactDistance = 3f;       // дистанция для взаимодействия
    public GameObject bombObject;             // твоя бомба на сцене (выключенная)
    public GameObject promptUI;              // UI текст "Нажми E"
    public BombTimer bombTimer;              // ссылка на BombTimer

    private bool bombPlanted = false;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (bombPlanted) return;

        // Raycast из камеры
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        bool looking = Physics.Raycast(ray, out hit, interactDistance)
                       && hit.transform == this.transform;

        if (promptUI != null) promptUI.SetActive(looking);

        if (looking && Input.GetKeyDown(KeyCode.E))
        {
            PlantBomb();
        }
    }

    void PlantBomb()
    {
        bombPlanted = true;

        if (promptUI != null) promptUI.SetActive(false);

        // Включаем бомбу
        if (bombObject != null) bombObject.SetActive(true);

        // Запускаем таймер
        if (bombTimer != null) bombTimer.StartTimer();

        Debug.Log("Бомба установлена!");
    }
}
