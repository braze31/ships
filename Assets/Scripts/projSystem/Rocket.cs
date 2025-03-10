﻿using ExitGames.Client.Photon;
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
    GameObject targetFlare;

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
    public bool flareInTarget = false;
    public bool changeTargetAgain = false;

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
        //StartCoroutine(disableCollider());
        initializationTime = Time.time;
        goPar = gameObject.transform.parent.gameObject;
        coppyPos = new GameObject("MyGO", typeof(RectTransform));
        coppyPos.transform.SetParent(goPar.transform,true);
        coppyPos.AddComponent<BoxCollider2D>().isTrigger = true;
        coppyPos.GetComponent<BoxCollider2D>().size = new Vector2(60f, 60f);
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
        Destroy(coppyPos, 2.5f);
        StartCoroutine(DestroyRocketbyTime(5f));
    }

    //IEnumerator disableCollider()
    //{
    //    //gameObject.GetComponent<Collider2D>().enabled = false;
    //    //yield return new WaitForSeconds(0.1f);
    //    //gameObject.GetComponent<Collider2D>().enabled = true;
    //}

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
        if (other.name == "MyGO" || other.tag == "Destroyer")
        {
            Destroy(gameObject);
        }
        if (targetFlare!=null)
        {
            if (other.tag == "Flare")
            {
                gameObject.SetActive(false);
                GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform.position, Quaternion.Euler(0f, 0f, 0f));
                expl.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0f);
                expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
                Destroy(other.gameObject);
                HealthRocket = 0;
            }
        }

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
            if (other.tag == "SlotGunFull" && other.name == TargetForRocket.name ||
                gameObject.tag == "RocketS" && (other.tag == "SlotGun" || other.tag == "SlotGunFull")
                )
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
            if (TargetForRocket.gameObject.tag == "SlotGun")
            {
                blockTarget = true;
            }
            if (TargetForRocket.gameObject.tag != "Flare")
            {
                flareInTarget = false;
            }
            if (!flareInTarget)
            {
                TargetForRocket = SearchTargetForRocket(goPar);
            }
            if (TargetForRocket.gameObject.tag == "Flare")
            {
                flareInTarget = true;
                TargetForRocket.gameObject.GetComponent<Flare>().GetAimedRocket(gameObject);
            }

            if (changeTargetAgain)
            {
                TargetForRocket = SearchTargetEmptySystem(goPar);
                gameObject.tag = "RocketS";
            }

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
            EXPLOYD();
        }
    }

    Transform SearchTargetForRocket(GameObject goPar)
    {
        float findBiggerValueofFillAmountSystem = 0;
        if (goPar.name == "Ship-Player-1")
        {
            allPoints.Add(SearchTargetEmptySystem(goPar));

            GameObject ES = GameObject.Find("Ship-Player-2");
            if (!blockTarget)
            {
                IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().FindAllSlotGunsFull();
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
            if (!flareInTarget)
            {
                IEnumerable<RectTransform> ESf = ES.GetComponent<Ship>().FindAllFlareFromShip();
                if (ESf.Count() > 0)
                {
                    if (trueRocket)
                    {
                        IEnumerable<RectTransform> ESfdown = ESf.Where(a => a.transform.position.y < 1100f);
                        if (ESfdown.Count() > 0)
                        {
                            targetFlare = ESfdown.First().gameObject;
                            targetFlare.GetComponent<Flare>().flareONAttack = true;
                            allPoints.Add(targetFlare.transform);
                        }
                    }
                    if (!trueRocket)
                    {
                        IEnumerable<RectTransform> ESfdown = ESf.Where(a => a.transform.position.y > 1100f);
                        if (ESfdown.Count() > 0)
                        {
                            targetFlare = ESfdown.First().gameObject;
                            targetFlare.GetComponent<Flare>().flareONAttack = true;
                            allPoints.Add(targetFlare.transform);
                        }
                    }
                }
                flareInTarget = true;
            }
        }
        if (goPar.name == "Ship-Player-2")
        {
            allPoints.Add(SearchTargetEmptySystem(goPar));

            GameObject PS = GameObject.Find("Ship-Player-1");
            if (!blockTarget)
            {
                IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().FindAllSlotGunsFull();
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
            if (!flareInTarget)
            {
                IEnumerable<RectTransform> ESf = PS.GetComponent<Ship>().FindAllFlareFromShip();
                if (ESf.Count() > 0)
                {
                    if (!trueRocket)
                    {
                        IEnumerable<RectTransform> ESfdown = ESf.Where(a => a.transform.position.y < 1100f);
                        if (ESfdown.Count() > 0)
                        {
                            targetFlare = ESfdown.First().gameObject;
                            targetFlare.GetComponent<Flare>().flareONAttack = true;
                            allPoints.Add(targetFlare.transform);
                        }
                    }
                    if (trueRocket)
                    {
                        IEnumerable<RectTransform> ESfdown = ESf.Where(a => a.transform.position.y > 1100f);
                        if (ESfdown.Count() > 0)
                        {
                            targetFlare = ESfdown.First().gameObject;
                            targetFlare.GetComponent<Flare>().flareONAttack = true;
                            allPoints.Add(targetFlare.transform);
                        }
                    }
                }
                flareInTarget = true;
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
                if (ship.slotGunsEmpty.ToList().Count() <= randomNumber)
                {
                    randomNumber = ship.slotGunsEmpty.ToList().Count()-1;
                }
                try
                {
                    targetEmpty = ship.slotGunsEmpty.ToList()[randomNumber].transform;
                    targetEmpty.transform.position = ship.slotGunsEmpty.ToList()[randomNumber].transform.position;
                }
                catch (System.ArgumentOutOfRangeException e)  // CS0168
                {
                    Debug.Log(randomNumber + " - " + ship.slotGunsEmpty.ToList().Count());
                    // Set IndexOutOfRangeException to the new exception's InnerException.
                    throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                }

            }
            else
            {
                GameObject go = GameObject.Find("Ship-Player-2");
                Ship ship = go.GetComponent<Ship>();
                ship.FindAllSlotGunsEmpty();
                if (ship.slotGunsEmpty.ToList().Count() <= randomNumber)
                {
                    randomNumber = ship.slotGunsEmpty.ToList().Count()-1;
                }
                try
                {
                    targetEmpty = coppyPos.transform;
                    targetEmpty.transform.position = new Vector3(
                        1080f + ship.slotGunsEmpty.ToList()[randomNumber].transform.position.x,
                        goPar.transform.position.y,
                        0f
                        );
                }
                catch (System.ArgumentOutOfRangeException e)  // CS0168
                {
                    Debug.Log(randomNumber + " - " + ship.slotGunsEmpty.ToList().Count());
                    // Set IndexOutOfRangeException to the new exception's InnerException.
                    throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                }
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
                if (ship.slotGunsEmpty.ToList().Count() <= randomNumber)
                {
                    randomNumber = ship.slotGunsEmpty.ToList().Count()-1;
                }
                try
                {
                    targetEmpty = ship.slotGunsEmpty.ToList()[randomNumber].transform;
                    targetEmpty.transform.position = ship.slotGunsEmpty.ToList()[randomNumber].transform.position;
                }
                catch (System.ArgumentOutOfRangeException e)  // CS0168
                {
                    Debug.Log(randomNumber + " - " + ship.slotGunsEmpty.ToList().Count());
                    // Set IndexOutOfRangeException to the new exception's InnerException.
                    throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                }
            }
            else
            {
                GameObject go = GameObject.Find("Ship-Player-1");
                Ship ship = go.GetComponent<Ship>();
                ship.FindAllSlotGunsEmpty();
                if (ship.slotGunsEmpty.ToList().Count() <= randomNumber)
                {
                    randomNumber = ship.slotGunsEmpty.ToList().Count()-1;
                }
                try
                {
                    targetEmpty = coppyPos.transform;
                    targetEmpty.transform.position = new Vector3(
                        -1080f + ship.slotGunsEmpty.ToList()[randomNumber].transform.position.x,
                        goPar.transform.position.y,
                        0f
                        );
                }
                catch (System.ArgumentOutOfRangeException e)  // CS0168
                {
                    Debug.Log(randomNumber + " - " + ship.slotGunsEmpty.ToList().Count());
                    // Set IndexOutOfRangeException to the new exception's InnerException.
                    throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                }
            }
        }
        return targetEmpty;
    }
}
