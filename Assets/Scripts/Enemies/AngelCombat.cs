
using UnityEngine;

public class AngelCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float projectileSpawnOffset = 0.6f;

    private Transform target;
    private Animator animator;
    private float nextAttackTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            target = player.transform;
    }

    private void Update()
    {
        if (target == null)
            return;

        float distance = Mathf.Abs(
            target.position.x - transform.position.x
        );

        if (distance > attackRange)
            return;

        if (Time.time < nextAttackTime)
            return;

        Attack();
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;

        animator.SetTrigger("Attack");

        Vector2 direction = (
            target.position - transform.position
        ).normalized;

        Vector3 spawnPosition = transform.position +
            (Vector3)(direction * projectileSpawnOffset);

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        AngelProjectile projectileScript =
            projectile.GetComponent<AngelProjectile>();

        if (projectileScript != null)
            projectileScript.Initialize(direction);
    }
}
