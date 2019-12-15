using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameFunctions
{
    public static Transform GetCanvas()
    {
        return GameObject.Find(GameConstants.HUD_CANVAS).transform;
    }

    public static void SpawnUI(GameObject prefab, Transform parent, Vector3 pos)
    {
        //GameObject go = GameObject.Instantiate(prefab, parent);
        //go.transform.position = new Vector3(pos.x, 0, pos.z);
        //GameManager.AddObject(go);
    }
}
