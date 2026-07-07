using UnityEngine;

public class Player : MonoBehaviour
{
    // manager or a central hub
    public Rigidbody2D RB { get; private set;  }
    public Animator Anim { get; private set; }
    public SpriteRenderer SR { get; private set; }
    public Collider2D Collider { get; private set; }
    public Transform GroundCheck { get; private set; }

    public PlayerInputHandler Input { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerAnimator PlayerAnim { get; private set; }

    [SerializeField] private Transform groundCheckTransform;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        SR = GetComponentInChildren<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();
        GroundCheck = GetComponentInChildren<Transform>();

        Input = GetComponent<PlayerInputHandler>();
        Movement = GetComponent<PlayerMovement>();
        Combat = GetComponent<PlayerCombat>();
        PlayerAnim = GetComponent<PlayerAnimator>();
        StateMachine = GetComponent<PlayerStateMachine>();

        GroundCheck = groundCheckTransform;
    }

}
