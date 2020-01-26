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

    void Start()
    {
        goPar = gameObject.transform.parent.gameObject;
        TargetTPonY = SearchTargetForRocket(goPar);
        StartCoroutine(disableCollider());
        //StartEvent();
    }

    //void StartEvent()
    //{
    //    RaiseEventOptions options2 = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    //    SendOptions sendOptions2 = new SendOptions { Reliability = true };
    //    ExitGames.Client.Photon.Hashtable evData2 = new ExitGames.Client.Photon.Hashtable();

    //    GameObject gPar = gameObject.transform.root.gameObject;
    //    string nameViewIDPlayer = gPar.name.Substring(0, 4);
    //    evData2["start"] = true;
    //    evData2["playerID"] = nameViewIDPlayer;
    //    evData2["pos"] = TargetTPonY.position.y;
    //    evData2["countRSystem"] = countRSystem;
    //    evData2["idRocket"] = gameObject.GetInstanceID();
    //    PhotonNetwork.RaiseEvent(3, evData2, options2, sendOptions2);
    //}

    IEnumerator disableCollider()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.3f);
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
        if (col.gameObject.name == "RightTrigger")
        {
            rightTrigg = true;
            Invoke("DestroyRocketbyTime", 10);
            SendEvent();
        }
        if (col.gameObject.name == "LeftTrigger")
        {
            leftTrigg = true;
            //Invoke("DestroyRocketbyTime", 10);
            SendEvent();
        }
        if (col.gameObject.tag == "Player1" || col.gameObject.tag == "Enemy")
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
        if (col.gameObject.tag == "Rocket")
        {
            gameObject.SetActive(false);
            gameObject.GetComponent<Collider2D>().enabled = false;
            GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform);
            expl.transform.SetParent(col.gameObject.transform.parent.gameObject.transform);
            exploydEvent = true;
            SendEvent();
        }
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
        evData1["pos"] = TargetTPonY.position.y;
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

    Transform SearchTargetForRocket(GameObject goPar)
    {
        if (goPar.name == "Ship-Player")
        {
            allPoints.Add(GameObject.Find("Ship-Enemy").transform);
            GameObject ES = GameObject.Find("Ship-Enemy");
            IEnumerable<Image> ESt = ES.gameObject.transform.GetComponentsInChildren<Image>().Where(i=>i.name=="TopGun" || i.name=="BotGun");
            foreach (Image item in ESt)
            {
                if (item.tag == "SlotGunFull")
                {
                    allPoints.Add(item.GetComponent<Transform>());
                }
            }
        }
        if (goPar.name == "Ship-Enemy")
        {
            allPoints.Add(GameObject.Find("Ship-Player").transform);
            GameObject PS = GameObject.Find("Ship-Player");

            IEnumerable<Image> PSt = PS.gameObject.transform.GetComponentsInChildren<Image>().Where(i => i.name == "TopGun" || i.name == "BotGun");
            foreach (Image item in PSt)
            {
                if (item.tag == "SlotGunFull")
                {
                    allPoints.Add(item.GetComponent<Transform>());
                }
            }
        }
        return allPoints.Last();
    }
}
