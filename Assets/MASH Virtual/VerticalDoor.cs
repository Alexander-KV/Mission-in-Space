using UnityEngine;

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
    }

    private void Update()
    {
        // Показываем/скрываем UI
        if (_promptUI != null)
        {
            _promptUI.SetActive(_playerNear);
        }

        // Обработка нажатия E — ВСЕГДА работает!
        if (_playerNear && Input.GetKeyDown(_key))
        {
            ToggleDoor();
        }

        // Авто-закрытие (только если дверь открыта и не анимируется)
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

    // ТОГГЛ: всегда переключает состояние (открыто  закрыто)
    public void ToggleDoor()
    {
        if (_isAnimating) return; // Ждём окончания анимации

        _isOpen = !_isOpen;
        _isAnimating = true;

        // Если открыли — запускаем таймер авто-закрытия
        if (_isOpen && _autoClose)
        {
            _openTime = Time.time;
        }
    }

    // Открыть (если закрыта)
    public void OpenDoor()
    {
        if (!_isOpen && !_isAnimating)
        {
            _isOpen = true;
            _isAnimating = true;
            if (_autoClose) _openTime = Time.time;
        }
    }

    // Закрыть (если открыта)
    public void CloseDoor()
    {
        if (_isOpen && !_isAnimating)
        {
            _isOpen = false;
            _isAnimating = true;
        }
    }
}