using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TitleScreenController : MonoBehaviour
{
    [Header("Pantalla de inicio")]
    [SerializeField] private string _gameScene = "SampleScene";
    private UnityEngine.InputSystem.InputAction _start;
    private bool _loading;

    private void Awake()
    {
        _start = new UnityEngine.InputSystem.InputAction("StartGame", UnityEngine.InputSystem.InputActionType.Button);
        UnityEngine.InputSystem.InputActionSetupExtensions.AddBinding(_start, "<Keyboard>/enter");
        UnityEngine.InputSystem.InputActionSetupExtensions.AddBinding(_start, "<Keyboard>/numpadEnter");
        _start.performed += OnStart;
    }

    private void OnEnable()
    {
        _loading = false;
        _start.Enable();
    }

    private void OnStart(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_loading) return;
        if (!Application.CanStreamedLevelBeLoaded(_gameScene))
        {
            Debug.LogError("No se puede abrir la escena de juego: " + _gameScene, this);
            return;
        }
        _loading = true;
        _start.Disable();
        SceneManager.LoadSceneAsync(_gameScene, LoadSceneMode.Single);
    }

    private void OnDisable() { _start?.Disable(); }

    private void OnDestroy()
    {
        if (_start == null) return;
        _start.performed -= OnStart;
        _start.Dispose();
    }
}
