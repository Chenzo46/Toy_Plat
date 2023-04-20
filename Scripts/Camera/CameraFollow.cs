using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = new Vector2();
    [SerializeField] private Vector2 DampItensity = new Vector2();

    [SerializeField] private bool freeCam = false;
    [SerializeField] private float freeCamSpeed = 2f;

    float rfr1;
    float rfr2;
    private void Awake()
    {
        
    }
    private void LateUpdate()
    {
        if (!freeCam)
        {
            //GameObject.Find("blueGuy").GetComponent<PlayerController>().enableMovement();
            transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, target.position.x + offset.x, ref rfr1, DampItensity.x),
            Mathf.SmoothDamp(transform.position.y, target.position.y + offset.y, ref rfr2, DampItensity.y),
            transform.position.z);
        }
        else
        {
            //GameObject.Find("blueGuy").GetComponent<PlayerController>().disableMovement();
            Vector2 m_Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            transform.position += (Vector3)m_Input * freeCamSpeed * Time.deltaTime;

            Camera.main.orthographicSize += -Input.GetAxisRaw("Mouse ScrollWheel");


        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            freeCam = !freeCam;
        }

        
    }

    public void changeTarget(Transform trgt)
    {
        target = trgt;
    }
}
