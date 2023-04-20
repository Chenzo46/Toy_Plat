using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyGunnerBehaviour : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootDistance;
    [SerializeField] private Transform gunPos;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float bulletReloadTime = 1f;


    private Animator anim;
    private EnemyData emData;
    private Rigidbody2D rb2D;
    private PlayerSearch plrSearch;
    private SpriteRenderer spr;

    private bool canShoot = true;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        emData = GetComponent<EnemyData>();
        rb2D = GetComponent<Rigidbody2D>();
        plrSearch = GetComponent<PlayerSearch>();
        spr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            shoot();
        }
        */
    }

    private void FixedUpdate()
    {
        if (emData.inPursuit() && !emData.isTouchingPlayer() && !emData.recievingKnockBack && !inShootingDistance())
        {
            plrSearch.StopAllCoroutines();
            rb2D.velocity = new Vector2(emData.getPlayerDirection().x * speed * (Time.fixedDeltaTime*100), rb2D.velocity.y);
            anim.SetBool("isDriving", true);
        }
        else
        {
            anim.SetBool("isDriving", false);
        }

        if (plrSearch.searching)
        {
            rb2D.velocity = new Vector2(plrSearch.Direction.x * speed * (Time.fixedDeltaTime * 100), rb2D.velocity.y);
            anim.SetBool("isDriving", true);
            if (plrSearch.Direction == Vector2.right)
            {
                spr.flipX = false;
            }
            else if (plrSearch.Direction == Vector2.left)
            {
                spr.flipX = true;
            }
        }
        else if (!plrSearch.searching && !emData.inPursuit())
        {
            anim.SetBool("isDriving", false);
        }

        if (inShootingDistance() && canShoot && emData.inPursuit())
        {
            shoot();
            StartCoroutine(shootingTime());
        }

        
    }

    private bool inShootingDistance()
    {
        return Vector2.Distance(emData.getPlayerPosition(), transform.position) <= shootDistance;
    }
    private void shoot()
    {
        GameObject g = Instantiate(bullet, gunPos.position, bullet.transform.rotation);
        g.GetComponent<Rigidbody2D>().AddForce(emData.getPlayerDirection() * bulletSpeed, ForceMode2D.Impulse);
        anim.SetBool("Shooting", true);
    }

    public void doneShooting()
    {
        anim.SetBool("Shooting", false);
    }

    private IEnumerator shootingTime()
    {
        canShoot = false;
        yield return new WaitForSeconds(bulletReloadTime);
        canShoot = true;
    }
}
