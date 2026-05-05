using UnityEngine;
using Unity.FPS.Gameplay;
using Unity.FPS.Game;

[RequireComponent(typeof(BoxCollider))]
public class VerticalDoor : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _doorVisuals;

    [Header("Настройки")]
    [SerializeField] private float _openHeight = 3.0f;
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private Vector3 _direction = Vector3.up;

    [Header("Авто-закрытие")]
    [SerializeField] private bool _autoClose = true;
    [SerializeField] private float _closeDelay = 10f;

    [Header("UI")]
    [SerializeField] private GameObject _promptUI;
    [SerializeField] private KeyCode _key = KeyCode.E;

    [Header("Блокировка")]
    public bool isLocked = true;   // дверь заперта? (по умолчанию – да)

    [Header("Блокировка по цели")]
    [Tooltip("Если указана цель, дверь будет заперта до её выполнения.")]
    public Objective requiredObjective;

    private bool _isOpen;
    private bool _isAnimating;
    private float _progress;
    private bool _playerNear;
    private float _openTime;

    private Vector3 _startPos;
    private Vector3 _endPos;

    private void Start()
    {
        if (_doorVisuals == null) _doorVisuals = transform;

        _startPos = _doorVisuals.position;
        _endPos = _startPos + _direction.normalized * _openHeight;

        if (_promptUI != null) _promptUI.SetActive(false);

        // Блокировка по цели
        if (requiredObjective != null)
        {
            Objective.OnObjectiveCompleted += OnSomeObjectiveCompleted;
            if (requiredObjective.IsCompleted)
            {
                isLocked = false;
            }
            else
            {
                isLocked = true;
            }
        }
    }

    private void OnSomeObjectiveCompleted(Objective obj)
    {
        if (obj == requiredObjective)
        {
            isLocked = false;
            Objective.OnObjectiveCompleted -= OnSomeObjectiveCompleted;
        }
    }

    private void Update()
    {
        // Показываем UI всегда, когда игрок рядом (независимо от блокировки)
        if (_promptUI != null)
        {
            _promptUI.SetActive(_playerNear);   // <-- ИСПРАВЛЕНИЕ
        }

        // Обработка нажатия E — только если дверь не заперта
        if (_playerNear && Input.GetKeyDown(_key) && !_isAnimating && !isLocked)
        {
            ToggleDoor();
        }

        // Авто-закрытие
        if (_autoClose && _isOpen && !_isAnimating)
        {
            if (Time.time - _openTime >= _closeDelay)
            {
                CloseDoor();
            }
        }

        // Анимация
        if (_isAnimating)
        {
            float target = _isOpen ? 1f : 0f;
            _progress = Mathf.MoveTowards(_progress, target, _speed * Time.deltaTime);
            _doorVisuals.position = Vector3.Lerp(_startPos, _endPos, _progress);

            if (Mathf.Approximately(_progress, target))
            {
                _isAnimating = false;
                _doorVisuals.position = _isOpen ? _endPos : _startPos;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNear = false;
        }
    }

    public void ToggleDoor()
    {
        if (_isAnimating || isLocked) return;
        _isOpen = !_isOpen;
        _isAnimating = true;
        if (_isOpen && _autoClose) _openTime = Time.time;
    }

    // Открыть (если закрыта) – может вызываться терминалом
    public void OpenDoor()
    {
        if (!_isOpen && !_isAnimating)
        {
            isLocked = false;    // разблокируем принудительно
            _isOpen = true;
            _isAnimating = true;
            if (_autoClose) _openTime = Time.time;
        }
    }

    public void CloseDoor()
    {
        if (_isOpen && !_isAnimating)
        {
            _isOpen = false;
            _isAnimating = true;
        }
    }

    void OnDestroy()
    {
        if (requiredObjective != null)
            Objective.OnObjectiveCompleted -= OnSomeObjectiveCompleted;
    }
}