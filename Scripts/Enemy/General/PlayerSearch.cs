using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSearch : MonoBehaviour
{
    //[Range(2,100)]
    //[SerializeField] private Vector2[] checkPoints = new Vector2[2];
    [SerializeField] private bool GizmosOn;
    [SerializeField] private float gizmoRadius;
    [SerializeField] private float checkPointWaitTime;
    [SerializeField] private Transform initialPos;

    [Range(2, 20)][SerializeField] private int searchDistance = 5;

    [SerializeField] private float raycastDistance = 0.05f;
    [SerializeField] private LayerMask rayMask;

    private ArrayList ValidPoints = new ArrayList(1);

    private EnemyData em;

    private bool updated = false;

    [HideInInspector]
    public bool searching = true;

    [HideInInspector]
    public Vector2 Direction = Vector2.right;

    private Vector2[] LG;

    private void Awake()
    {
        em = GetComponent<EnemyData>();
        //updatePath();
    }

    // Update is called once per frame
    void Update()
    {
        if(!em.inPursuit() && !updated)
        {
            updated = true;
            updatePath();
            StartCoroutine(waitToStart());
        }
        else if (em.inPursuit())
        {
            updated = false;
            searching = false;
        }

        //Debug.Log("Searching: " + searching) ;

        if (searching)
        {
            if (Vector2.Distance(LG[0], transform.position) <= 1f && Direction == Vector2.left)
            {
                //Debug.Log("left Reached");

                StartCoroutine(waitAtPoint());
            }
            else if (Vector2.Distance(LG[1], transform.position) <= 1f && Direction == Vector2.right)
            {
                //Debug.Log("right Reached");

                StartCoroutine(waitAtPoint());
            }
        }
    }

    public void updatePath()
    {
        //Right
        ValidPoints = new ArrayList();
        Vector2 currentPoint = initialPos.position;
        ValidPoints.Add(currentPoint);

        for(int i = 0; i < searchDistance; i++)
        {
            currentPoint += Vector2.right;

            //currentPoint = new Vector2(Mathf.Ceil(currentPoint.x), currentPoint.y);

            if (Physics2D.Raycast(currentPoint, Vector2.down, raycastDistance, rayMask) && !Physics2D.Raycast(currentPoint, Vector2.up, raycastDistance, rayMask))
            {
                ValidPoints.Add(currentPoint);
            }
            else
            {
                break;
            }
        }
        //Left
        currentPoint = initialPos.position;
        for (int i = 0; i < searchDistance; i++)
        {
            currentPoint += Vector2.left;

            //currentPoint = new Vector2(Mathf.Ceil(currentPoint.x), currentPoint.y);

            if (Physics2D.Raycast(currentPoint, Vector2.down, raycastDistance, rayMask) && !Physics2D.Raycast(currentPoint, Vector2.up, raycastDistance, rayMask))
            {
                ValidPoints.Add(currentPoint);
            }
            else
            {
                break;
            }
        }

        LG = leastandGreatest();

        updated = true;
    }

    private void OnDrawGizmos()
    {
        if (GizmosOn && updated)
        {

            foreach(Vector2 p in ValidPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(p, gizmoRadius);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(p, p - new Vector2(0, raycastDistance));
            }

        }
    }

    public Vector2[] leastandGreatest()
    {
        Vector2[] Points = new Vector2[2];


        Vector2 least = (Vector2)ValidPoints[0];
        Vector2 greatest = (Vector2)ValidPoints[0];
        for (int i = 0; i < ValidPoints.Count; i++)
        {
            if (least.x > ((Vector2)ValidPoints[i]).x)
            {
                least = (Vector2)ValidPoints[i];
            }
            if(greatest.x < ((Vector2)ValidPoints[i]).x)
            {
                greatest = (Vector2)ValidPoints[i];
            }
        }
        Points[0] = least;
        Points[1] = greatest;

        return Points;
    }

    public IEnumerator waitAtPoint()
    {
        searching = false;

        if (Direction == Vector2.right)
        {
            Direction = Vector2.left;
        }
        else
        {
            Direction = Vector2.right;
        }
         
        yield return new WaitForSeconds(checkPointWaitTime);

        searching = true;
    }

    public IEnumerator waitToStart()
    {
        yield return new WaitForSeconds(checkPointWaitTime);
        searching = true;
    }
}
