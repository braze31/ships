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
    GameObject coppyPos;
    Transform TargetForRocket;
    Transform ChangedTargetForRocket;

    public bool trueRocket;

    public bool rocketONAttack;

    public bool rightTrigg;
    public bool leftTrigg;

    public bool CheckForEvent = false;

    public bool triggerIsActived;

    private List<Transform> allPoints = new List<Transform>() { };
    [SerializeField]
    public int countRSystem = 0;
    private bool exploydEvent = false;

    float angle;

    private float SystemRocketDamageOnHPbar = 0.3f;
    private float ShipRocketDamage = 0.1f;
    public float ShipAndSystemRocketDamage = 0.05f;

    [SerializeField]
    public float initializationTime;
    private bool targetISDONE;
    [SerializeField]
    public float HealthRocket = 100;
    private bool blockTarget;
    private bool inSideRocket;
    private int randomNumber;
    private Transform targetEmpty;

    float posXShip;
    //public RectTransform ExplosionTransform;

    void Start()
    {
        StartCoroutine(disableCollider());
        initializationTime = Time.timeSinceLevelLoad;
        goPar = gameObject.transform.parent.gameObject;
        coppyPos = new GameObject("MyGO", typeof(RectTransform));
        coppyPos.transform.SetParent(goPar.transform,true);
        TargetForRocket = SearchTargetForRocket(goPar);
        if (!trueRocket && goPar.name == "Ship-Player-1")
        {
            GameObject go = GameObject.Find("Ship-Player-2");
            SearchTargetEmptySystem(goPar);
            gameObject.transform.position = new Vector3(
                -1080f + gameObject.transform.position.x,
                go.transform.position.y + (gameObject.transform.position.y - goPar.transform.position.y),
                0f
                );
        }
        if (!trueRocket && goPar.name == "Ship-Player-2")
        {
            GameObject go = GameObject.Find("Ship-Player-1");
            SearchTargetEmptySystem(goPar);
            gameObject.transform.position = new Vector3(
                1080f + gameObject.transform.position.x,
                go.transform.position.y + (gameObject.transform.position.y - goPar.transform.position.y),
                0f
                );
        }

        //if (TargetForRocket.tag == "SlotGunFull")
        //{
        //    gameObject.tag = "RocketS";
        //}
        posXShip = (int)coppyPos.GetComponent<RectTransform>().localPosition.x;
        Destroy(coppyPos, 1.5f);
        StartCoroutine(DestroyRocketbyTime(7f));
    }

    IEnumerator disableCollider()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    IEnumerator DestroyRocketbyTime(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }

    public void TakeRandomNumberForSearch(int numberForSearch)
    {
        randomNumber = numberForSearch;
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
        if (TargetForRocket != null)
        {
            if (other.tag == "SlotGun" && other.name == TargetForRocket.name)
            {
                gameObject.SetActive(false);
                GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform.position, Quaternion.Euler(0f, 0f, 0f));
                expl.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0f);
                expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
                Ship go = other.transform.parent.gameObject.GetComponent<Ship>();
                if (!rocketONAttack)
                {
                    go.ShipTakeDamage(ShipAndSystemRocketDamage);
                }
            }
            if (other.tag == "SlotGunFull" && other.name == TargetForRocket.name)
            {
                gameObject.SetActive(false);
                GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform.position, Quaternion.Euler(0f, 0f, 0f));
                expl.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0f);
                expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
                Ship go = other.transform.parent.gameObject.GetComponent<Ship>();
                if (!rocketONAttack)
                {
                    DropZone dz = other.GetComponent<DropZone>();
                    dz.healthBar.ReduceHPSystem(SystemRocketDamageOnHPbar, gameObject);
                    GameObject icon = other.gameObject.GetComponentInChildren<DropZone>().iconSystem.gameObject;
                    if (go.shieldActive)
                    {
                        if (icon.GetComponent<Image>().sprite.name == "shield")
                        {
                            go.shieldImage.gameObject.GetComponent<Shield>().TakeNewColorA();
                            go.shieldImage.gameObject.GetComponent<Shield>().TakeDamage(SystemRocketDamageOnHPbar);
                            go.ShipTakeDamage(ShipAndSystemRocketDamage);
                        }
                    }
                    else
                    {
                        go.ShipTakeDamage(ShipAndSystemRocketDamage);
                    }
                }
                HealthRocket = 0;
            }
        }
    }

    public void DestroyAndAnimate(GameObject other)
    {
        if (!targetISDONE)
        {
            gameObject.SetActive(false);
            // for laser transorm, not same position like explosion transform.
            GameObject expl = Instantiate(ExplosionAnim, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f));
            expl.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0f);
            expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
            Destroy(gameObject);
        }
    }

    public void ReduceDamageRocket()
    {
        ShipAndSystemRocketDamage *= 0.3f;
    }

    public void BackDamageRocket()
    {
        ShipAndSystemRocketDamage *= 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;
        gameObject.transform.Translate(userDirection * movespeed * Time.deltaTime);
        if (TargetForRocket!=null)
        {
            if (ChangedTargetForRocket!=null)
            {
                if (ChangedTargetForRocket != TargetForRocket)
                {
                    blockTarget = true;
                }
            }

            ChangedTargetForRocket = TargetForRocket;

            gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
            angle = Vector3.Angle(transform.right, TargetForRocket.transform.position - transform.position);
            if (TargetForRocket.transform.position.y > transform.position.y)
            {
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
            }
            else
            {
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -angle);
            }
        }
        if (gameObject.GetComponent<RectTransform>().transform.position.x >= 2160f 
            || gameObject.GetComponent<RectTransform>().transform.position.x <= -1080f)
        {
            Destroy(gameObject);
        }
        if (HealthRocket <= 0)
        {
            Destroy(gameObject);
        }
    }

    Transform SearchTargetForRocket(GameObject goPar)
    {
        float findBiggerValueofFillAmountSystem = 0;
        if (goPar.name == "Ship-Player-1")
        {
            if (!trueRocket)
            {
                allPoints.Add(SearchTargetEmptySystem(goPar));
            }
            if (trueRocket)
            {
                allPoints.Add(SearchTargetEmptySystem(goPar));
            }

            GameObject ES = GameObject.Find("Ship-Player-2");
            IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().rt;
            foreach (RectTransform item in ESt)
            {
                if (findBiggerValueofFillAmountSystem<=item.GetComponent<DropZone>().healthBar.bar.fillAmount)
                {
                    if (item.tag == "SlotGunFull")
                    {
                        if (!trueRocket)
                        {
                            allPoints.Add(item.GetComponent<RectTransform>());
                        }
                        else
                        {
                            coppyPos.transform.position = new Vector3(
                                1080f + item.transform.position.x,
                                goPar.transform.position.y - (ES.transform.position.y-item.transform.position.y),
                                0f
                            );
                        }
                        findBiggerValueofFillAmountSystem = item.GetComponent<DropZone>().healthBar.bar.fillAmount;
                    }
                }
            }
        }
        if (goPar.name == "Ship-Player-2")
        {
            if (!trueRocket)
            {
                //allPoints.Add(GameObject.Find("Ship-Player-1").transform);
                allPoints.Add(SearchTargetEmptySystem(goPar));
            }
            if (trueRocket)
            {
                //GameObject goMirrorShip = GameObject.Find("Ship-Enemy");
                //coppyPos.transform.position = new Vector3(
                //    -1080f + GameObject.Find("Ship-Player-1").transform.position.x,
                //    goPar.transform.position.y,
                //    0f
                //    );
                //allPoints.Add(coppyPos.transform);
                allPoints.Add(SearchTargetEmptySystem(goPar));
            }

            GameObject PS = GameObject.Find("Ship-Player-1");
            IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().rt;
            foreach (RectTransform item in PSt)
            {
                if (findBiggerValueofFillAmountSystem <= item.GetComponent<DropZone>().healthBar.bar.fillAmount)
                {
                    if (item.tag == "SlotGunFull")
                    {
                        if (!trueRocket)
                        {
                            allPoints.Add(item.GetComponent<RectTransform>());
                        }
                        else
                        {
                            coppyPos.transform.position = new Vector3(
                                -1080f + item.transform.position.x,
                                goPar.transform.position.y - (PS.transform.position.y - item.transform.position.y),
                                0f
                            );
                        }
                        findBiggerValueofFillAmountSystem = item.GetComponent<DropZone>().healthBar.bar.fillAmount;
                    }
                }
                
            }
        }
        if (allPoints.Count==0)
        {
            return null;
        }
        return allPoints.Last();
    }

    Transform SearchTargetEmptySystem(GameObject goPar)
    {
        if (goPar.name == "Ship-Player-1")
        {
            if (!trueRocket)
            {
                //allPoints.Add(GameObject.Find("Ship-Enemy").transform);
                GameObject go = GameObject.Find("Ship-Player-2");
                Ship ship = go.GetComponent<Ship>();
                ship.FindAllSlotGunsEmpty();
                targetEmpty = ship.slotGunsEmpty.ToList()[randomNumber].transform;
                targetEmpty.transform.position = ship.slotGunsEmpty.ToList()[randomNumber].transform.position;
            }
            else
            {
                GameObject go = GameObject.Find("Ship-Player-2");
                Ship ship = go.GetComponent<Ship>();
                ship.FindAllSlotGunsEmpty();
                targetEmpty = coppyPos.transform;
                targetEmpty.transform.position = new Vector3(
                    1080f + ship.slotGunsEmpty.ToList()[randomNumber].transform.position.x,
                    goPar.transform.position.y,
                    0f
                    );
            }
        }
        if (goPar.name == "Ship-Player-2")
        {
            if (!trueRocket)
            {
                //allPoints.Add(GameObject.Find("Ship-Enemy").transform);
                GameObject go = GameObject.Find("Ship-Player-1");
                Ship ship = go.GetComponent<Ship>();
                ship.FindAllSlotGunsEmpty();
                targetEmpty = ship.slotGunsEmpty.ToList()[randomNumber].transform;
                targetEmpty.transform.position = ship.slotGunsEmpty.ToList()[randomNumber].transform.position;
            }
            else
            {
                GameObject go = GameObject.Find("Ship-Player-1");
                Ship ship = go.GetComponent<Ship>();
                ship.FindAllSlotGunsEmpty();
                targetEmpty = coppyPos.transform;
                targetEmpty.transform.position = new Vector3(
                    -1080f + ship.slotGunsEmpty.ToList()[randomNumber].transform.position.x,
                    goPar.transform.position.y,
                    0f
                    );
            }
        }
        return targetEmpty;
    }
}
