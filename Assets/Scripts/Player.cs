using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead;

   /* [Header("Speed Info")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMultiplier;
    [Space]
    [SerializeField] private float milestoneIncreaser;
    private float speedMilestone; */
   
    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private Vector2 knockbackDir;

    private bool canDoubleJump;

    private bool playerUnlocked;
    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCoolDown;
    private float slideCoolDownCounter;
    private float slideTimerCounter;
    private bool isSliding;


    [Header("Collsion info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceilingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;
    private bool wallDetected;
    private bool ceilingDetected;

  
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

      //  speedMilestone = milestoneIncreaser;
    }

    
    void Update()
    {
        AnimatorControllers();
        CheckCollision();

        slideTimerCounter -= Time.deltaTime;
        slideCoolDownCounter -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.O) && !isDead)
        {
            StartCoroutine(Die());
        }

        if (isDead)
        {
            return;
        }

        if (playerUnlocked)
        {
            Movement();
        }
        if (isGrounded)
        {
            canDoubleJump = true;
        }

      //  SpeedController();
        
        CheckInput();
        CheckForSlide();   
    }

    public void Damage()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        isDead = true;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(1f);
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(1f);
        GameManager.instance.RestartLevel();
    }



    /* private void SpeedController()

     {
         if(moveSpeed == maxSpeed) 
             return; 

         if(transform.position.x > speedMilestone)
         {
             speedMilestone = speedMilestone + milestoneIncreaser;
             moveSpeed = moveSpeed * speedMultiplier;
             milestoneIncreaser = milestoneIncreaser * speedMultiplier;

             if(moveSpeed > maxSpeed)
             {
                 moveSpeed = maxSpeed;
             }
         }
     } */
    private void CheckForSlide()
    {
        if(slideTimerCounter < 0 && !ceilingDetected)
        {
            isSliding = false;
        }
    }

    private void Movement()
    {
        if (wallDetected)
            return;
        if(isSliding)
        {
            rb.velocity = new Vector2 (slideSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
       
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            playerUnlocked = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            JumpButton();
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            SlideButton();
        }
    }

    private void SlideButton()
    {
        if (rb.velocity.x != 0 && slideCoolDownCounter < 0)
        {
            isSliding = true;
            slideTimerCounter = slideTime;
            slideCoolDownCounter = slideCoolDown;
        }
        
    }

    private void JumpButton()
    {
        if (isSliding)
            return;

        if (isGrounded)
        {
            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (canDoubleJump)
        {
            
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce); 
        }
        
    }

    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        ceilingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, 
            transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2 
            (transform.position.x, transform.position.y + ceilingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
       
    }

    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);

        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isDead", isDead);
       
        if(rb.velocity.y < -20)
        {
            anim.SetBool("canRoll", true);
        }
    }

    private void RollAnimFinished() => anim.SetBool("canRoll", false);
}
