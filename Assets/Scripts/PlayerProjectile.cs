
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private float _damage = 25f;

    private Rigidbody2D _body;
    private Vector2 _direction;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;
        Destroy(gameObject, _lifetime);
    }

    private void FixedUpdate()
    {
        _body.MovePosition(
            _body.position +
            _direction * _speed * Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        CharacterHealth health =
            other.GetComponentInParent<CharacterHealth>();

        if (health != null)
            health.TakeDamage(_damage);

        Destroy(gameObject);
    }
}
