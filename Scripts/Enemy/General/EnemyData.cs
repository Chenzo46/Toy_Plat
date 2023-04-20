using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [SerializeField] private string e_Name;
    [SerializeField] private float health;

    [SerializeField] private float lookRange;
    [SerializeField] private bool drawGizmos;

    [SerializeField] private LayerMask Player;

    [SerializeField] private float pushBack = 100f;
    [SerializeField]private CapsuleCollider2D col2D;

    private SpriteRenderer s_Renderer;
    private Transform playerPosition;
    private Rigidbody2D rb2D;


    private float initialLookRange;

    private PlayerController plrController;

    [HideInInspector] public bool recievingKnockBack = false;

    private void Awake()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;

        initialLookRange = lookRange;

        s_Renderer = GetComponent<SpriteRenderer>();

        plrController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        col2D = GetComponent<CapsuleCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (inPursuit())
        {
            lookRange = initialLookRange * 2;
            lookAtPlayer();
            Debug.DrawRay(transform.position, plrController.gameObject.transform.position - transform.position);
        }
        else
        {
            lookRange = initialLookRange;
        }

        if (isTouchingPlayer())
        {
            rb2D.velocity = Vector2.zero;

            Transform plr = plrController.gameObject.transform;

            Vector2 ImpactDir = new Vector2();
            Vector2 Dir = new Vector2();
            if (plr.position.x < transform.position.x)
            {
                ImpactDir = Vector2.left;
            }
            else
            {
                ImpactDir = Vector2.right;
            }

            if(plr.position.y < transform.position.y)
            {
                Dir = ImpactDir;
            }
            else if (plr.position.y > transform.position.y)
            {
                Dir = (ImpactDir*10) + (Vector2)plr.position - (Vector2)transform.position;
            }
            else
            {
                Dir =  (Vector2)plr.position - (Vector2)transform.position;
            }
            recievingKnockBack = true;
            rb2D.AddForce(-Dir, ForceMode2D.Force);
            plrController.dealDamage(Dir.normalized, pushBack);
        }
        else if (rb2D.velocity.x == 0 && !isTouchingPlayer())
        {
            recievingKnockBack = false;
        }

       
    }

    public bool inPursuit()
    {

        return Physics2D.OverlapCircle(transform.position, lookRange, Player) != null;
    }

    public Vector2 getPlayerDirection()
    {
        if (playerPosition.position.x < transform.position.x)
        {
            return Vector2.left;
        }
        else
        {
            return Vector2.right;
        }
    }

    public Vector2 getPlayerPosition()
    {
        return playerPosition.position;
    }

    private void lookAtPlayer()
    {
        if(playerPosition.position.x < transform.position.x)
        {
            s_Renderer.flipX = true;
        }
        else
        {
            s_Renderer.flipX = false;
        }
    }

    public bool isTouchingPlayer()
    {
        return Physics2D.OverlapBox(col2D.bounds.center, col2D.size, 0f, Player)  != null;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (inPursuit())
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.blue;
            }

            Gizmos.DrawWireCube(transform.position, new Vector2(lookRange*2,lookRange));

            if (isTouchingPlayer())
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }
           

            Gizmos.DrawWireCube(col2D.bounds.center, col2D.size);
        }
    }
}
