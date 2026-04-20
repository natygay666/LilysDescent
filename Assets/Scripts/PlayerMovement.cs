using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        Blocking
    }

    public PlayerState currentState;

    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 700f;
    public float jumpForce = 5f;

    [Header("References")]
    public Transform cameraTransform;
    public Transform blockSpawnPoint;
    public GameObject blockPrefab;

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded;
    private float currentSpeed;

    private GameObject currentBlock;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround();
        HandleState();
        HandleAnimations();
    }

    void HandleState()
    {
        // INPUT
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isMovingInput = (h != 0 || v != 0);
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool blocking = Input.GetMouseButton(2); // ruedita presionada

        // PRIORIDAD DE ESTADOS
        if (blocking)
        {
            ChangeState(PlayerState.Blocking);
        }
        else if (!isGrounded)
        {
            ChangeState(PlayerState.Jumping);
        }
        else if (isMovingInput)
        {
            ChangeState(PlayerState.Moving);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }

        // EJECUCIÓN
        switch (currentState)
        {
            case PlayerState.Idle:
                Idle();
                break;

            case PlayerState.Moving:
                Move(h, v);
                break;

            case PlayerState.Jumping:
                Move(h, v);
                if (jumpPressed && isGrounded)
                {
                    Jump();
                }
                break;

            case PlayerState.Blocking:
                Block();
                break;
        }
    }

    void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(newState);
    }

    void EnterState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Blocking:
                if (currentBlock == null)
                {
                    currentBlock = Instantiate(blockPrefab, blockSpawnPoint.position, blockSpawnPoint.rotation, transform);
                }
                break;
        }
    }

    void ExitState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Blocking:
                if (currentBlock != null)
                {
                    Destroy(currentBlock);
                }
                break;
        }
    }

    // ===== ESTADOS =====

    void Idle()
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    void Move(float h, float v)
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 direction = (camForward * v + camRight * h).normalized;

        Vector3 velocity = direction * speed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        float targetSpeed = direction.magnitude;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Block()
    {
        // Mientras bloquea, el jugador no se mueve
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        // Mantener el bloque enfrente
        if (currentBlock != null)
        {
            currentBlock.transform.position = blockSpawnPoint.position;
            currentBlock.transform.rotation = blockSpawnPoint.rotation;
        }
    }

    // ===== ANIMACIONES =====

    void HandleAnimations()
    {
        animator.SetFloat("Speed", currentSpeed);

        animator.SetBool("Idle", currentState == PlayerState.Idle);
        animator.SetBool("Moving", currentState == PlayerState.Moving);
        animator.SetBool("Jumping", currentState == PlayerState.Jumping);
        animator.SetBool("Blocking", currentState == PlayerState.Blocking);
    }

    // ===== GROUND =====

    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}