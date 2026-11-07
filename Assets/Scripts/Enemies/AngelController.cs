using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AngelMovement))]
public class AngelController : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float stoppingDistance = 6.4f;
    [SerializeField] private string playerTag = "Player";

    private Animator animator;
    private AngelMovement movement;
    private Transform target;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<AngelMovement>();
    }

    private void FixedUpdate()
    {
        FindPlayer();

        bool isMoving = false;

        if (target != null)
        {
            // Como el juego es lateral, medimos únicamente la distancia horizontal.
            float horizontalDistance = Mathf.Abs(
                target.position.x - transform.position.x
            );

            if (horizontalDistance <= detectionRange)
            {
                isMoving = movement.MoveTowards(
                    target,
                    stoppingDistance
                );
            }
        }

        animator.SetBool("IsMoving", isMoving);
    }

    private void FindPlayer()
    {
        if (target != null)
            return;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null)
            target = player.transform;
    }
}