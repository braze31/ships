using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public RawImage test;
    private RawImage test1;
    public static bool isPRESS;
    bool isRed = false;
    public void TestRed()
    {
        Debug.Log("PRESS");
        test1 = test.GetComponent<RawImage>();
        if (!isRed)
        {
            test1.color = Color.red;
            isPRESS = true;
        }
        if(isPRESS == true)
        {
            test1.color = Color.white;

        }
    }
}
