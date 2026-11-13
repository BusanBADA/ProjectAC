using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyMovement_Basic : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float desiredDistance = 5f;
    [SerializeField] private float distanceRange = 1f;

    private Transform player;
    private Rigidbody2D rb;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > desiredDistance + distanceRange)
        {
            IsMoving = true;

            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else if (distance < desiredDistance - distanceRange)
        {
            IsMoving = true;

            Vector2 direction = (rb.position - (Vector2)player.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            IsMoving = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
}