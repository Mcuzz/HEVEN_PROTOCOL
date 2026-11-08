
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over Settings")]
    [SerializeField] private CharacterHealth _playerHealth;
    [SerializeField] private GameObject _gameOverPanel;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (_playerHealth != null)
            _playerHealth.Died += ShowGameOver;
    }

    private void OnDisable()
    {
        if (_playerHealth != null)
            _playerHealth.Died -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        if (_gameOverPanel == null)
        {
            UnityEngine.Debug.LogError(
                "Game Over Panel no está asignado."
            );

            return;
        }

        _gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        UnityEngine.Debug.Log(
            "GAME OVER: El Científico ha sido derrotado."
        );
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

        UnityEngine.Application.Quit();

        UnityEngine.Debug.Log(
            "Solicitud de salida del juego."
        );
    }
}
