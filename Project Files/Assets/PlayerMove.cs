using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpd;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            if(!Input.GetKey(KeyCode.LeftArrow))
                transform.position = transform.position + Vector3.right * moveSpd * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
            transform.position = transform.position + Vector3.left * moveSpd * Time.deltaTime;
    }
}
