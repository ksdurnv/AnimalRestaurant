using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinding1 : MonoBehaviour
{

    public Transform cubeA;
    public Transform cubeB;
    public Transform cubeC;
    

    public float cubeSpeed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 cubePosA = cubeA.position;
        Vector3 cubePosB = cubeB.position;
        Vector3 cubePosC = cubeC.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            StartCoroutine(Find());
        }
    }

    IEnumerator Find()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(cubeC.position, cubeA.position, Time.deltaTime * cubeSpeed);

            yield return new WaitForSeconds(.1f);
        }
        
    }
}
