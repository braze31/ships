using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Laser : MonoBehaviour
{
    GameObject ShipPlayer1;
    GameObject ShipPlayer2;
    GameObject goParant;
    GameObject RocketCheckTrig;

    Transform target;
    float valueHPSystemForLaser;
    public UILineRenderer LR;
    public float LineDrawSpeed;

    private bool offCameraLasser;
    private GameObject laserTargetGameObjectSearch;
    private bool SearchTargetInUpdate = false;
    private float dist;
    private float count;
    private Vector2 secondPoint;
    private float countBack;

    private bool RememberStartPoint;
    private bool RememberSecondPoint;
    Vector3 firstPosPoint;
    Vector3 SecondPosPoint;

    private bool disAppear;

    public bool STOPTHIS = false;
    public bool STARTTHIS = false;
    public bool SwapPoints = false;
    private bool oneTimeCor;
    public int randomNumber;
    private Transform targetEmpty;
    private GameObject targetBomb;
    private GameObject targetFlare;
    private bool findTargetFlare;
    private bool dontChangeFlare;

    void Awake()
    {
        ShipPlayer1 = GameObject.Find("Ship-Player-1");
        ShipPlayer2 = GameObject.Find("Ship-Player-2");
    }

    void Start()
    {
        goParant = gameObject.transform.parent.gameObject;
        valueHPSystemForLaser = 1f;
        laserTargetGameObjectSearch = new GameObject("MyLaser", typeof(RectTransform));
        targetEmpty = SearchTargetEmptySystem(goParant);
        if (offCameraLasser && goParant.name == "Ship-Player-1")
        {
            laserTargetGameObjectSearch.transform.position = targetEmpty.transform.position;
        }
        if (offCameraLasser && goParant.name == "Ship-Player-2")
        {
            laserTargetGameObjectSearch.transform.position = targetEmpty.transform.position;
        }
        if (!offCameraLasser && goParant.name == "Ship-Player-1")
        {
            laserTargetGameObjectSearch.transform.position = new Vector3(
                1080f + targetEmpty.transform.position.x,
                targetEmpty.transform.position.y
                );
        }
        if (!offCameraLasser && goParant.name == "Ship-Player-2")
        {
            laserTargetGameObjectSearch.transform.position = new Vector3(
                -1080f + targetEmpty.transform.position.x,
                targetEmpty.transform.position.y
                );
        }

        target = SearchTargetForLaser(goParant);
        if (target == null)
        {
            LR.Points[1] = targetEmpty.transform.position;
            SearchTargetInUpdate = false;
            dist = Vector3.Distance(LR.Points[0], LR.Points[1]);
        }
        else
        {
            SearchTargetInUpdate = true;
        }
    }

    public int TakeRandomNumberForSearch(int _randomNumber)
    {
        randomNumber = _randomNumber;
        return randomNumber;
    }

    public void TakeStartPosition(Vector3 pos)
    {
        if (!RememberStartPoint)
        {
            firstPosPoint = pos;
            RememberStartPoint = true;
        }
        // LR don't have change size. Take same pos for second point, 
        // because game have some not correct frames from second point in the PREFAB laser.
        LR.Points[0] = pos;
        LR.Points[1] = pos;
    }
    public void TakeStartPositionOFFLaser(Vector3 pos)
    {
        offCameraLasser = true;
        if (pos.y < 1100f)
        {
            LR.Points[0] = new Vector3(
                -1080 + pos.x,
                ShipPlayer2.transform.position.y + (pos.y - ShipPlayer1.transform.position.y),
                pos.z);
            LR.Points[1] = LR.Points[0];
        }
        if (pos.y > 1100f)
        {
            LR.Points[0] = new Vector3(
                1080 + pos.x,
                ShipPlayer1.transform.position.y + (pos.y - ShipPlayer2.transform.position.y),
                pos.z);
            LR.Points[1] = LR.Points[0];
        }
        if (!RememberStartPoint)
        {
            firstPosPoint = LR.Points[0];
            RememberStartPoint = true;
        }
    }

    public void TakeSecondPosition(Vector3 GameObjectPos)
    {
        if (!RememberSecondPoint)
        {
            SecondPosPoint = GameObjectPos;
            RememberSecondPoint = true;
        }
        secondPoint = GameObjectPos;
        LR.Points[1] = GameObjectPos;
        
    }

    void Update()
    {
        // no target for laser
        if (!SearchTargetInUpdate)
        {
            if (!STARTTHIS)
            {
                DrawLine();
            }
        }
        if (SearchTargetInUpdate)
        {
            if (targetFlare == null)
            {
                findTargetFlare = false;
            }
            if (findTargetFlare)
            {
                target = targetFlare.transform;
            }
            else
            {
                if (!dontChangeFlare)
                {
                    target = SearchTargetForLaser(goParant);
                }
            }

            if (target!=null)
            {
                LR.Points[1] = target.position;
                dist = Vector3.Distance(LR.Points[0], LR.Points[1]);
            }

            if (!STARTTHIS)
            {
                DrawLine();
            }
        }
        if (disAppear)
        {
            if (LR.LineThickness > 0)
            {
                LR.LineThickness -= Time.deltaTime * 777;
            }
            if (LR.LineThickness < 30)
            {
                StartCoroutine(destroyLaser());
            }
        }

        if (target!=null)
        {
            if (targetFlare != null)
            {
                if (targetFlare.tag == "Flare")
                {
                    Vector2 newVR = new Vector2(targetFlare.transform.position.x, targetFlare.transform.position.y);
                    if (LR.Points[1] == newVR)
                    {
                        dontChangeFlare = true;
                        targetFlare.GetComponent<Flare>().HealthRocket -= Time.deltaTime * 400;
                        targetFlare.GetComponent<Flare>().flareONAttack = true;
                        if (targetFlare.GetComponent<Flare>().HealthRocket <= 0)
                        {
                            SearchTargetInUpdate = false;
                            STARTTHIS = false;
                            disAppear = true;
                            LR.Points[1] = targetFlare.transform.position;
                            StartCoroutine(waitSearchAfterBOOM());
                        }
                    }
                }
            }

            if (targetBomb != null)
            {
                if (targetBomb.tag == "Bomb")
                {
                    Vector2 newVR = new Vector2(targetBomb.transform.position.x, targetBomb.transform.position.y);
                    if (LR.Points[1] == newVR)
                    {
                        targetBomb.GetComponent<Bomb>().HealthRocket -= Time.deltaTime * 400;
                        targetBomb.GetComponent<Bomb>().rocketONAttack = true;
                        if (targetBomb.GetComponent<Bomb>().HealthRocket <= 0)
                        {
                            SearchTargetInUpdate = false;
                            STARTTHIS = false;
                            disAppear = true;
                            LR.Points[1] = targetBomb.transform.position;
                            targetBomb.GetComponent<Bomb>().DestroyAndAnimate(gameObject);
                            StartCoroutine(waitSearchAfterBOOM());
                        }
                    }
                    else
                    {
                        targetBomb.GetComponent<Bomb>().rocketONAttack = false;
                    }
                }
            }

            if (RocketCheckTrig != null)
            {
                Vector2 newVR = new Vector2(RocketCheckTrig.transform.position.x, RocketCheckTrig.transform.position.y);
                if (LR.Points[1] == newVR)
                {
                    RocketCheckTrig.GetComponent<Rocket>().HealthRocket -= Time.deltaTime * 400;
                    RocketCheckTrig.GetComponent<Rocket>().rocketONAttack = true;
                    if (RocketCheckTrig.GetComponent<Rocket>().HealthRocket <= 0)
                    {
                        SearchTargetInUpdate = false;
                        STARTTHIS = false;
                        disAppear = true;
                        LR.Points[1] = RocketCheckTrig.transform.position;
                        RocketCheckTrig.GetComponent<Rocket>().DestroyAndAnimate(gameObject);
                        StartCoroutine(waitSearchAfterBOOM());
                    }
                }
                else
                {
                    RocketCheckTrig.GetComponent<Rocket>().rocketONAttack = false;
                }
            }
        }

        if (valueHPSystemForLaser <= 0)
        {
            disAppear = true;
            STARTTHIS = true;
            StartCoroutine(destroyLaser());
        }

        LR.SetAllDirty();
    }

    void DrawLine()
    {
        if (count < dist)
        {
            if (LR.LineThickness < 110)
            {
                LR.LineThickness += Time.deltaTime * 555;
            }
            count += LineDrawSpeed * Time.deltaTime;

            float x = Mathf.Lerp(0, dist, count);

            Vector3 pointA = LR.Points[0];
            Vector3 pointB = LR.Points[1];

            Vector3 pointALongTime = x * Vector3.Normalize(pointB - pointA) + pointA;

            LR.Points[1] = pointALongTime;

            if (!oneTimeCor && LR.LineThickness > 110)
            {
                StartCoroutine(WaitStartDrawBack());
                oneTimeCor = true;
            }
        }
    }

    IEnumerator WaitStartDrawBack()
    {
        // 1 ses beetween laser prefab in PlayerControls.cs script
        // time to stop DRAWLINE() function
        yield return new WaitForSeconds(0.2f);
        STARTTHIS = true;
        // time to dispappear laser on scene
        yield return new WaitForSeconds(0.2f);
        disAppear = true;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(destroyLaser());
    }

    void AttackShip(GameObject ShipAttack)
    {
        Ship go = ShipAttack.gameObject.GetComponent<Ship>();
        if (go != null)
        {
            if (RocketCheckTrig == null || targetBomb == null)
            {
                if (go.shieldActive)
                {
                    go.ShipTakeDamage(0.05f * 0.3f);
                    //go.shieldImage.gameObject.GetComponent<Shield>().TakeNewColorA();
                }
                else
                {
                    go.ShipTakeDamage(0.05f);
                }
            }
        }
    }

    public void TakeStartHPBARLaser(Image item)
    {
        valueHPSystemForLaser = item.transform.parent.GetComponent<DropZone>().healthBar.bar.fillAmount;
    }

    IEnumerator destroyLaser()
    {
        Destroy(laserTargetGameObjectSearch);
        yield return new WaitForSeconds(0.0001f);
        Destroy(gameObject);
    }

    IEnumerator waitSearchAfterBOOM()
    {
        STARTTHIS = true;
        Destroy(laserTargetGameObjectSearch);
        yield return new WaitForSeconds(0.1f);
        disAppear = true;
    }

    Transform SearchTargetForLaser(GameObject goParant)
    {
        if (goParant.name == "Ship-Player-1")
        {
            GameObject ES = GameObject.Find("Ship-Player-2");

            IEnumerable<RectTransform> ESf = ES.GetComponent<Ship>().FindAllFlareFromShip();
            if (ESf != null && ESf.Count() > 0 && !findTargetFlare)
            {
                if (!offCameraLasser)
                {
                    IEnumerable<RectTransform> ESfdown = ESf.Where(a => a.transform.position.y < 1100f);
                    if (ESfdown.Count() > 0)
                    {
                        targetFlare = ESfdown.First().gameObject;
                        targetFlare.GetComponent<Flare>().flareONAttack = true;
                        findTargetFlare = true;
                        return targetFlare.transform;
                    }
                }
                if (offCameraLasser)
                {
                    IEnumerable<RectTransform> ESfdown = ESf.Where(a => a.transform.position.y > 1100f);
                    if (ESfdown.Count() > 0)
                    {
                        targetFlare = ESfdown.First().gameObject;
                        targetFlare.GetComponent<Flare>().flareONAttack = true;
                        findTargetFlare = true;
                        return targetFlare.transform;
                    }
                }
            }

            IEnumerable<RectTransform> ESb = ES.GetComponent<Ship>().FindAllBombFromShip();
            if (ESb != null && ESb.Count()>0)
            {
                float maxB = 0;

                foreach (RectTransform item in ESb)
                {
                    if (item != null && !item.Equals(null) && item.tag == "Bomb")
                    {
                        float valueTime = item.GetComponent<Bomb>().initializationTime;
                        if (!offCameraLasser)
                        {
                            if (item.transform.position.y < 1100f)
                            {
                                if (maxB < valueTime)
                                {
                                    maxB = valueTime;
                                    targetBomb = item.gameObject;
                                    //findAimnOneMoreTime = false;
                                }
                            }
                        }
                        if (offCameraLasser)
                        {
                            if (item.transform.position.y > 1100f)
                            {
                                if (maxB < valueTime)
                                {
                                    maxB = valueTime;
                                    targetBomb = item.gameObject;
                                    //findAimnOneMoreTime = false;
                                }
                            }
                        }
                    }
                }
                return targetBomb.transform;
            }


            IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().FindAllRocketromShip();

            float maxV = 0;

            foreach (RectTransform item in ESt)
            {
                if (item != null && !item.Equals(null))
                {
                    float valueTime = item.GetComponent<Rocket>().initializationTime;
                    if (!offCameraLasser)
                    {
                        if (item.transform.position.y < 1100f)
                        {
                            if (maxV < valueTime)
                            {
                                maxV = valueTime;
                                RocketCheckTrig = item.gameObject;
                                //findAimnOneMoreTime = false;
                            }
                        }
                    }
                    if (offCameraLasser)
                    {
                        if (item.transform.position.y > 1100f)
                        {
                            if (maxV < valueTime)
                            {
                                maxV = valueTime;
                                RocketCheckTrig = item.gameObject;
                                //findAimnOneMoreTime = false;
                            }
                        }
                    }
                }
            }
        }
        if (goParant.name == "Ship-Player-2")
        {
            GameObject PS = GameObject.Find("Ship-Player-1");

            IEnumerable<RectTransform> ESf = PS.GetComponent<Ship>().FindAllFlareFromShip();
            if (ESf != null && ESf.Count() > 0 && !findTargetFlare)
            {
                if (!offCameraLasser)
                {
                    IEnumerable<RectTransform> ESfdown = ESf.Where(a => a.transform.position.y > 1100f);
                    if (ESfdown.Count() > 0)
                    {
                        targetFlare = ESfdown.First().gameObject;
                        targetFlare.GetComponent<Flare>().flareONAttack = true;
                        findTargetFlare = true;
                        return targetFlare.transform;
                    }
                }
                if (offCameraLasser)
                {
                    IEnumerable<RectTransform> ESfdown = ESf.Where(a => a.transform.position.y < 1100f);
                    if (ESfdown.Count() > 0)
                    {
                        targetFlare = ESfdown.First().gameObject;
                        targetFlare.GetComponent<Flare>().flareONAttack = true;
                        findTargetFlare = true;
                        return targetFlare.transform;
                    }
                }
            }

            IEnumerable<RectTransform> ESb = PS.GetComponent<Ship>().FindAllBombFromShip();
            if (ESb != null && ESb.Count() > 0)
            {
                float maxB = 0;

                foreach (RectTransform item in ESb)
                {
                    if (item != null && !item.Equals(null) && item.tag == "Bomb")
                    {
                        float valueTime = item.GetComponent<Bomb>().initializationTime;
                        if (!offCameraLasser)
                        {
                            if (item.transform.position.y > 1100f)
                            {
                                if (maxB < valueTime)
                                {
                                    maxB = valueTime;
                                    targetBomb = item.gameObject;
                                    //findAimnOneMoreTime = false;
                                }
                            }
                        }
                        if (offCameraLasser)
                        {
                            if (item.transform.position.y < 1100f)
                            {
                                if (maxB < valueTime)
                                {
                                    maxB = valueTime;
                                    targetBomb = item.gameObject;
                                    //findAimnOneMoreTime = false;
                                }
                            }
                        }
                    }
                }
                return targetBomb.transform;
            }

            IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().FindAllRocketromShip();
            float maxV = 0;
            foreach (RectTransform item in PSt)
            {
                if (item != null && !item.Equals(null))
                {
                    float valueTime = item.GetComponent<Rocket>().initializationTime;
                    if (!offCameraLasser)
                    {
                        if (item.transform.position.y > 1100f)
                        {
                            if (maxV < valueTime)
                            {
                                maxV = valueTime;
                                RocketCheckTrig = item.gameObject;
                                //findAimnOneMoreTime = false;
                            }
                        }
                    }
                    if (offCameraLasser)
                    {
                        if (item.transform.position.y < 1100f)
                        {
                            if (maxV < valueTime)
                            {
                                maxV = valueTime;
                                RocketCheckTrig = item.gameObject;
                                //findAimnOneMoreTime = false;
                            }
                        }
                    }
                }
            }
        }
        if (RocketCheckTrig==null)
        {
            return null;
        }
        return RocketCheckTrig.transform;
    }

    Transform SearchTargetEmptySystem(GameObject goPar)
    {
        if (goPar.name == "Ship-Player-1")
        {
            GameObject ES = GameObject.Find("Ship-Player-2");
            IEnumerable<RectTransform> ESO = ES.GetComponent<Ship>().FindAllObjectsFromShip();
            if (ESO.Count()==0)
            {
                AttackShip(ShipPlayer2);
            }

            if (offCameraLasser)
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
                    randomNumber = ship.slotGunsEmpty.ToList().Count() - 1;
                }
                try
                {
                    targetEmpty = laserTargetGameObjectSearch.transform;
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
            GameObject PS = GameObject.Find("Ship-Player-1");
            IEnumerable<RectTransform> ESO = PS.GetComponent<Ship>().FindAllObjectsFromShip();
            if (ESO.Count() == 0)
            {
                AttackShip(ShipPlayer1);
            }

            if (offCameraLasser)
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
                    targetEmpty = laserTargetGameObjectSearch.transform;
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
