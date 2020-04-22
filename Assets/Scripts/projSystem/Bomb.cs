using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    public GameObject ExplosionAnim;
    public Transform ExplosionTransform;
    public bool trueRocket;
    int randomNumber;
    public static int movespeed = 400;
    public Vector3 userDirection = Vector3.right;
    [SerializeField]
    public float initializationTime;
    private GameObject goPar;
    private GameObject coppyPos;
    Transform TargetForRocket;
    Transform ChangedTargetForRocket;
    private Transform targetEmpty;
    private List<Transform> allPoints = new List<Transform>() { };
    [SerializeField]
    public float HealthRocket = 100;
    float angle;
    private bool blockTarget;
    public bool rocketONAttack;

    private float SystemBombDamageOnHPbar = 0.6f;
    public float ShipAndSystemBombDamage = 0.3f;

    void Start()
    {
        StartCoroutine(disableCollider());
        initializationTime = Time.time;
        goPar = gameObject.transform.parent.gameObject;
        coppyPos = new GameObject("MyGO", typeof(RectTransform));
        coppyPos.transform.SetParent(goPar.transform, true);
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
        StartCoroutine(DestroyBombbyTime(10f));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (TargetForRocket != null)
        {
            if (other.tag == "SlotGun" && other.name == TargetForRocket.name)
            {
                gameObject.SetActive(false);
                GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform.position, Quaternion.Euler(0f, 0f, 0f));
                expl.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 0f);
                expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
                Ship go = other.transform.parent.gameObject.GetComponent<Ship>();
                if (!rocketONAttack)
                {
                    go.ShipTakeDamage(ShipAndSystemBombDamage);
                }
            }
            if (other.tag == "SlotGunFull" && other.name == TargetForRocket.name)
            {
                gameObject.SetActive(false);
                GameObject expl = Instantiate(ExplosionAnim, ExplosionTransform.transform.position, Quaternion.Euler(0f, 0f, 0f));
                expl.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 0f);
                expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
                Ship go = other.transform.parent.gameObject.GetComponent<Ship>();
                if (!rocketONAttack)
                {
                    DropZone dz = other.GetComponent<DropZone>();
                    dz.healthBar.ReduceHPSystem(SystemBombDamageOnHPbar, gameObject);
                    GameObject icon = other.gameObject.GetComponentInChildren<DropZone>().iconSystem.gameObject;
                    if (go.shieldActive)
                    {
                        if (icon.GetComponent<Image>().sprite.name == "shield")
                        {
                            go.shieldImage.gameObject.GetComponent<Shield>().TakeNewColorA();
                            go.shieldImage.gameObject.GetComponent<Shield>().TakeDamage(SystemBombDamageOnHPbar);
                            go.ShipTakeDamage(ShipAndSystemBombDamage);
                        }
                    }
                    else
                    {
                        go.ShipTakeDamage(ShipAndSystemBombDamage);
                    }
                }
                HealthRocket = 0;
            }
        }
    }

    public void ReduceDamageRocket()
    {
        ShipAndSystemBombDamage *= 0.3f;
    }

    IEnumerator disableCollider()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    IEnumerator DestroyBombbyTime(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }

    public void DestroyAndAnimate(GameObject other)
    {
        //if (!targetISDONE)
        //{
            gameObject.SetActive(false);
            // for laser transorm, not same position like explosion transform.
            GameObject expl = Instantiate(ExplosionAnim, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f));
            expl.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 0f);
            expl.transform.SetParent(other.gameObject.transform.parent.gameObject.transform);
            Destroy(gameObject);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;
        gameObject.transform.Translate(userDirection * movespeed * Time.deltaTime);
        if (TargetForRocket != null)
        {
            if (ChangedTargetForRocket != null)
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
        if (Vector2.Distance(transform.position, coppyPos.transform.position) < 5f)
        {
            Destroy(gameObject);
        }
        if (HealthRocket <= 0)
        {
            Destroy(gameObject);
        }
    }

    public int TakeRandomNumberForSearch(int _randomNumber)
    {
        randomNumber = _randomNumber;
        return randomNumber;
    }

    Transform SearchTargetForRocket(GameObject goPar)
    {
        float findBiggerValueofFillAmountSystem = 0;
        if (goPar.name == "Ship-Player-1")
        {
            allPoints.Add(SearchTargetEmptySystem(goPar));

            GameObject ES = GameObject.Find("Ship-Player-2");
            IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().FindAllSlotGunsFull();
            foreach (RectTransform item in ESt)
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
                                1080f + item.transform.position.x,
                                goPar.transform.position.y - (ES.transform.position.y - item.transform.position.y),
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
            allPoints.Add(SearchTargetEmptySystem(goPar));

            GameObject PS = GameObject.Find("Ship-Player-1");
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
        if (allPoints.Count == 0)
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
                    randomNumber = ship.slotGunsEmpty.ToList().Count() - 1;
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
                    randomNumber = ship.slotGunsEmpty.ToList().Count() - 1;
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
                    randomNumber = ship.slotGunsEmpty.ToList().Count() - 1;
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
                    randomNumber = ship.slotGunsEmpty.ToList().Count() - 1;
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
