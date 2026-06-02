using UnityEngine;
using UnityEngine.InputSystem;

/// Handles pausing during a run. Pressing Pause (Esc / gamepad Start) freezes the
/// world, flips GameManager state to Paused, and shows the pause overlay. Resume,
/// Restart, and Menu are wired to the panel's buttons.
public class PauseController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction;   // Player/Pause (Esc + Start)

    [Header("Panel")]
    [SerializeField] private GameObject panel;                   // the pause overlay (starts inactive)

    private void OnEnable()  => pauseAction.action.Enable();
    private void OnDisable() => pauseAction.action.Disable();

    private void Update()
    {
        if (!pauseAction.action.WasPressedThisFrame()) return;
        if (GameManager.Instance == null) return;

        // Only toggle between an active run and a paused run — ignore Menu / Game Over.
        if (GameManager.Instance.State == GameState.Playing) Pause();
        else if (GameManager.Instance.State == GameState.Paused) Resume();
    }

    private void Pause()
    {
        GameManager.Instance.SetState(GameState.Paused);
        Time.timeScale = 0f;
        if (panel != null) panel.SetActive(true);
    }

    /// Public so both the Resume button and Esc can call it.
    public void Resume()
    {
        if (GameManager.Instance != null) GameManager.Instance.SetState(GameState.Playing);
        Time.timeScale = 1f;
        if (panel != null) panel.SetActive(false);
    }

    /// Wired to the Restart button.
    public void OnRestart()
    {
        if (GameManager.Instance != null) GameManager.Instance.StartRun();  // resets timeScale + lives
        else Time.timeScale = 1f;
        SceneLoader.LoadGameplay();
    }

    /// Wired to the Menu button.
    public void OnMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.SetState(GameState.Menu);
        SceneLoader.LoadMainMenu();
    }
}