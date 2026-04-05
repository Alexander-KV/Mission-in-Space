using UnityEngine;
using TMPro;

public class SciFiDoor : MonoBehaviour
{
    [Header("Створки")]
    public Transform LeftDoor;
    public Transform RightDoor;

    [Header("Настройки")]
    public float OpenDistance = 2.5f;
    public float Speed = 3.0f;

    public Vector3 MoveDirection = new Vector3(1, 0, 0);

    [Header("UI")]
    public GameObject promptUI;
    public KeyCode interactKey = KeyCode.E;

    private bool isOpen = false;
    private bool isAnimating = false;
    private float animProgress = 0f;
    private bool playerNear = false;

    private Vector3 leftClosed, rightClosed;
    private Vector3 leftOpen, rightOpen;

    void Start()
    {
        if (LeftDoor == null || RightDoor == null) return;

        // 1. Запоминаем исходные (закрытые) позиции
        leftClosed = LeftDoor.localPosition;
        rightClosed = RightDoor.localPosition;

        // 2. Считаем открытые позиции
        Vector3 dir = MoveDirection.normalized;
        leftOpen = leftClosed - dir * OpenDistance;
        rightOpen = rightClosed + dir * OpenDistance;

        // 3. Принудительно ставим в закрытое состояние при старте
        LeftDoor.localPosition = leftClosed;
        RightDoor.localPosition = rightClosed;

        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        // Показываем UI, если игрок рядом
        if (promptUI != null) promptUI.SetActive(playerNear);

        // Обработка нажатия E (только если анимация не идёт)
        if (playerNear && Input.GetKeyDown(interactKey) && !isAnimating)
        {
            isOpen = !isOpen;
            isAnimating = true;
        }

        // Плавная анимация
        if (isAnimating)
        {
            float target = isOpen ? 1f : 0f;
            animProgress = Mathf.MoveTowards(animProgress, target, Speed * Time.deltaTime);

            LeftDoor.localPosition = Vector3.Lerp(leftClosed, leftOpen, animProgress);
            RightDoor.localPosition = Vector3.Lerp(rightClosed, rightOpen, animProgress);

            // Фиксация в конечной точке
            if (Mathf.Approximately(animProgress, target))
            {
                isAnimating = false;
                LeftDoor.localPosition = isOpen ? leftOpen : leftClosed;
                RightDoor.localPosition = isOpen ? rightOpen : rightClosed;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = false;
    }
}