using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public static int movespeed = 300;
    public Vector3 userDirection = Vector3.right;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(userDirection * movespeed * Time.deltaTime);
    }
}
