
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public sealed class ScientistMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float _speed = 4f;

    [Header("Salto")]
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _body;
    private SpriteRenderer _renderer;
    private Animator _animator;

    private InputAction _move;
    private InputAction _jump;

    private float _direction;
    private bool _isGrounded;

    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int IsGroundedHash =
        Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        // Movimiento horizontal: A y D.
        _move = new InputAction(
            "Move",
            InputActionType.Value
        );

        _move.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");

        _move.performed += OnMove;
        _move.canceled += OnMove;

        // Salto: barra espaciadora.
        _jump = new InputAction(
            "Jump",
            InputActionType.Button,
            "<Keyboard>/space"
        );

        _jump.performed += OnJump;
    }

    private void OnEnable()
    {
        _move.Enable();
        _jump.Enable();
    }

    private void Update()
    {
        if (_groundCheck == null)
            return;

        _isGrounded = Physics2D.OverlapCircle(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer
        );
        if (_animator != null)
            _animator.SetBool(IsGroundedHash, _isGrounded);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<float>();

        if (_direction != 0f)
            _renderer.flipX = _direction < 0f;

        if (_animator != null)
            _animator.SetBool(IsMovingHash, _direction != 0f);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!_isGrounded)
            return;

        _body.linearVelocity = new Vector2(
            _body.linearVelocity.x,
            _jumpForce
        );

        _isGrounded = false;

        if (_animator != null)
            _animator.SetBool(IsGroundedHash, false);
    }

    private void FixedUpdate()
    {
        _body.linearVelocity = new Vector2(
            _direction * _speed,
            _body.linearVelocity.y
        );
    }

    private void OnDisable()
    {
        _move.Disable();
        _jump.Disable();

        _direction = 0f;

        if (_body != null)
        {
            _body.linearVelocity = new Vector2(
                0f,
                _body.linearVelocity.y
            );
        }
    }

    private void OnDestroy()
    {
        _move.Dispose();
        _jump.Dispose();
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            _groundCheck.position,
            _groundCheckRadius
        );
    }
}
