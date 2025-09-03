using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsPanel;

    [SerializeField] private Button resumeButton;

    private bool isPaused = false;

    private void Update()
    {
        bool escPressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        // bool startPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        if (!isPaused && (escPressed /*|| startPressed*/))
        {
            Pause();
        }
    }

    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenOptions(bool show)
    {
        if (optionsPanel != null) optionsPanel.SetActive(show);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}