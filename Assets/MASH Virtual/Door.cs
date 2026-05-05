using UnityEngine;
using TMPro;
using Unity.FPS.Gameplay;
using Unity.FPS.Game;

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
    public bool AutoClose = true;
    public float CloseDelay = 10f;

    [Header("UI")]
    public GameObject promptUI;
    public KeyCode interactKey = KeyCode.E;

    [Header("Безопасность")]
    public bool isLocked = true;   // дверь заперта? (по умолчанию — да)

    [Header("Блокировка по цели")]
    [Tooltip("Если указана цель, дверь будет заперта до её выполнения.")]
    public Objective requiredObjective;

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

        leftClosed = LeftDoor.localPosition;
        rightClosed = RightDoor.localPosition;

        Vector3 dir = MoveDirection.normalized;
        leftOpen = leftClosed - dir * OpenDistance;
        rightOpen = rightClosed + dir * OpenDistance;

        LeftDoor.localPosition = leftClosed;
        RightDoor.localPosition = rightClosed;

        if (promptUI != null) promptUI.SetActive(false);

        // Подписываемся на СТАТИЧЕСКОЕ событие завершения цели
        if (requiredObjective != null)
        {
            Objective.OnObjectiveCompleted += OnSomeObjectiveCompleted;

            // Если цель уже выполнена — сразу разблокируем
            if (requiredObjective.IsCompleted)
            {
                isLocked = false;
            }
            else
            {
                isLocked = true;   // гарантируем блокировку
            }
        }
    }

    // Обработчик статического события: срабатывает при завершении ЛЮБОЙ цели
    private void OnSomeObjectiveCompleted(Objective obj)
    {
        // Проверяем, что завершена именно наша цель
        if (obj == requiredObjective)
        {
            isLocked = false;
            // Отписываемся, чтобы не срабатывало повторно
            Objective.OnObjectiveCompleted -= OnSomeObjectiveCompleted;
        }
    }

    void Update()
    {
        if (promptUI != null) promptUI.SetActive(playerNear);

        // Если игрок рядом, нажата E, анимация не идёт и дверь НЕ заперта — открываем/закрываем
        if (playerNear && Input.GetKeyDown(interactKey) && !isAnimating && !isLocked)
        {
            ToggleDoor();
        }

        // Авто-закрытие
        if (AutoClose && isOpen && !isAnimating)
        {
            if (Time.time - openTime >= CloseDelay)
            {
                CloseDoor();
            }
        }

        // Анимация движения створок
        if (isAnimating)
        {
            float target = isOpen ? 1f : 0f;
            animProgress = Mathf.MoveTowards(animProgress, target, Speed * Time.deltaTime);

            LeftDoor.localPosition = Vector3.Lerp(leftClosed, leftOpen, animProgress);
            RightDoor.localPosition = Vector3.Lerp(rightClosed, rightOpen, animProgress);

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

    public void ToggleDoor()
    {
        if (isAnimating) return;
        isOpen = !isOpen;
        isAnimating = true;
        if (isOpen && AutoClose) openTime = Time.time;
    }

    // Метод, вызываемый из терминала при верном коде
    public void OpenDoor()
    {
        if (!isOpen && !isAnimating)
        {
            isLocked = false;   // снимаем блокировку
            isOpen = true;
            isAnimating = true;
            if (AutoClose) openTime = Time.time;
        }
    }

    public void CloseDoor()
    {
        if (isOpen && !isAnimating)
        {
            isOpen = false;
            isAnimating = true;
        }
    }

    void OnDestroy()
    {
        // Отписываемся от статического события при уничтожении объекта
        if (requiredObjective != null)
            Objective.OnObjectiveCompleted -= OnSomeObjectiveCompleted;
    }
}