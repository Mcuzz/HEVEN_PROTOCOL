using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class AngelMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool MoveTowards(Transform target, float stoppingDistance)
    {
        if (target == null)
            return false;

        float directionX = target.position.x - rb.position.x;

        if (Mathf.Abs(directionX) <= stoppingDistance)
            return false;

        // El sprite original mira hacia la izquierda.
        spriteRenderer.flipX = directionX > 0;

        Vector2 nextPosition = rb.position;

        nextPosition.x = Mathf.MoveTowards(
            rb.position.x,
            target.position.x,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        return true;
    }
}