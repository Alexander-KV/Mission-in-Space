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

    [Header("Авто-закрытие")]
    public bool AutoClose = true;          // Авто-закрытие включено?
    public float CloseDelay = 10f;         // Задержка перед закрытием (сек)

    [Header("UI")]
    public GameObject promptUI;
    public KeyCode interactKey = KeyCode.E;

    private bool isOpen = false;
    private bool isAnimating = false;
    private float animProgress = 0f;
    private bool playerNear = false;
    private float openTime = 0f;

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

        // Обработка нажатия E — ВСЕГДА работает (если не анимация)
        if (playerNear && Input.GetKeyDown(interactKey) && !isAnimating)
        {
            ToggleDoor();
        }

        // Авто-закрытие (только если дверь открыта и не анимируется)
        if (AutoClose && isOpen && !isAnimating)
        {
            if (Time.time - openTime >= CloseDelay)
            {
                CloseDoor();
            }
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

    // ТОГГЛ: всегда переключает состояние (открыто закрыто)
    public void ToggleDoor()
    {
        if (isAnimating) return; // Ждём окончания анимации

        isOpen = !isOpen;
        isAnimating = true;

        // Если открыли — запускаем таймер авто-закрытия
        if (isOpen && AutoClose)
        {
            openTime = Time.time;
        }
    }

    // Открыть (если закрыта)
    public void OpenDoor()
    {
        if (!isOpen && !isAnimating)
        {
            isOpen = true;
            isAnimating = true;
            if (AutoClose) openTime = Time.time;
        }
    }

    // Закрыть (если открыта)
    public void CloseDoor()
    {
        if (isOpen && !isAnimating)
        {
            isOpen = false;
            isAnimating = true;
        }
    }
}