using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieBehaviour : MonoBehaviour
{
    GameObject targetRef;
    Transform targetTransform;
    Vector3 currDir;
    [SerializeField]float moveSpeed = 15.0f;
    
    void Start()
    {
        targetRef = GameObject.FindGameObjectWithTag("Base");
        targetTransform = targetRef.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        currDir = targetTransform.position - transform.position;
        float distance = currDir.magnitude;
        Vector3 normDir = currDir / distance;
        transform.rotation = Quaternion.LookRotation(normDir);
      
    }

    private void FixedUpdate()
    {
        moveCharacter(currDir);
    }


    void moveCharacter(Vector3 direction)
     {
        transform.position += currDir.normalized * moveSpeed * Time.deltaTime;
     }
}