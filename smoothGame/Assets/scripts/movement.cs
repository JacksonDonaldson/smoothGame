using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // movement speed
    [SerializeField] private float airspeed = 5f; // movement speed
    [SerializeField] private float drag = 1f;
    [SerializeField] private float jumpForce = 7f; // jump force
    [SerializeField] private LayerMask groundLayer; // layermask for detecting ground

    [SerializeField] private float groundCheckDistance = 0.7f; // radius of the circle used for ground detection
    [SerializeField] private float sprintMult = 1.5f;
    [SerializeField] private Vector2 wallJumpSpeed = new Vector2(5f,7f);

    private Rigidbody2D rb; // reference to the player's Rigidbody2D
    [SerializeField] private bool isGrounded = false; // flag for whether the player is grounded
    [SerializeField] private bool canJump = true; // flag for whether the player can jump
    private bool jumpWasPressed = false;
    [SerializeField] private bool canWallJump = false;
    private int wallDirection = 1;

    [SerializeField] private float gravityMult = 1.5f;
    [SerializeField] private float maxSlopeAngle = 45f;


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
            if(Math.Sign(playerForce - dragForce) == Math.Sign(playerForce) )
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
            if (isGrounded && canJump) // jump if the player is grounded and can jump
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                canJump = false;
            }
            else if (canWallJump)
            {
                float speed = wallJumpSpeed.x * wallDirection;
                rb.velocity = new Vector2(speed, wallJumpSpeed.y);
            }
        }
        jumpWasPressed = false;

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
                canJump = true;
            }
            if (Input.GetButtonDown("Jump"))
            {
                jumpWasPressed = true;
            }

        }
    void OnCollisionStay2D(Collision2D collision)
    {
        print(collision.gameObject.tag);
        if (collision.gameObject.tag == "WallJump")
        {
            if(collision.transform.position.x < transform.position.x)
            {
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


