using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rocket : MonoBehaviourPun
{
    public GameObject ExplosionAnim;
    public Transform ExplosionTransform;

    public static int movespeed = 600;
    public Vector3 userDirection = Vector3.right;
    GameObject goPar;
    Transform TargetTPonY;

    public bool rightTrigg;
    public bool leftTrigg;

    public bool CheckForEvent = false;

    private List<Transform> allPoints = new List<Transform>() { };
    [SerializeField]
    public int countRSystem = 0;
    private bool exploydEvent = false;

    float angle;
    float difOnTrig;

    void Start()
    {
        goPar = gameObject.transform.parent.gameObject;
        TargetTPonY = SearchTargetForRocket(goPar);
        StartCoroutine(disableCollider());

        var csharp = new GameObject("MyGO", typeof(RectTransform));
        if (goPar.name == "Ship-Player-1")
        {
            float dif = GameObject.Find("Ship-Enemy").GetComponent<RectTransform>().transform.position.y-TargetTPonY.position.y+10f;
            float dif1 = GameObject.Find("Ship-Player").GetComponent<RectTransform>().transform.position.y;

            csharp.transform.position = new Vector3(1080 + TargetTPonY.position.x, dif1 - dif, transform.position.z);
            userDirection = Vector3.right;
            angle = Vector3.Angle(transform.right, csharp.transform.position - transform.position);
            if (csharp.transform.position.y > transform.position.y)
            {
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
            }
            else
            {
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -angle);
            }
        }
        if (goPar.name == "Ship-Player-2")
        {
            float dif = GameObject.Find("Ship-Player").GetComponent<RectTransform>().transform.position.y - TargetTPonY.position.y+10f;
            float dif1 = GameObject.Find("Ship-Enemy").GetComponent<RectTransform>().transform.position.y;

            csharp.transform.position = new Vector3(-1080+TargetTPonY.position.x, dif1 - dif, transform.position.z);
            userDirection = Vector3.left;
            angle = Vector3.Angle(transform.right, csharp.transform.position - transform.position);
            IEnumerable<Image> imageRotate = gameObject.GetComponentsInChildren<Image>().Where(i => i.name == "Image");

            imageRotate.First().GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 1f);
            if (csharp.transform.position.y > transform.position.y)
            {
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -angle);
            }
            else
            {
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
        Destroy(csharp, 15f);
    }

    IEnumerator disableCollider()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    void DestroyRocketbyTime()
    {
        Destroy(gameObject);
    }

    public void EXPLOYD()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<Collider2D>().enabled = false;
        GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform);
        GameObject col = GameObject.FindGameObjectWithTag("PlayerCanvas");
        expl.transform.SetParent(col.gameObject.transform);
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "RightTrigger" && goPar.name == "Ship-Player-1")
        {
            difOnTrig = GameObject.Find("Ship-Player").GetComponent<RectTransform>().transform.position.y - gameObject.transform.position.y;

            rightTrigg = true;
            //Invoke("DestroyRocketbyTime", 10);
            SendEvent();
        }
        if (col.gameObject.name == "LeftTrigger" && goPar.name == "Ship-Player-2")
        {
            difOnTrig = GameObject.Find("Ship-Enemy").GetComponent<RectTransform>().transform.position.y - gameObject.transform.position.y;

            leftTrigg = true;
            //Invoke("DestroyRocketbyTime", 10);
            SendEvent();
        }
        if (col.gameObject.tag == "Player1" || col.gameObject.tag == "Enemy" )
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            gameObject.SetActive(false);
            GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform);
            expl.transform.SetParent(col.gameObject.transform.parent.gameObject.transform);
            Ship go = col.gameObject.GetComponent<Ship>();
            if (go != null)
            {
                go.ShipTakeDamage(0.1f);
            }
            //Invoke("DestroyRocketbyTime", 10);
        }
        //if (col.gameObject.tag == "Rocket")
        //{
        //    gameObject.SetActive(false);
        //    gameObject.GetComponent<Collider2D>().enabled = false;
        //    GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform);
        //    expl.transform.SetParent(col.gameObject.transform.parent.gameObject.transform);
        //    exploydEvent = true;
        //    SendEvent();
        //}
    }

    public void SendEvent()
    {
        RaiseEventOptions options1 = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions1 = new SendOptions { Reliability = true };
        ExitGames.Client.Photon.Hashtable evData1 = new ExitGames.Client.Photon.Hashtable();

        GameObject gPar = gameObject.transform.root.gameObject;
        string nameViewIDPlayer = gPar.name.Substring(0, 4);

        evData1["playerID"] = nameViewIDPlayer;
        evData1["trig"] = -1;
        //evData1["pos"] = TargetTPonY.position.y;
        evData1["countRSystem"] = countRSystem;
        evData1["exploydEvent"] = exploydEvent;
        evData1["idRocket"] = gameObject.GetInstanceID();

        if (rightTrigg)
        {
            evData1["trig"] = 1;
        }
        if (leftTrigg)
        {
            evData1["trig"] = 0;
        }
        PhotonNetwork.RaiseEvent(3, evData1, options1, sendOptions1);
        
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
                    if (TargetTPonY.position.y == GameObject.Find("Ship-Player-2").GetComponent<RectTransform>().transform.position.y)
                    {
                        gameObject.transform.position = new Vector3(0, GameObject.Find("Ship-Player-2").GetComponent<RectTransform>().transform.position.y - difOnTrig);
                    }
                    else if((int)angle==0)
                    {
                        gameObject.transform.position = new Vector3(0,TargetTPonY.position.y);
                    }
                    else
                    {
                        gameObject.transform.position = new Vector3(0, GameObject.Find("Ship-Player-2").GetComponent<RectTransform>().transform.position.y);
                        userDirection = Vector3.right;
                        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
                        angle = Vector3.Angle(transform.right, TargetTPonY.transform.position - transform.position);
                        if (TargetTPonY.transform.position.y > transform.position.y)
                        {
                            gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
                        }
                        else
                        {
                            gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -angle);
                        }
                    }
                }
                rightTrigg = false;
            }
            if (leftTrigg)
            {

                if (TargetTPonY != null)
                {
                    if (TargetTPonY.position.y == GameObject.Find("Ship-Player-1").GetComponent<RectTransform>().transform.position.y)
                    {
                        gameObject.transform.position = new Vector3(1080, GameObject.Find("Ship-Player-1").GetComponent<RectTransform>().transform.position.y - difOnTrig);
                    }
                    else if ((int)angle == 0)
                    {
                        gameObject.transform.position = new Vector3(1080, TargetTPonY.position.y);
                    }
                    else
                    {
                        gameObject.transform.position = new Vector3(1080, GameObject.Find("Ship-Player-1").GetComponent<RectTransform>().transform.position.y);
                        userDirection = Vector3.right;
                        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
                        angle = Vector3.Angle(transform.right, TargetTPonY.transform.position - transform.position);
                        IEnumerable<Image> imageRotate = gameObject.GetComponentsInChildren<Image>().Where(i => i.name == "Image");

                        imageRotate.First().GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 1f);
                        if (TargetTPonY.transform.position.y > transform.position.y)
                        {
                            gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
                        }
                        else
                        {
                            gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -angle);
                        }
                    }
                }

                leftTrigg = false;
            }
            CheckForEvent = false;
        }
    }

    Transform SearchTargetForRocket(GameObject goPar)
    {
        if (goPar.name == "Ship-Player-1")
        {
            allPoints.Add(GameObject.Find("Ship-Enemy").transform);
            GameObject ES = GameObject.Find("Ship-Player-2");
            IEnumerable<RawImage> ESt = ES.gameObject.transform.GetComponentsInChildren<RawImage>()
                .Where(i=>i.name=="SlotBOT" || i.name=="SlotTOP");

            foreach (RawImage item in ESt)
            {
                //GameObject IRW = item.gameObject.transform.GetComponentInChildren<RawImage>().gameObject;
                //Debug.Log(IRW.transform.GetComponentInChildren<RawImage>().gameObject.tag);

                if (item.tag == "SlotGunFull")
                {
                    allPoints.Add(item.GetComponent<Transform>());
                }
            }
        }
        if (goPar.name == "Ship-Player-2")
        {
            allPoints.Add(GameObject.Find("Ship-Player").transform);
            GameObject PS = GameObject.Find("Ship-Player-1");

            IEnumerable<RawImage> PSt = PS.gameObject.transform.GetComponentsInChildren<RawImage>()
                .Where(i => i.name == "SlotBOT" || i.name == "SlotTOP");

            foreach (RawImage item in PSt)
            {
                //GameObject IRW = item.gameObject.transform.GetComponentInChildren<RawImage>().gameObject;
                //Debug.Log(IRW.transform.GetComponentInChildren<RawImage>().gameObject.tag);

                if (item.tag == "SlotGunFull")
                {
                    allPoints.Add(item.GetComponent<Transform>());
                }
            }
        }
        return allPoints.Last();
    }
}
