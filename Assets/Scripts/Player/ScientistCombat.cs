
using UnityEngine;
using UnityEngine.InputSystem;

public class ScientistCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.4f;
    [SerializeField] private Camera _aimCamera;
    [SerializeField] private RectTransform _reticle;

    private Animator _animator;
    private SpriteRenderer _renderer;
    private InputAction _shoot;
    private InputAction _aim;
    private Collider2D[] _playerColliders;
    private bool _hasShootAnimation;
    private float _nextFireTime;
    private Vector2 _pointerPosition;
    private Vector3 _aimPoint;
    private Vector3 _firePointLocalPosition;
    private RectTransform _reticleRoot;
    private Canvas _reticleCanvas;

    private static readonly int ShootHash =
        Animator.StringToHash("Shoot");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            foreach (var parameter in _animator.parameters)
                if (parameter.nameHash == ShootHash)
                    _hasShootAnimation = true;
        }
        _renderer = GetComponent<SpriteRenderer>();
        _playerColliders = GetComponentsInChildren<Collider2D>();
        if (_firePoint != null)
            _firePointLocalPosition = _firePoint.localPosition;
        if (_reticle != null)
        {
            _reticleRoot = _reticle.parent as RectTransform;
            _reticleCanvas = _reticle.GetComponentInParent<Canvas>();
        }
        _aim = new UnityEngine.InputSystem.InputAction(
            "Aim", UnityEngine.InputSystem.InputActionType.Value, "<Mouse>/position");
        _aim.performed += OnAim;

        _shoot = new InputAction(
            "Shoot",
            InputActionType.Button,
            "<Mouse>/leftButton"
        );

        _shoot.performed += OnShoot;
    }

    private void OnEnable()
    {
        _aim.Enable();
        _pointerPosition = _aim.ReadValue<Vector2>();
        _shoot.Enable();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        _pointerPosition = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        if (_reticle != null)
        {
            bool visible = Time.timeScale > 0f;
            if (Cursor.visible == visible)
                Cursor.visible = !visible;
            if (_reticle.gameObject.activeSelf != visible)
                _reticle.gameObject.SetActive(visible);
            if (visible && _reticleRoot != null && _reticleCanvas != null)
            {
                Camera camera = _reticleCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null : _reticleCanvas.worldCamera;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _reticleRoot, _pointerPosition, camera, out Vector2 localPoint))
                    _reticle.anchoredPosition = localPoint;
            }
        }
        if (Time.timeScale > 0f)
            UpdateFacing();
    }

    private bool UpdateFacing()
    {
        if (_aimCamera == null || _firePoint == null)
            return false;
        Ray ray = _aimCamera.ScreenPointToRay(_pointerPosition);
        Plane plane = new Plane(Vector3.forward, transform.position);
        if (!plane.Raycast(ray, out float distance))
            return false;
        _aimPoint = ray.GetPoint(distance);
        if (Mathf.Abs(_aimPoint.x - transform.position.x) > 0.0001f)
            _renderer.flipX = _aimPoint.x < transform.position.x;
        Vector3 firePosition = _firePointLocalPosition;
        firePosition.x = Mathf.Abs(firePosition.x) * (_renderer.flipX ? -1f : 1f);
        _firePoint.localPosition = firePosition;
        return true;
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (Time.timeScale <= 0f || Time.time < _nextFireTime)
            return;

        Fire();
    }

    private void Fire()
    {
        if (_projectilePrefab == null || _firePoint == null || _aimCamera == null)
            return;

        _pointerPosition = _aim.ReadValue<Vector2>();
        if (!UpdateFacing())
            return;
        Vector3 spawnPosition = _firePoint.position;
        Vector2 direction = (Vector2)(_aimPoint - spawnPosition);
        if (direction.sqrMagnitude < 0.0001f)
            return;
        direction.Normalize();

        _nextFireTime = Time.time + _fireRate;

        if (_animator != null && _hasShootAnimation)
            _animator.SetTrigger(ShootHash);

        GameObject projectile = Instantiate(
            _projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        foreach (var projectileCollider in projectile.GetComponentsInChildren<Collider2D>())
        {
            foreach (var playerCollider in _playerColliders)
                Physics2D.IgnoreCollision(projectileCollider, playerCollider);
        }

        PlayerProjectile projectileScript =
            projectile.GetComponent<PlayerProjectile>();

        if (projectileScript != null)
            projectileScript.Initialize(direction);
    }

    private void OnDisable()
    {
        _aim.Disable();
        _shoot.Disable();
        if (_reticle != null)
        {
            _reticle.gameObject.SetActive(false);
            Cursor.visible = true;
        }
    }

    private void OnDestroy()
    {
        _aim.performed -= OnAim;
        _aim.Dispose();
        _shoot.Dispose();
    }
}
