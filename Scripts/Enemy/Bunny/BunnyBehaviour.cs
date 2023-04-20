using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyBehaviour : MonoBehaviour
{

    [SerializeField] private float jumpStrength = 5;
    [SerializeField] private float jumpTime = 1.2f;
    [SerializeField] private float h_Speed = 3;

    [SerializeField] private float checkRadius = 0.3f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;

    private Animator anim;

    private GameObject plr;


    private EnemyData em;
    private Rigidbody2D rb2D;

    private bool canJump = false;
    private bool cooldownOn = false;

    // Start is called before the first frame update
    void Awake()
    {
        em = GetComponent<EnemyData>();
        rb2D = GetComponent<Rigidbody2D>();
        plr = GameObject.FindGameObjectWithTag("Player");
        
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (em.inPursuit() && canJump && isGrounded())
        {
            rb2D.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            Vector2 Dir = plr.transform.position - transform.position;
            rb2D.AddForce(Dir * h_Speed, ForceMode2D.Impulse);
            canJump = false;
        }
        else if (!canJump && isGrounded() && !cooldownOn && em.inPursuit())
        {
            StartCoroutine(jumpCooldown());   
        }

        anim.SetBool("Grounded", isGrounded());
    }
    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground) != null;
    }

    IEnumerator jumpCooldown()
    {
        cooldownOn = true;
        yield return new WaitForSeconds(jumpTime);
        cooldownOn = false;
        canJump = true;
    }


}
