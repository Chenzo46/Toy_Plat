using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBehaviour : MonoBehaviour
{
    [SerializeField] private float pushForce = 1;
    [SerializeField] private int punches = 4;

    private SpriteRenderer spriteRend;
    private Rigidbody2D rb2D;
    private PlayerController plr;
    private Animator anim;

    private bool firstClick = false;

    private float initalGravity;
    private int initialPunches;

    private bool cooldownOver = true;

    private void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        plr = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        initalGravity = rb2D.gravityScale;
        initialPunches = punches;
    }

    void Update()
    {
        if(punches > 0)
        {
            if (Input.GetButtonDown("Punch") && !firstClick)
            {
                if (isFacingLeft())
                {
                    punch(Vector2.left);
                }
                else if (!isFacingLeft())
                {
                    punch(Vector2.right);
                }

                

                firstClick = true;
                StartCoroutine(punchAfter());
            }
            else if (Input.GetButtonDown("Punch") && firstClick)
            {
                StopAllCoroutines();
                if (isFacingLeft())
                {
                    punch(Vector2.left);
                }
                else if (!isFacingLeft())
                {
                    punch(Vector2.right);
                }
                StartCoroutine(punchAfter());
            }
        }

        if (plr.isGrounded() && cooldownOver)
        {
            punches = initialPunches;
        }

        anim.SetBool("donePunching", cooldownOver);

    }

    private bool isFacingLeft()
    {
        return spriteRend.flipX == false;
    }

    private void punch(Vector2 Dir)
    {
        rb2D.velocity = Vector2.zero;
        rb2D.AddForce(Dir * pushForce, ForceMode2D.Force);
        rb2D.gravityScale = 0;
        punches--;
        plr.disableMovement();
        anim.SetTrigger("Punch");
        //anim.ResetTrigger("Punch");

    }

    IEnumerator punchAfter()
    {
        cooldownOver = false;
        yield return new WaitForSeconds(0.7f);
        cooldownOver = true;
        

        firstClick = false;

        rb2D.gravityScale = initalGravity;
        if (plr.isGrounded())
        {
            punches = initialPunches;
        }
        
        plr.enableMovement();
    }
}
