using System;
using UnityEngine;
using System.Collections;
using Cinemachine;

public class ArrowController : MonoBehaviour
{
    public Transform Target;
    private Vector3 startPosition;
    private float progress;
    private float speed = 5f;
    
 
    void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        progress += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(startPosition, Target.position, progress);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ArrowController => OnTriggerEnter");
        if (other.gameObject.GetComponent<ZombieController>())
        {
            Debug.Log("ArrowController => OnTriggerEnter ArrowController");
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("ArrowController => OnCollisionEnter");
        if (other.gameObject.GetComponent<ZombieController>())
        {
            Debug.Log("ArrowController => OnCollisionEnter ZombieController");

        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("ArrowController => OnControllerColliderHit");
        if (hit.gameObject.GetComponent<ZombieController>())
        {
            Debug.Log("OnControllerColliderHit ZombieController");

        }
    }
}
