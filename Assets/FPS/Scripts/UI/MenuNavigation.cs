using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class MenuNavigation : MonoBehaviour
    {
        [SerializeField] private Selectable DefaultSelection; // Добавлен атрибут SerializeField для видимости в инспекторе

        private InputAction m_SubmitAction;
        private InputAction m_NavigateAction;

        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Проверка наличия EventSystem
            if (EventSystem.current == null)
            {
                Debug.LogError("MenuNavigation: EventSystem не найден на сцене!");
                return;
            }

            EventSystem.current.SetSelectedGameObject(null);

            // Проверка наличия InputSystem
            if (InputSystem.actions == null)
            {
                Debug.LogError("MenuNavigation: InputSystem.actions не инициализирован!");
                return;
            }

            m_SubmitAction = InputSystem.actions.FindAction("UI/Submit");
            m_NavigateAction = InputSystem.actions.FindAction("UI/Navigate");

            // Проверка наличия DefaultSelection
            if (DefaultSelection == null)
            {
                Debug.LogWarning("MenuNavigation: DefaultSelection не назначен! " +
                               "Пожалуйста, назначьте объект в инспекторе.");
            }
        }

        void LateUpdate()
        {
            // Проверяем все необходимые ссылки
            if (EventSystem.current == null || DefaultSelection == null)
                return;

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                bool shouldSelect = false;

                // Безопасная проверка Input Actions
                if (m_SubmitAction != null)
                {
                    shouldSelect |= m_SubmitAction.WasPressedThisFrame();
                }

                if (m_NavigateAction != null)
                {
                    try
                    {
                        shouldSelect |= m_NavigateAction.ReadValue<Vector2>().sqrMagnitude != 0;
                    }
                    catch
                    {
                        // Игнорируем ошибки чтения Input Action
                    }
                }

                if (shouldSelect && DefaultSelection.gameObject != null)
                {
                    EventSystem.current.SetSelectedGameObject(DefaultSelection.gameObject);
                }
            }
        }

        // Для отладки - можно вызвать из инспектора
        public void ValidateReferences()
        {
            if (DefaultSelection == null)
                Debug.LogError("DefaultSelection не назначен!");
            else
                Debug.Log($"DefaultSelection назначен: {DefaultSelection.name}");

            if (EventSystem.current == null)
                Debug.LogError("EventSystem не найден на сцене!");
            else
                Debug.Log("EventSystem найден");
        }
    }
}