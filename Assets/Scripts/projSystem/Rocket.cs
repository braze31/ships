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
    GameObject goPar;
    Transform TargetTPonY;

    public bool rightTrigg;
    public bool leftTrigg;

    public bool CheckForEvent = false;

    void Start()
    {
        goPar = gameObject.transform.parent.gameObject;
        TargetTPonY = SearchTarget(goPar);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       // GameObject shipGO = other.gameObject.transform.parent.gameObject;
        if (other.name == "RightTrigger")
        {
            rightTrigg = true;
        }
        if (other.name == "LeftTrigger")
        {
            leftTrigg = true;
        }
        if (other.gameObject.tag == "Player1" || other.gameObject.tag == "Enemy")
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            gameObject.SetActive(false);
            GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform);
            expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
            Ship go = other.gameObject.GetComponent<Ship>();
            if (go != null)
            {
                go.ShipTakeDamage(0.1f);
            }
            Invoke("DestroyRocketbyTime", 10);
        }
        if ((other.gameObject.tag == "RocketP1" && gameObject.tag == "RocketP2") || 
            (other.gameObject.tag == "RocketP2" && gameObject.tag == "RocketP1"))
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            gameObject.SetActive(false);
            GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform);
            expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
            Invoke("DestroyRocketbyTime", 10);
        }
    }

    //void OnTriggerExit2D(Collider2D other)
    //{
    //    if (gameObject.transform.position.x < -100)
    //    {
    //        Debug.Log("SOMETHING WRONG THIS ROCKET");
    //        rightTrigg = true;
    //    }
    //    if (gameObject.transform.position.x > 1200)
    //    {
    //        Debug.Log("SOMETHING WRONG THIS ROCKET");
    //        rightTrigg = true;
    //    }
    //    CheckForEvent = true;
    //}

    IEnumerator ColliderNotActive(float time)
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    void DestroyRocketbyTime()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(userDirection * movespeed * Time.deltaTime);
        if (CheckForEvent)
        {
            if (rightTrigg)
            {
                if (TargetTPonY != null)
                {
                    gameObject.transform.position = new Vector3(-50, TargetTPonY.position.y);
                }
                rightTrigg = false;
            }
            if (leftTrigg)
            {
                if (TargetTPonY != null)
                {
                    gameObject.transform.position = new Vector3(1130, TargetTPonY.position.y);
                }
                leftTrigg = false;
            }
            CheckForEvent = false;
        }
    }

    Transform SearchTarget(GameObject goPar)
    {
        if (goPar.name == "Ship-Player")
        {
            var t = TargetOnShip("Ship-Enemy");
            if (t!=null)
            {
                return (Transform)t;
            }
            //gameObject.tag = "RocketP1";
            Transform target3 = GameObject.Find("Ship-Enemy").GetComponent<Transform>();
            return target3;
        }
        if (goPar.name == "Ship-Enemy")
        {
            var t = TargetOnShip("Ship-Player");
            if (t != null)
            {
                return (Transform)t;
            }
            //gameObject.tag = "RocketP2";
            Transform target3 = GameObject.Find("Ship-Player").GetComponent<Transform>();
            return target3;
        }
        return null;
    }

    Transform TargetOnShip(string shipPlayer)
    {
        GameObject SE = GameObject.Find(shipPlayer);
        Transform target1 = SE.transform.Find("TopGun").GetComponent<Transform>();
        if (target1.tag == "SlotGunFull")
        {
            return target1;
        }
        Transform target2 = SE.transform.Find("BotGun").GetComponent<Transform>();
        if (target2.tag == "SlotGunFull")
        {
            return target2;
        }

        return null;
    }
}
