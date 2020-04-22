using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefShip : MonoBehaviour
{
    private float speed = 100f;
    private float startWaiteTime = 0.1f;
    private float startShipFlyTimeFromMain = 1.4f;
    private bool startFlyFromMainShip = true;
    private bool startPatrol = false;

    [SerializeField]
    public RectTransform[] moveSpots;
    public GameObject projectile;
    private RectTransform[] moveSpotsCoppy;
    private GameObject goPar;
    private RectTransform target;
    private bool stopSearch;
    private int randomSpot;
    private bool changePOSAngle;
    public bool trueDefship;
    Rigidbody2D rb;
    private Vector2 dir;

    private float timeBtwShots = 0.2f;
    private float startTimeBtwShots = 0.1f;

    void Start()
    {
        randomSpot = 0;
        GameObject ship = gameObject.transform.parent.gameObject;
        goPar = gameObject.transform.parent.gameObject;
        rb = GetComponent<Rigidbody2D>();

        if (goPar.name == "Ship-Player-1")
        {
            moveSpots = ship.GetComponentsInChildren<RectTransform>().Where(a => a.tag == "MoveSpot" 
            && a.name != "TransformPosCoppy"
            && a.transform.position.y<1100f
            ).ToArray();
        }
        if (goPar.name == "Ship-Player-2")
        {
            moveSpots = ship.GetComponentsInChildren<RectTransform>().Where(a => a.tag == "MoveSpot" 
            && a.name != "TransformPosCoppy"
            && a.transform.position.y>1100f
            ).ToArray();
        }

        if (!trueDefship && goPar.name == "Ship-Player-1")
        {
            GameObject go = GameObject.Find("Ship-Player-2");
            gameObject.transform.position = new Vector3(
                -1080f + gameObject.transform.position.x,
                go.transform.position.y + (gameObject.transform.position.y - goPar.transform.position.y),
                0f
                );
            moveSpotsCoppy = ship.GetComponentsInChildren<RectTransform>().Where(a => a.tag == "MoveSpot"
                && a.transform.position.y > 1100f
                && a.transform.position.x < 0f
                ).ToArray();
            if (moveSpotsCoppy.Length==0)
            {
                foreach (var item in moveSpots)
                {
                    GameObject coppyPos = new GameObject("TransformPosCoppy", typeof(RectTransform));
                    coppyPos.transform.SetParent(goPar.transform, false);
                    coppyPos.transform.position = new Vector3(
                        -1080f + item.transform.position.x,
                        go.transform.position.y + (item.transform.position.y - goPar.transform.position.y),
                        0f
                        );
                    coppyPos.tag = "MoveSpot";
                }
            }
            moveSpots = ship.GetComponentsInChildren<RectTransform>().Where(a => a.tag == "MoveSpot" && a.transform.position.x <= 0f).ToArray();
        }
        if (!trueDefship && goPar.name == "Ship-Player-2")
        {
            GameObject go = GameObject.Find("Ship-Player-1");
            gameObject.transform.position = new Vector3(
                1080f + gameObject.transform.position.x,
                go.transform.position.y + (gameObject.transform.position.y - goPar.transform.position.y),
                0f
                );
            moveSpotsCoppy = ship.GetComponentsInChildren<RectTransform>().Where(a => a.tag == "MoveSpot"
                && a.transform.position.y < 1100f
                && a.transform.position.x > 1080f
                ).ToArray();
            if (moveSpotsCoppy.Length == 0)
            {
                foreach (var item in moveSpots)
                {
                    GameObject coppyPos = new GameObject("TransformPosCoppy", typeof(RectTransform));
                    coppyPos.transform.SetParent(goPar.transform, false);
                    coppyPos.transform.position = new Vector3(
                        1080f + item.transform.position.x,
                        go.transform.position.y + (item.transform.position.y - goPar.transform.position.y),
                        0f
                        );
                    coppyPos.tag = "MoveSpot";
                }
            }
            moveSpots = ship.GetComponentsInChildren<RectTransform>().Where(a => a.tag == "MoveSpot" && a.transform.position.x >= 1080f).ToArray();
        }

    }

    void FixedUpdate()
    {
        if (startFlyFromMainShip)
        {
            if (goPar.name == "Ship-Player-1")
            {
                gameObject.transform.Translate(Vector2.up * speed * Time.deltaTime);
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else
            {
                gameObject.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, -90f);
                gameObject.transform.Translate(Vector2.up * speed * Time.deltaTime);
            }

            startShipFlyTimeFromMain -= Time.deltaTime;
            if (startShipFlyTimeFromMain <= 0)
            {
                startFlyFromMainShip = false;
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
                startPatrol = true;
                speed = speed * 7;
            }

        }
        if (startPatrol)
        {
            if (Vector2.Distance(transform.position, moveSpots[randomSpot].position) < 50f)
            {
                randomSpot += 1;
                if (randomSpot == moveSpots.Length)
                {
                    randomSpot = 0;
                }
            }

            if (!changePOSAngle)
            {
                dir = (moveSpots[randomSpot].position - rb.transform.position).normalized;
                float rotateAmount = Vector3.Cross(dir, transform.up).z;
                rb.angularVelocity = -rotateAmount * 300f;
                rb.velocity = transform.up * 400f;

                // another way rotate ship on target

                //float targetAngle = Mathf.Atan2(
                //    moveSpots[randomSpot].position.y - transform.position.y,
                //    moveSpots[randomSpot].position.x - transform.position.x) * Mathf.Rad2Deg;

                ////get the current angle of the ship
                //float sourceAngle = Mathf.Atan2(
                //    this.transform.up.y,
                //    this.transform.up.x) * Mathf.Rad2Deg;
                //float diffAngle = targetAngle - sourceAngle;

                ////use the smaller of the two angles to ensure we always turn the correct way
                //if (Mathf.Abs(diffAngle) > 180f)
                //{
                //    diffAngle = sourceAngle - targetAngle;
                //}

            }

        }
        if (!startFlyFromMainShip && !stopSearch)
        {
            target = SearchTargetForShip(goPar);
            if (target==null)
            {
                startPatrol = true;
                changePOSAngle = false;
            }
        }
        if (target!=null)
        {
            changePOSAngle = true;
            if (Vector2.Distance(transform.position, target.position) > 170f)
            {
                dir = (target.position - rb.transform.position).normalized;
                float rotateAmount = Vector3.Cross(dir, transform.up).z;
                rb.angularVelocity = -rotateAmount * 800f;
                rb.velocity = transform.up * 750;
            }

            if (Vector2.Distance(transform.position, target.position) <= 450f)
            {
                if (timeBtwShots <= 0)
                {
                    GameObject myLaser = Instantiate(projectile, transform.position, Quaternion.identity);
                    myLaser.transform.SetParent(goPar.transform);
                    myLaser.GetComponent<LaserDefShip>().TakeTarget(target.gameObject);
                    timeBtwShots = startTimeBtwShots;
                }
                else
                {
                    timeBtwShots -= Time.deltaTime;
                }
            }
        }
    }

    public RectTransform SearchTargetForShip(GameObject goPar)
    {

        if (goPar.name == "Ship-Player-1")
        {
            GameObject go = GameObject.Find("Ship-Player-2");
            if (trueDefship)
            {
                IEnumerable<RectTransform> systems = go.GetComponent<Ship>().FindAllRocketromShip().Where(a => a.transform.position.y < 1100f);
                if (systems.Count() > 0)
                {
                    target = systems.First();
                    startPatrol = false;
                    return target;
                }
            }
            else
            {
                IEnumerable<RectTransform> systems = go.GetComponent<Ship>().FindAllRocketromShip().Where(a => a.transform.position.y > 1100f);
                if (systems.Count() > 0)
                {
                    target = systems.First();
                    startPatrol = false;
                    return target;
                }
            }
        }
        if (goPar.name == "Ship-Player-2")
        {
            GameObject go = GameObject.Find("Ship-Player-1");
            if (trueDefship)
            {
                IEnumerable<RectTransform> systems = go.GetComponent<Ship>().FindAllRocketromShip().Where(a => a.transform.position.y > 1100f);
                if (systems.Count() > 0)
                {
                    target = systems.First();
                    startPatrol = false;
                    return target;
                }
            }
            else
            {
                IEnumerable<RectTransform> systems = go.GetComponent<Ship>().FindAllRocketromShip().Where(a => a.transform.position.y < 1100f);
                if (systems.Count() > 0)
                {
                    target = systems.First();
                    startPatrol = false;
                    return target;
                }
            }
        }

        return null;
    }
}
