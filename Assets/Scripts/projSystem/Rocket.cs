using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject ExplosionAnim;
    private GameObject canvasPlayer;
    public Transform ExplosionTransform;

    public static int movespeed = 300;
    public Vector3 userDirection = Vector3.right;

    private IEnumerable<Transform> targetObjects = new Transform[] { };
    private bool block;
    private RectTransform RT;
    private bool checkShip;
    private bool isTriger;

    void Start()
    {
        GameObject goPar = gameObject.transform.parent.gameObject;
        gameObject.GetComponent<Collider2D>().enabled = false;
        RT = gameObject.GetComponent<RectTransform>();
        if (RT != null && gameObject != null && targetObjects != null)
        {
            if (goPar.name == "Ship-Player")
            {
                targetObjects = GiveMeAllSlotGunOnShip("Enemy");
                checkShip = false;
            }
            if (goPar.name == "Ship-Enemy")
            {
                targetObjects = GiveMeAllSlotGunOnShip("Player1");
                checkShip = true;
            }
            StartCoroutine(ActiveCollider());
        }
        //RT = gameObject.GetComponent<RectTransform>();
        //Vector3 newV = GetGUIElementOffset(RT);
        //if (targetObjects.ToList().Count == 0)
        //{
        //    targetObjects = parentGO.GetComponentsInChildren<Transform>().Where(i => i.tag == "Enemy");
        //}
    }
    IEnumerable<Transform> GiveMeAllSlotGunOnShip(string nameShip)
    {
        GameObject parentGO = GameObject.FindGameObjectWithTag(nameShip);

        IEnumerable<Transform> target1 = parentGO.GetComponentsInChildren<Transform>().Where(i => i.tag == "SlotGunFull" && i.name =="BotGun");
        if (target1.ToList().Count > 0)
        {
            return target1;
        }
        IEnumerable<Transform> target2 = parentGO.GetComponentsInChildren<Transform>().Where(i => i.tag == "SlotGunFull" && i.name == "TopGun");
        if (target2.ToList().Count > 0)
        {
            return target2;
        }
        IEnumerable<Transform> target3 = parentGO.GetComponentsInChildren<Transform>().Where(i => i.tag == "SlotGun" && i.name == "BotGun");
        if (target3.ToList().Count > 0)
        {
            return target3;
        }
        IEnumerable<Transform> target4 = parentGO.GetComponentsInChildren<Transform>().Where(i => i.tag == "SlotGun" && i.name == "TopGun");
        if (target4.ToList().Count > 0)
        {
            return target4;
        }
        IEnumerable<Transform> target5 = parentGO.GetComponentsInChildren<Transform>().Where(i => i.tag == "SlotGun");
        return target5;

    }
    // collider2D not active when rocket created on scene ... f sec
    IEnumerator ActiveCollider()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        isTriger = true;
        gameObject.SetActive(false);
        GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform);
        expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);

        GameObject shipGO = other.gameObject.transform.parent.gameObject;
        

        if (other.gameObject.tag == "Player1" || other.gameObject.tag == "Enemy")
        {
            Ship go = other.gameObject.GetComponent<Ship>();
            if (go != null)
            {
                go.ShipTakeDamage(0.2f);
            }
        }
        //Invoke("DestroyRocketbyTime", 10);
    }

    //void DestroyRocketbyTime()
    //{
    //    Destroy(gameObject);
    //}

    // Update is called once per frame
    void Update()
    {
        if (!isTriger)
        {
            gameObject.transform.Translate(userDirection * movespeed * Time.deltaTime);
            RT = gameObject.GetComponent<RectTransform>();
            if (RT != null && gameObject != null && targetObjects != null)
            {
                Vector3 newV = GetGUIElementOffset(RT);
                if (!checkShip)
                {
                    // -615 & 675 coordinate where rocket don't see player camera
                    // need another way resolve this
                    // probably camera don't see rocket. RendererExtensions script didn't work for camera.main
                    if (newV.x < -615 && !block)
                    {
                        SearchTargetForRocket(-190);
                        block = true;
                    }
                }
                if (checkShip)
                {
                    if (newV.x > 675 && !block)
                    {
                        SearchTargetForRocket(1215);
                        block = true;
                    }
                }
            }
        }

        if (isTriger)
        {
            targetObjects = null;
        }
    }

    // -190 coordinate X for rocket start new position for attack enemy ship
    void SearchTargetForRocket(int poz)
    {
        int r = Random.Range(0, targetObjects.ToList().Count);
        if (targetObjects.ToList().Count>0)
        {
            Transform posTarget = targetObjects.ToList()[r].gameObject.transform;
            transform.position = new Vector2(poz, posTarget.transform.position.y);
        }
    }

    // edge coordinate system for object
    public static Vector3 GetGUIElementOffset(RectTransform rect)
    {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
        Vector3[] objectCorners = new Vector3[4];
        rect.GetWorldCorners(objectCorners);

        var xnew = 0f;
        var ynew = 0f;
        var znew = 0f;

        for (int i = 0; i < objectCorners.Length; i++)
        {
            if (objectCorners[i].x < screenBounds.xMin)
            {
                xnew = screenBounds.xMin - objectCorners[i].x;
            }
            if (objectCorners[i].x > screenBounds.xMax)
            {
                xnew = screenBounds.xMax - objectCorners[i].x;
            }
            if (objectCorners[i].y < screenBounds.yMin)
            {
                ynew = screenBounds.yMin - objectCorners[i].y;
            }
            if (objectCorners[i].y > screenBounds.yMax)
            {
                ynew = screenBounds.yMax - objectCorners[i].y;
            }
        }

        return new Vector3(xnew, ynew, znew);

    }
}
