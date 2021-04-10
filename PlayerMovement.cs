using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 0f;
    [SerializeField] float movementMultiplyer = 10f;
    [SerializeField] float airMultiplyer = 0.4f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 15f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float crouchSpeed = 2f;
    [SerializeField] float acceleration = 10f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;
    [SerializeField] float slideDrag = 0;

    [Header("Player Stats")]
    [SerializeField] float playerHeight = 2f;
    [SerializeField] float crouchHeight = 1f;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    float groundDistance = 0.4f;

    float horizontalMovement;
    float verticalMovement;

    bool crouching;

    Vector3 slopeMoveDirection;

    RaycastHit slopeHit;

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }


    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        HandleCrouch();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplyer, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplyer, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplyer * airMultiplyer, ForceMode.Acceleration);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void HandleCrouch()
    {
        Vector3 crouchScale = new Vector3(transform.localScale.x, crouchHeight / playerHeight, transform.localScale.z);
        Vector3 normalScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        if (Input.GetKey(crouchKey))
        {
            crouching = true;
            transform.localScale = crouchScale;
        }
        else
        {
            crouching = false;
            transform.localScale = normalScale;
        }
    }

    void ControlDrag()
    {
        if (isGrounded && !crouching)
        {
            rb.drag = groundDrag;
        }
        else if (isGrounded && crouching)
        {
            rb.drag = slideDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }


}
