using UnityEngine;

public class EnemyMovement_Basic : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float desiredDistance = 5f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 7f;
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D rb;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        Debug.Log("플레이어와의 거리: " + distance);

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

        if (distance > maxDistance)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else if (distance < minDistance)
        {
            rb.MovePosition(rb.position - direction * moveSpeed * Time.fixedDeltaTime);
        }
    }
}