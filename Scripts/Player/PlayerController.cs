using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpStrength = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private LayerMask Ground;

    [SerializeField] private GameObject liftOff;
    [SerializeField] private Transform particleSpawn;

    [SerializeField] private float recoveryTime = 0.5f;

    [SerializeField] private bool GizmosOn = false;

    private bool slam = false;

    private SpriteRenderer spriteRend;
    private Rigidbody2D rb2D;
    private Animator anim;

    private bool canGetHit = true;

    private bool disableMov = false;

    private float moveInput;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!disableMov)
        {
            if (Input.GetButtonDown("Jump") && isGrounded())
            {
                GameObject g = Instantiate(liftOff, particleSpawn.position, liftOff.transform.rotation);
                g.transform.parent = null;
                rb2D.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            }
            else if (Input.GetButtonUp("Jump") && !isFalling())
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
            }

            if (rb2D.velocity.x > 0)
            {
                spriteRend.flipX = true;
            }
            else if (rb2D.velocity.x < 0)
            {
                spriteRend.flipX = false;
            }

            anim.SetBool("isGrounded", isGrounded());
            anim.SetBool("isFalling", isFalling());
            anim.SetBool("isRunning", isRunning());
        }
        

       

        if(slam && isGrounded())
        {
            GameObject g = Instantiate(liftOff, particleSpawn.position, liftOff.transform.rotation);
            g.transform.parent = null;
            slam = false;
        }
        else if (!isGrounded())
        {
            slam = true;
        }

         
        
       
    }

    private void FixedUpdate()
    {
        if (!disableMov)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            rb2D.velocity = new Vector2(moveInput * speed * (Time.fixedDeltaTime * 100), rb2D.velocity.y);
            //rb2D.MovePosition(new Vector2(transform.position.x + (moveInput * speed * (Time.fixedDeltaTime )), transform.position.y));
        }
        
    }

    private bool isRunning()
    {
        return moveInput != 0;
    }

    private bool isFalling()
    {
        return rb2D.velocity.y < 0.1;
    }

    public bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, Ground) != null;
    }

    private void OnDrawGizmos()
    {
        if (GizmosOn)
        {
            Gizmos.DrawWireCube(groundCheck.position, checkSize);
        }
        
    }

    public void disableMovement()
    {
        disableMov = true;
        
    }

    public void enableMovement()
    {
        disableMov = false;
    }

    public void dealDamage(Vector2 direction, float Magnitude)
    {
        if (canGetHit)
        {
            rb2D.velocity = Vector2.zero;
            rb2D.AddForce(direction * Magnitude, ForceMode2D.Impulse);
            StartCoroutine(damageRecovery());
        }
       
    }

    IEnumerator damageRecovery()
    {
        canGetHit = false;
        anim.SetBool("isDamaged", true);
        disableMovement();
        yield return new WaitForSeconds(recoveryTime);
        anim.SetBool("isDamaged", false);
        enableMovement();
        canGetHit = true;
    }
}
