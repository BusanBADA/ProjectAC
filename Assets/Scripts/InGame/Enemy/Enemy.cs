using UnityEngine;

public class Enemy : MonoBehaviour
{
    private MechHull2D mechHull;
    private GameObject player;

    [SerializeField]
    private Transform weapon;

    [SerializeField]
    private float attackRange = 7f;

    private Weapon2D weaponController;

    [SerializeField]
    private float rotationSpeed = 120f;

    private EnemyMovement_Basic movement;

    private void Awake()
    {
        mechHull = GetComponent<MechHull2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        weaponController = weapon.GetComponent<Weapon2D>();
        movement = GetComponent<EnemyMovement_Basic>();
    }

    private void Update()
    {
        if (mechHull != null && mechHull.IsDead)
        {
            Destroy(gameObject);
        }

        if (player == null || weapon == null)
            return;

        Vector2 direction = (player.transform.position - weapon.position).normalized;

        Debug.DrawRay(weapon.position, direction * 3f, Color.green);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);

        weapon.rotation = Quaternion.RotateTowards(
            weapon.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        float distance = Vector2.Distance(
        transform.position,
        player.transform.position
        );

        bool canAttack = distance <= attackRange;

        if (weaponController != null && movement != null)
        {
            weaponController.SetCanFire(canAttack && !movement.IsMoving);
        }
    }
}