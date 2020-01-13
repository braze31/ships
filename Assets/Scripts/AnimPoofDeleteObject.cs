using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPoofDeleteObject : MonoBehaviour
{
    void FixedUpdate()
    {
        Destroy(gameObject, 6.0f);
    }

}
