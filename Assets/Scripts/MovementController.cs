using Photon.Pun;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Falling,
    Landing,
    Dead
}

/// <summary>
/// Player direction.
/// </summary>
public enum PlayerDirection
{
    Left,
    Right
}

public class MovementController : MonoBehaviourPun
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float airSpeed = 1f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private int jumpCounterValue = 1;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private Transform circle;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform healthBarTransform;
    
    private float moveInput;
    private bool isGrounded;

    /// <summary>
    /// jumpCounter is used to limit the number of jumps.
    /// </summary>
    private int jumpCounter;
    private bool isFacingRight = true;

    protected PlayerDirection PlayerDirection = PlayerDirection.Right;
    private bool isLeftButtonPressed = false;
    private bool isRightButtonPressed = false;
    private bool isJumpButtonPressed = false;
    private bool isWaitingForNextJumpClick = false;
    protected bool isControlEnabled = true;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        jumpCounter = jumpCounterValue;
    }
    
    //Movement related calucation
    protected virtual void FixedUpdate()
    {
        if (photonView.IsMine && isControlEnabled)
        {
            isGrounded = Physics2D.OverlapCircle(circle.position, 0.2f, ground);
            moveInput = isLeftButtonPressed ? -1 : isRightButtonPressed ? 1 : Input.GetAxis("Horizontal");
            var moveSpeed = isGrounded ? speed : airSpeed;
            rb.velocity = new Vector2(moveInput * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
       
            if((PlayerDirection == PlayerDirection.Right && moveInput < 0) || (PlayerDirection == PlayerDirection.Left && moveInput > 0))
                Flip();
        }
    }
    
    /// <summary>
    /// Notified through GameController when control is disabled
    /// </summary>
    /// <param name="pressed">if button is pressed or released</param>
    protected void OnLeftButtonClicked(bool pressed)
    {
        if (photonView.IsMine)
        {
            isLeftButtonPressed = pressed;
        }
    }
    
    /// <summary>
    /// Notified through GameController when control is disabled
    /// </summary>
    /// <param name="pressed">if button is pressed or released</param>
    protected void OnRightButtonClicked(bool pressed)
    {
        if (photonView.IsMine)
        {
            isRightButtonPressed = pressed;
        }
    }

    /// <summary>
    /// Notified through GameController when control is disabled
    /// </summary>
    /// <param name="pressed">if button is pressed or released</param>
    protected void OnJumpButtonClicked(bool pressed)
    {
        if (photonView.IsMine)
        {
            if (isWaitingForNextJumpClick && !pressed)//One we have received the click, we will wait for the pointer to be released before accepting the next click
            {
                isWaitingForNextJumpClick = false;
            }
            isJumpButtonPressed = pressed;
        }
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        if (isGrounded)
        {
            jumpCounter = jumpCounterValue;
        }
        if (jumpCounter > 0 && ((Input.GetKeyDown(KeyCode.Space)) || (isJumpButtonPressed && !isWaitingForNextJumpClick)))
        {
            isWaitingForNextJumpClick = true;
            jumpCounter--;
            rb.velocity = Vector2.up * jumpForce;
        }
    }

    /// <summary>
    /// Flips the player on direction change
    /// </summary>
    private void Flip()
    {
        if (PlayerDirection == PlayerDirection.Left)
        {
            PlayerDirection = PlayerDirection.Right;
        }
        else
        {
            PlayerDirection = PlayerDirection.Left;
        }
        
        var transform1 = transform;
        Vector3 scaler = transform1.localScale;
        scaler.x *= -1;
        transform1.localScale = scaler;
        var healthBarLocalScale = healthBarTransform.localScale;
        healthBarLocalScale.x *= -1;
        healthBarTransform.localScale = healthBarLocalScale;
    }
}
