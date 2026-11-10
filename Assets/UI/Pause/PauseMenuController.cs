using UnityEngine;

public sealed class PauseMenuController : MonoBehaviour
{
    [Header("Interfaz")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private UnityEngine.UI.Button _resumeButton;

    private UnityEngine.InputSystem.InputAction _pause;
    private float _previousTimeScale;
    private bool _previousCursorVisible;
    private CursorLockMode _previousCursorLock;
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        _pause = new UnityEngine.InputSystem.InputAction(
            "Pause", UnityEngine.InputSystem.InputActionType.Button, "<Keyboard>/escape");
        _pause.performed += OnPause;
        _resumeButton.onClick.AddListener(Resume);
        _pausePanel.SetActive(false);
    }

    private void OnEnable() => _pause.Enable();

    private void OnPause(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (IsPaused)
            Resume();
        else if (Time.timeScale > 0f)
            Pause();
    }

    public void Pause()
    {
        if (IsPaused || Time.timeScale <= 0f)
            return;

        _previousTimeScale = Time.timeScale;
        _previousCursorVisible = Cursor.visible;
        _previousCursorLock = Cursor.lockState;
        IsPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _pausePanel.SetActive(true);
        _resumeButton.Select();
    }

    public void Resume()
    {
        if (!IsPaused)
            return;

        IsPaused = false;
        _pausePanel.SetActive(false);
        Time.timeScale = _previousTimeScale;
        Cursor.lockState = _previousCursorLock;
        Cursor.visible = _previousCursorVisible;
    }

    private void OnDisable()
    {
        _pause.Disable();
        Resume();
    }

    private void OnDestroy()
    {
        _resumeButton.onClick.RemoveListener(Resume);
        _pause.performed -= OnPause;
        _pause.Dispose();
    }
}
