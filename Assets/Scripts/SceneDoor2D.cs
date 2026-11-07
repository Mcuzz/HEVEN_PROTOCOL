using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class SceneDoor2D : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private string _destinationScene;
    [SerializeField] private Rigidbody2D _player;
    [SerializeField] private Transform _arrivalPoint;
    [SerializeField] private float _centerTolerance = 0.18f;
    [SerializeField] private float _arrivalDelay = 0.6f;

    private static string _pendingArrival;
    private bool _loading;
    private float _readyAt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetTravel() { _pendingArrival = null; }

    private void Start()
    {
        _readyAt = Time.time + _arrivalDelay;
        if (_pendingArrival != gameObject.scene.name || _player == null || _arrivalPoint == null) return;
        _player.position = _arrivalPoint.position;
        _player.linearVelocity = Vector2.zero;
        _pendingArrival = null;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_loading || Time.time < _readyAt || _player == null || other.attachedRigidbody != _player) return;
        if (Mathf.Abs(_player.position.x - transform.position.x) > _centerTolerance) return;
        if (string.IsNullOrEmpty(_destinationScene) || !Application.CanStreamedLevelBeLoaded(_destinationScene))
        {
            Debug.LogError("La escena de destino de la puerta no está incluida en la compilación: " + _destinationScene, this);
            enabled = false;
            return;
        }
        _loading = true;
        _pendingArrival = _destinationScene;
        SceneManager.LoadSceneAsync(_destinationScene, LoadSceneMode.Single);
    }
}
