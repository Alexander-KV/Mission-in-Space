using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Unity.FPS.UI
{
    public class QuitGameButton : MonoBehaviour
    {
        private InputAction m_SubmitAction;

        void Start()
        {
            m_SubmitAction = InputSystem.actions.FindAction("UI/Submit");
            m_SubmitAction.Enable();
        }

        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject
                && m_SubmitAction.WasPressedThisFrame())
            {
                QuitGame();
            }
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}