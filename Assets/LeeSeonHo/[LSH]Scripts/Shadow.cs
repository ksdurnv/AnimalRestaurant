using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float f = 1 - this.transform.position.y / 5;
        if (f < 0) f = 0;
        this.GetComponent<RectTransform>().localScale = new Vector3(f, f, f);
    }
}
