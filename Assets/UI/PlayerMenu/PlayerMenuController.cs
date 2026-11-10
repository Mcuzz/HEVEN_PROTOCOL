using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PlayerMenuController : MonoBehaviour
{
    [Header("Menú")]
    [SerializeField] private TMPro.TMP_InputField _nameInput;
    [SerializeField] private UnityEngine.UI.Button _startButton;
    [SerializeField] private UnityEngine.UI.Button _quitButton;
    [SerializeField] private string _gameScene = "SampleScene";
    private bool _loading;

    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        string saved = PlayerProfile.PlayerName;
        _nameInput.SetTextWithoutNotify(saved == "Jugador" ? string.Empty : saved);
        _nameInput.onValueChanged.AddListener(OnNameChanged);
        _startButton.onClick.AddListener(StartGame);
        _quitButton.onClick.AddListener(QuitGame);
        OnNameChanged(_nameInput.text);
    }

    private void OnNameChanged(string value)
    {
        _startButton.interactable = !_loading && !string.IsNullOrWhiteSpace(value);
    }

    public void StartGame()
    {
        if (_loading || string.IsNullOrWhiteSpace(_nameInput.text))
            return;
        if (!Application.CanStreamedLevelBeLoaded(_gameScene))
        {
            Debug.LogError("No se puede cargar la escena: " + _gameScene, this);
            return;
        }
        PlayerProfile.SaveName(_nameInput.text);
        _loading = true;
        _startButton.interactable = false;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(_gameScene, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        _nameInput.onValueChanged.RemoveListener(OnNameChanged);
        _startButton.onClick.RemoveListener(StartGame);
        _quitButton.onClick.RemoveListener(QuitGame);
    }
}
