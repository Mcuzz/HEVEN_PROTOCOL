
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class CharacterHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    private Animator _animator;
    private Rigidbody2D _body;
    private bool _isDead;

    // Propiedades públicas para consultar la vida desde el HUD.
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    private static readonly int HurtHash =
        Animator.StringToHash("Hurt");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();

        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead || damage <= 0f)
            return;

        _currentHealth = Mathf.Max(
            0f,
            _currentHealth - damage
        );

        UnityEngine.Debug.Log(
            $"{gameObject.name} recibió {damage} de daño. " +
            $"Vida restante: {_currentHealth}"
        );

        if (_currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (_animator != null)
            _animator.SetTrigger(HurtHash);
    }

    private void Die()
    {
        _isDead = true;

        UnityEngine.Debug.Log(
            $"{gameObject.name} ha sido derrotado."
        );

        if (_body != null)
        {
            _body.linearVelocity = Vector2.zero;
            _body.simulated = false;
        }

        Collider2D[] colliders =
            GetComponents<Collider2D>();

        foreach (Collider2D col in colliders)
            col.enabled = false;

        MonoBehaviour[] scripts =
            GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
    }
}
