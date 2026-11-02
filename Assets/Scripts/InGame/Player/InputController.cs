using UnityEngine;

public enum MoveType
{
    None,
    Leg,
    Fly
}

public class InputController : MonoBehaviour
{
    private MoveType moveType;

    Rigidbody2D playerRigid;
    public Vector2 inputVec;

    [SerializeField]
    private float limitWeightMaxSpeed;
    [SerializeField]
    private float normalMaxSpeed;
    [SerializeField]
    private float stopFric;

    private void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        limitWeightMaxSpeed = PlayerInfo.Instance.maxSpeed / 2;
        normalMaxSpeed = PlayerInfo.Instance.maxSpeed;
    }

    private void Update()
    {
        inputVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (inputVec != Vector2.zero)
        {
            Vector2 velocity = playerRigid.linearVelocity;

            if (velocity.magnitude > 0f)
            {
                float dot = Vector2.Dot(inputVec, velocity.normalized);
                if (dot < 0)
                {
                    playerRigid.linearVelocity *= stopFric;  //new Vector2(0,0);//*=-dot
                }
            }
            Vector2 force = inputVec * PlayerInfo.Instance.axel;
            playerRigid.AddForce(force, ForceMode2D.Force);
        }
        else
        {
            playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.x * stopFric, playerRigid.linearVelocity.y * stopFric);
        }
    }
}