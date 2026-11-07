
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class AngelProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 20f;

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool initialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 travelDirection)
    {
        direction = travelDirection.normalized;
        initialized = true;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
            sprite.flipX = direction.x < 0;

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (!initialized)
            return;

        Vector2 nextPosition = rb.position +
            direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        CharacterHealth health =
            other.GetComponentInParent<CharacterHealth>();

        if (health != null)
            health.TakeDamage(damage);

        Destroy(gameObject);
    }
}
