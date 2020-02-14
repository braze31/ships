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

    public static int movespeed = 800;
    public Vector3 userDirection = Vector3.right;
    GameObject goPar;
    Transform TargetForRocket;

    public bool rightTrigg;
    public bool leftTrigg;

    public bool CheckForEvent = false;

    private List<Transform> allPoints = new List<Transform>() { };
    [SerializeField]
    public int countRSystem = 0;
    private bool exploydEvent = false;

    float angle;

    public float SystemRocketDamage = 0.3f;
    public float ShipRocketDamage = 0.1f;
    public float ShipAndSystemRocketDamage = 0.05f;

    //public RectTransform ExplosionTransform;

    void Start()
    {
        goPar = gameObject.transform.parent.gameObject;
        TargetForRocket = SearchTargetForRocket(goPar);
        if (TargetForRocket.name=="Ship-Player" || TargetForRocket.name == "Ship-Enemy")
        {
            gameObject.tag = "RocketS";
        }

        StartCoroutine(disableCollider());
        var csharp = new GameObject("MyGO", typeof(RectTransform));
        if (goPar.name == "Ship-Player-1")
        {
            float dif1 = GameObject.Find("Ship-Player").GetComponent<RectTransform>().transform.position.y;

            csharp.transform.position = new Vector3(1080 + TargetForRocket.position.x, dif1, transform.position.z);
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
            float dif1 = GameObject.Find("Ship-Enemy").GetComponent<RectTransform>().transform.position.y;

            csharp.transform.position = new Vector3(-1080+TargetForRocket.position.x, dif1, transform.position.z);
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
        GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform.position, Quaternion.Euler(1f, 1f, 0f));
        expl.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0f);
        GameObject col = GameObject.FindGameObjectWithTag("PlayerCanvas");
        expl.transform.SetParent(col.gameObject.transform);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "RightTrigger" && goPar.name == "Ship-Player-1")
        {
            rightTrigg = true;
            //Invoke("DestroyRocketbyTime", 10);
            SendEvent();
        }
        if (other.gameObject.name == "LeftTrigger" && goPar.name == "Ship-Player-2")
        {
            leftTrigg = true;
            //Invoke("DestroyRocketbyTime", 10);
            SendEvent();
        }
        if (other.name == TargetForRocket.name && (other.name != "Ship-Player" || other.name != "Ship-Enemy"))
        {
            gameObject.SetActive(false);
            GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform.position, Quaternion.Euler(0f, 0f, 0f));
            expl.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0f);
            expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
            
            DropZone dz = other.GetComponent<DropZone>();
            dz.healthBar.ReduceHPSystem(SystemRocketDamage);
            Ship go = other.transform.parent.gameObject.GetComponent<Ship>();
            go.ShipTakeDamage(ShipAndSystemRocketDamage);
        }
        if (gameObject.tag =="RocketS" && other.name == "Ship-Player-1"
            || gameObject.tag == "RocketS" && other.name == "Ship-Player-2")
        {
            Ship go = other.gameObject.GetComponent<Ship>();
            if (go != null)
            {
                go.ShipTakeDamage(ShipRocketDamage);
            }
            gameObject.SetActive(false);
            GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform.position, Quaternion.Euler(0f, 0f, 0f));
            expl.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0f);
            expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
        }
        //    //if (col.gameObject.tag == "Rocket")
        //    //{
        //    //    gameObject.SetActive(false);
        //    //    gameObject.GetComponent<Collider2D>().enabled = false;
        //    //    GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform);
        //    //    expl.transform.SetParent(col.gameObject.transform.parent.gameObject.transform);
        //    //    exploydEvent = true;
        //    //    SendEvent();
        //    //}
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
        evData1["tag"] = gameObject.tag;
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
                if (TargetForRocket != null)
                {
                    var csharp = new GameObject("MyGOUP", typeof(RectTransform));
                    TargetForRocket = SearchTargetForRocket(goPar);
                    if (TargetForRocket.name == "Ship-Player" || TargetForRocket.name == "Ship-Enemy")
                    {
                        gameObject.tag = "RocketS";
                        SendEvent();
                    }

                    gameObject.transform.position = new Vector3(0, GameObject.Find("Ship-Player-2").GetComponent<RectTransform>().transform.position.y);
                    csharp.transform.position = new Vector3(TargetForRocket.transform.position.x, TargetForRocket.transform.position.y, TargetForRocket.transform.position.z);
                    
                    gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);

                    angle = Vector3.Angle(transform.right, csharp.transform.position - transform.position);
                    if (TargetForRocket.transform.position.y > transform.position.y)
                    {
                        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
                    }
                    else
                    {
                        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -angle);
                    }
                    Destroy(csharp, 15f);
                }
                rightTrigg = false;
            }
            if (leftTrigg)
            {

                if (TargetForRocket != null)
                {
                    var csharp = new GameObject("MyGODOWN", typeof(RectTransform));
                    TargetForRocket = SearchTargetForRocket(goPar);
                    if (TargetForRocket.name == "Ship-Player" || TargetForRocket.name == "Ship-Enemy")
                    {
                        gameObject.tag = "RocketS";
                        SendEvent();
                    }

                    gameObject.transform.position = new Vector3(1080, GameObject.Find("Ship-Player-1").GetComponent<RectTransform>().transform.position.y);
                    csharp.transform.position = new Vector3(TargetForRocket.transform.position.x, TargetForRocket.transform.position.y, TargetForRocket.transform.position.z);
                    
                    userDirection = Vector3.right;
                    gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
                    angle = Vector3.Angle(transform.right, csharp.transform.position - transform.position);
                    IEnumerable<Image> imageRotate = gameObject.GetComponentsInChildren<Image>().Where(i => i.name == "Image");

                    imageRotate.First().GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 1f);
                    if (TargetForRocket.transform.position.y > transform.position.y)
                    {
                        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
                    }
                    else
                    {
                        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -angle);
                    }
                    Destroy(csharp, 15f);
                }
                leftTrigg = false;
            }
            CheckForEvent = false;
        }
    }

    Transform SearchTargetForRocket(GameObject goPar)
    {
        float findBiggerValueofFillAmountSystem = 0;
        if (goPar.name == "Ship-Player-1")
        {
            allPoints.Add(GameObject.Find("Ship-Enemy").transform);
            GameObject ES = GameObject.Find("Ship-Player-2");
            IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().rt;
            foreach (RectTransform item in ESt)
            {
                if (findBiggerValueofFillAmountSystem<=item.GetComponent<DropZone>().healthBar.bar.fillAmount)
                {
                    if (item.tag == "SlotGunFull")
                    {
                        allPoints.Add(item.GetComponent<RectTransform>());
                        findBiggerValueofFillAmountSystem = item.GetComponent<DropZone>().healthBar.bar.fillAmount;
                    }
                }
            }
        }
        if (goPar.name == "Ship-Player-2")
        {
            allPoints.Add(GameObject.Find("Ship-Player").transform);
            GameObject PS = GameObject.Find("Ship-Player-1");
            IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().rt;
            foreach (RectTransform item in PSt)
            {
                if (item.tag == "SlotGunFull")
                {
                    if (findBiggerValueofFillAmountSystem <= item.GetComponent<DropZone>().healthBar.bar.fillAmount)
                    {
                        if (item.tag == "SlotGunFull")
                        {
                            allPoints.Add(item.GetComponent<RectTransform>());
                            findBiggerValueofFillAmountSystem = item.GetComponent<DropZone>().healthBar.bar.fillAmount;
                        }
                    }
                }
            }
        }
        return allPoints.Last();
    }
}
