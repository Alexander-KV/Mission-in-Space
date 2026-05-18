using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour
{
    [Header("Настройки кнопки")]
    public string action = "Resume";
    public string menuSceneName = "MainMenu";
    public GameObject pausePanel;

    public void OnButtonClick()
    {
        Debug.Log("КНОПКА НАЖАТА: " + action); // добавь эту строку

        
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        switch (action)
        {
            case "Resume":
                Time.timeScale = 1f;
                if (pausePanel != null) pausePanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case "MainMenu":
                Time.timeScale = 1f;
                SceneManager.LoadScene(menuSceneName);
                break;
            case "Quit":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    
}