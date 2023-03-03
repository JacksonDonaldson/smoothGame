using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class movement : MonoBehaviour
{

    //ground based movement
    [SerializeField] private float speed = 6f;
    [SerializeField] private float sprintMult = 1.5f;

    [SerializeField] private float airspeed = 55f; 
    [SerializeField] private float drag = 10f;
    [SerializeField] private float jumpForce = 7f; // jump force
    [SerializeField] private LayerMask groundLayer; // layermask for detecting ground

    [SerializeField] private float groundCheckDistance = 0.6f; // radius of the circle used for ground detection
    
    [SerializeField] private Vector2 wallJumpSpeed = new Vector2(10f,7f);

    private Rigidbody2D rb; // reference to the player's Rigidbody2D
    private bool isGrounded = false; // flag for whether the player is grounded
    [SerializeField] private int jumps = 0; // flag for number of double jumps
    
    private bool canWallJump = false;
    private int wallDirection = 1;

    [SerializeField] private float gravityMult = 1.5f;
    [SerializeField] private float maxSlopeAngle = 45f;

    [SerializeField] private float terminalVelocity = -10f;
    [SerializeField] private float wallTerminalVelocity = -5f;

    private bool jumpWasPressed = false;
    private bool leftWasPressed = false;
    private bool rightWasPressed = false;
    private bool upWasPressed = false;
    private bool downWasPressed = false;
    [SerializeField] private float horizontalPushForce = 1f;
    [SerializeField] private float upPushForce = 1f;
    [SerializeField] private float downPushForce = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // get the player's Rigidbody2D component
    }

    private void FixedUpdate()
    {
        

        float mult = 1;
        if (Input.GetButton("Sprint"))
        {
            mult = sprintMult;
        }

        float horizontalInput = Input.GetAxis("Horizontal"); // get horizontal input
        if (isGrounded)
        {
            rb.velocity = new Vector2(horizontalInput * speed * mult, rb.velocity.y); // move the player horizontally
        }
        else
        {
            float dragForce = drag * rb.velocity.x;
            float playerForce = airspeed * mult * horizontalInput;
            if(Math.Sign(playerForce - dragForce) == Math.Sign(playerForce) || playerForce == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + Time.deltaTime * horizontalInput * airspeed * mult - drag * rb.velocity.x * Time.deltaTime, rb.velocity.y); // move the player horizontally

            }
        }

        if(isGrounded && horizontalInput == 0)
        {
            rb.velocity = Vector2.zero;
        }
        

        if (horizontalInput < 0) // flip the player's sprite if moving left
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (horizontalInput > 0) // flip the player's sprite if moving right
        {
            transform.localScale = new Vector2(1, 1);
        }

        if (jumpWasPressed)
        {
            if (canWallJump)
            {
                float speed = wallJumpSpeed.x * wallDirection;
                rb.velocity = new Vector2(speed, wallJumpSpeed.y);
            }
            else if (isGrounded || jumps > 0) // jump if the player is grounded and can jump
            {
                
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                if (!isGrounded)
                {
                    jumps--;
                }
                
            }
        }
        jumpWasPressed = false;

        if (leftWasPressed)
        {
            rb.AddForce(Vector2.right * horizontalPushForce, ForceMode2D.Impulse);
        }
        if (rightWasPressed)
        {
            rb.AddForce(Vector2.left * horizontalPushForce, ForceMode2D.Impulse);
        }
        if (upWasPressed)
        {
            rb.AddForce(Vector2.down * downPushForce, ForceMode2D.Impulse);
        }
        if (downWasPressed)
        {
            rb.AddForce(Vector2.up * upPushForce, ForceMode2D.Impulse);
        }
        downWasPressed = false;
        upWasPressed = false;
        rightWasPressed = false;
        leftWasPressed = false;



        if (isGrounded)
        {
            rb.gravityScale = 0;
        }
        else if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityMult;
        }
        else
        {
            rb.gravityScale = 1;
        }

        float maxFallSpeed = terminalVelocity;
        if (canWallJump)
        {
            maxFallSpeed = wallTerminalVelocity;
        }
        if(rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }





    }





    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle <= maxSlopeAngle)
            {
                isGrounded = true;
                
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }


        if (isGrounded) // if the player is grounded, set canJump to true
        {
            jumps = 0;
        }


        if (Input.GetButtonDown("Jump"))
        {
            jumpWasPressed = true;
        }
        if (Input.GetButtonDown("ShootUp"))
        {
            upWasPressed = true;
        }
        if (Input.GetButtonDown("ShootLeft"))
        {
            leftWasPressed = true;
        }
        if (Input.GetButtonDown("ShootRight"))
        {
            rightWasPressed = true;
        }
        if (Input.GetButtonDown("ShootDown"))
        {
            downWasPressed = true;
        }


    }


    void OnCollisionStay2D(Collision2D collision)
    {
        //print(collision.gameObject.tag);
        if (collision.gameObject.tag == "WallJump")
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, groundCheckDistance, groundLayer);

            if(hit.collider != null)
            {
                print(hit.collider.gameObject);
                wallDirection = 1;
            }
            else
            {
                wallDirection = -1;
            }


            if (!isGrounded)
            {
                canWallJump = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "WallJump")
        {
            canWallJump = false;
        }
    }

}


