
using UnityEngine;
using UnityEngine.InputSystem;

public class ScientistCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.4f;

    private Animator _animator;
    private SpriteRenderer _renderer;
    private InputAction _shoot;
    private float _nextFireTime;

    private static readonly int ShootHash =
        Animator.StringToHash("Shoot");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();

        _shoot = new InputAction(
            "Shoot",
            InputActionType.Button,
            "<Keyboard>/f"
        );

        _shoot.performed += OnShoot;
    }

    private void OnEnable()
    {
        _shoot.Enable();
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (Time.time < _nextFireTime)
            return;

        Fire();
    }

    private void Fire()
    {
        if (_projectilePrefab == null || _firePoint == null)
            return;

        _nextFireTime = Time.time + _fireRate;

        if (_animator != null)
            _animator.SetTrigger(ShootHash);

        Vector2 direction = _renderer.flipX
            ? Vector2.left
            : Vector2.right;

        Vector3 spawnPosition = _firePoint.position;

        GameObject projectile = Instantiate(
            _projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        PlayerProjectile projectileScript =
            projectile.GetComponent<PlayerProjectile>();

        if (projectileScript != null)
            projectileScript.Initialize(direction);
    }

    private void OnDisable()
    {
        _shoot.Disable();
    }

    private void OnDestroy()
    {
        _shoot.Dispose();
    }
}
