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
            AttackShip(ShipPlayer2);
        }
        if (offCameraLasser && goParant.name == "Ship-Player-2")
        {
            laserTargetGameObjectSearch.transform.position = targetEmpty.transform.position;
            AttackShip(ShipPlayer1);
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
            target = SearchTargetForLaser(goParant);
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
            if (go.shieldActive)
            {
                go.ShipTakeDamage(0.05f*0.3f);
                //go.shieldImage.gameObject.GetComponent<Shield>().TakeNewColorA();
            }
            else
            {
                go.ShipTakeDamage(0.05f);
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
            IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().AllObjectsFromShip;

            float maxV = 0;

            foreach (RectTransform item in ESt)
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
        if (goParant.name == "Ship-Player-2")
        {
            GameObject PS = GameObject.Find("Ship-Player-1");
            IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().AllObjectsFromShip;

            float maxV = 0;

            foreach (RectTransform item in PSt)
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
            if (offCameraLasser)
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
                targetEmpty = laserTargetGameObjectSearch.transform;
                targetEmpty.transform.position = new Vector3(
                    1080f + ship.slotGunsEmpty.ToList()[randomNumber].transform.position.x,
                    goPar.transform.position.y,
                    0f
                    );
            }
        }
        if (goPar.name == "Ship-Player-2")
        {
            if (offCameraLasser)
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
                targetEmpty = laserTargetGameObjectSearch.transform;
                targetEmpty.transform.position = new Vector3(
                    -1080f + ship.slotGunsEmpty.ToList()[randomNumber].transform.position.x,
                    goPar.transform.position.y,
                    0f
                    );
            }
        }
        return targetEmpty;
    }

    //void Start()
    //{
    //    StartCoroutine(DestroyLaserByTime());
    //    valueHPSystemForLaser = 1f;
    //    goParant = gameObject.transform.parent.gameObject;
    //    laserTargetGameObjectSearch = new GameObject("MyLaser", typeof(RectTransform));
    //    if (offCameraLasser && goParant.name == "Ship-Player-1")
    //    {
    //        laserTargetGameObjectSearch.transform.position = ShipPlayer2.transform.position;
    //    }
    //    if (offCameraLasser && goParant.name == "Ship-Player-2")
    //    {
    //        laserTargetGameObjectSearch.transform.position = ShipPlayer1.transform.position;
    //    }
    //    if (!offCameraLasser && goParant.name == "Ship-Player-1")
    //    {
    //        laserTargetGameObjectSearch.transform.position = new Vector3(
    //            1080f + ShipPlayer2.transform.position.x,
    //            ShipPlayer1.transform.position.y
    //            );
    //    }
    //    if (!offCameraLasser && goParant.name == "Ship-Player-2")
    //    {
    //        laserTargetGameObjectSearch.transform.position = new Vector3(
    //            -1080f + ShipPlayer1.transform.position.x,
    //            ShipPlayer2.transform.position.y
    //            );
    //    }

    //    if (offCameraLasser)
    //    {
    //        target = SearchTargetForOffLaser(goParant);
    //    }
    //    else
    //    {
    //        target = SearchTargetForLaser(goParant);
    //    }


    //}

    //IEnumerator DestroyLaserByTime()
    //{
    //    yield return new WaitForSeconds(4.2f);
    //    Destroy(gameObject);
    //    Destroy(laserTargetGameObjectSearch);
    //}

    //public void TakeStartHPBARLaser(Image item)
    //{
    //    valueHPSystemForLaser = item.GetComponent<DropZone>().healthBar.bar.fillAmount;
    //}

    //public void TakeStartPosition(Vector3 pos)
    //{
    //    if (!RememberStartPoint)
    //    {
    //        firstPosPoint = pos;
    //        RememberStartPoint = true;
    //    }
    //    // LR don't have change size. Take same pos for second point, 
    //    // because game have some not correct frames from second point in the PREFAB laser.
    //    LR.Points[0] = pos;
    //    LR.Points[1] = pos;
    //}
    //public void TakeStartPositionOFFLaser(Vector3 pos)
    //{
    //    offCameraLasser = true;
    //    if (pos.y < 1100f)
    //    {
    //        LR.Points[0] = new Vector3(
    //            -1080 + pos.x, 
    //            ShipPlayer2.transform.position.y + (pos.y - ShipPlayer1.transform.position.y),
    //            pos.z);
    //        LR.Points[1] = LR.Points[0];
    //    }
    //    if (pos.y > 1100f)
    //    {
    //        LR.Points[0] = new Vector3(
    //            1080 + pos.x, 
    //            ShipPlayer1.transform.position.y + (pos.y - ShipPlayer2.transform.position.y),
    //            pos.z);
    //        LR.Points[1] = LR.Points[0];
    //    }
    //    if (!RememberStartPoint)
    //    {
    //        firstPosPoint = LR.Points[0];
    //        RememberStartPoint = true;
    //    }
    //}

    //public void TakeSecondPosition(Vector3 GameObjectPos)
    //{
    //    if (!RememberSecondPoint)
    //    {
    //        SecondPosPoint = GameObjectPos;
    //        RememberSecondPoint = true;
    //    }
    //    secondPoint = GameObjectPos;
    //    LR.Points[1] = GameObjectPos;
    //    dist = Vector3.Distance(LR.Points[0], LR.Points[1]);
    //}

    //IEnumerator WaitStartDrawBack()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    STARTTHIS = true;
    //    yield return new WaitForSeconds(0.1f);
    //    disAppear = true;
    //}

    ////IEnumerator WaitCycle()
    ////{
    ////    yield return new WaitForSeconds(0.2f);
    ////    SearchTargetInUpdate = true;
    ////    yield return new WaitForSeconds(0.5f);
    ////    findShipSystem = true;
    ////    findAimnOneMoreTime = true;
    ////    disAppear = false;
    ////    count = 0;
    ////    STARTTHIS = false;
    ////    SearchTargetInUpdate = false;
    ////}

    //void DrawLine()
    //{
    //    if (count < dist)
    //    {
    //        if (LR.LineThickness < 150)
    //        {
    //            LR.LineThickness += Time.deltaTime * 300;
    //        }
    //        count += LineDrawSpeed * Time.deltaTime;

    //        float x = Mathf.Lerp(0, dist, count);

    //        Vector3 pointA = LR.Points[0];
    //        Vector3 pointB = LR.Points[1];

    //        Vector3 pointALongTime = x * Vector3.Normalize(pointB - pointA) + pointA;

    //        LR.Points[1] = pointALongTime;

    //        if (!oneTimeCor && LR.LineThickness > 110)
    //        {
    //            StartCoroutine(WaitStartDrawBack());
    //            oneTimeCor = true;
    //        }
    //    }
    //}

    //IEnumerator waitSearchAfterBOOM()
    //{
    //    Destroy(laserTargetGameObjectSearch);
    //    yield return new WaitForSeconds(0.1f);
    //    findAimnOneMoreTime = false;
    //    SearchTargetInUpdate = true;
    //    disAppear = true;

    //}


    //void Update()
    //{
    //    if (goParant.name == "Ship-Player-1")
    //    {
    //        go = GameObject.Find("Ship-Player-2");
    //    }
    //    if (goParant.name == "Ship-Player-2")
    //    {
    //        go = GameObject.Find("Ship-Player-1");
    //    }
    //    IEnumerable<RectTransform> listR = go.GetComponents<RectTransform>().Where(a => a.tag == "Rocket" || a.tag == "RocketS");
    //    foreach (var item in listR)
    //    {
    //        if (item.GetComponent<Rocket>().rocketONAttack)
    //        {
    //            if (offCameraLasser)
    //            {
    //                LR.Points[1] = LR.Points[0];
    //                Debug.Log("ya mydak ne mogy rewit ety problemy");
    //            }
    //        }
    //    }

    //    if (!SearchTargetInUpdate)
    //    {
    //        if (target == RocketCheckTrig)
    //        {
    //            findAimnOneMoreTime = true;
    //        }
    //        if (!offCameraLasser)
    //        {
    //            target = SearchTargetForLaser(goParant);
    //        }
    //        if (offCameraLasser)
    //        {
    //            target = SearchTargetForOffLaser(goParant);
    //        }
    //        if (target != null)
    //        {
    //            TakeSecondPosition(target.position);
    //        }
    //    }

    //    if (RocketCheckTrig != null)
    //    {
    //        Vector2 newVR = new Vector2(RocketCheckTrig.transform.position.x, RocketCheckTrig.transform.position.y);
    //        if (LR.Points[1] == newVR)
    //        {
    //            RocketCheckTrig.GetComponent<Rocket>().HealthRocket -= Time.deltaTime * 200;
    //            RocketCheckTrig.GetComponent<Rocket>().rocketONAttack = true;
    //            if (RocketCheckTrig.GetComponent<Rocket>().HealthRocket <= 0)
    //            {
    //                SearchTargetInUpdate = true;
    //                LR.Points[1] = RocketCheckTrig.transform.position;
    //                RocketCheckTrig.GetComponent<Rocket>().DestroyAndAnimate(gameObject);
    //                //count = 0;

    //                StartCoroutine(waitSearchAfterBOOM());
    //            }
    //        }
    //        else
    //        {
    //            RocketCheckTrig.GetComponent<Rocket>().rocketONAttack = false;
    //        }
    //    }

    //    if (!STARTTHIS)
    //    {
    //        DrawLine();
    //    }

    //    if (disAppear)
    //    {
    //        if (LR.LineThickness > 0)
    //        {
    //            LR.LineThickness -= Time.deltaTime * 350;
    //        }
    //        if (LR.LineThickness < 30)
    //        {
    //            StartCoroutine(destroyLaser());
    //        }
    //    }
    //    if (oneTimeCor && LR.LineThickness <= 0)
    //    {
    //        //StartCoroutine(WaitCycle());
    //        oneTimeCor = false;
    //    }

    //    if (valueHPSystemForLaser <= 0)
    //    {
    //        disAppear = true;
    //        STARTTHIS = true;
    //        STOPTHIS = true;
    //        StartCoroutine(destroyLaser());
    //    }

    //    dist = Vector3.Distance(LR.Points[0], LR.Points[1]);
    //    LR.SetAllDirty();
    //}

    //Transform SearchTargetForLaser(GameObject goParant)
    //{
    //    if (goParant.name == "Ship-Player-1")
    //    {
    //        if (laserTargetGameObjectSearch == null)
    //        {
    //            return null;
    //        }
    //        GameObject ES = GameObject.Find("Ship-Player-2");
    //        IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().AllObjectsFromShip;
    //        float maxV = 0;
    //        if (ESt.Count() == 0 && findShipSystem)
    //        {
    //            laserTargetGameObjectSearch.transform.position = new Vector3(
    //                1080 + ShipPlayer2.transform.position.x, 
    //                goParant.transform.position.y, 
    //                0f);
    //            findAimnOneMoreTime = false;
    //            //count = 0;
    //        }

    //        if (findAimnOneMoreTime)
    //        {
    //            findShipSystem = false;
    //            foreach (RectTransform item in ESt)
    //            {
    //                float valueTime = item.GetComponent<Rocket>().initializationTime;
    //                if (maxV < valueTime)
    //                {
    //                    maxV = valueTime;
    //                    RocketCheckTrig = item.gameObject;
    //                    //findAimnOneMoreTime = false;
    //                    //count = 0;
    //                }
    //            }
    //            RocketUnderAttack = RocketCheckTrig;
    //        }

    //        if (RocketCheckTrig != null)
    //        {
    //            if (RocketCheckTrig.transform.position.y > 1100f)
    //            {
    //                laserTargetGameObjectSearch.transform.position = new Vector3(
    //                    1080 + RocketCheckTrig.transform.position.x,
    //                    goParant.transform.position.y + (RocketCheckTrig.transform.position.y - ES.transform.position.y),
    //                    0f
    //                    );
    //            }
    //            else
    //            {
    //                laserTargetGameObjectSearch.transform.position = RocketCheckTrig.transform.position;
    //            }
    //        }
    //    }
    //    if (goParant.name == "Ship-Player-2")
    //    {
    //        if (laserTargetGameObjectSearch == null)
    //        {
    //            return null;
    //        }
    //        GameObject PS = GameObject.Find("Ship-Player-1");
    //        IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().AllObjectsFromShip;
    //        if (PSt.Count() == 0 && findShipSystem)
    //        {
    //            laserTargetGameObjectSearch.transform.position = new Vector3(
    //                -1080 + ShipPlayer1.transform.position.x,
    //                goParant.transform.position.y, 
    //                0f);
    //            findAimnOneMoreTime = false;
    //        }
    //        float maxV = 0;

    //        if (findAimnOneMoreTime)
    //        {
    //            findShipSystem = false;
    //            foreach (RectTransform item in PSt)
    //            {
    //                float valueTime = item.GetComponent<Rocket>().initializationTime;
    //                if (maxV < valueTime)
    //                {
    //                    maxV = valueTime;
    //                    RocketCheckTrig = item.gameObject;
    //                    //findAimnOneMoreTime = false;
    //                    //count = 0;
    //                }
    //            }
    //            RocketUnderAttack = RocketCheckTrig;
    //        }

    //        if (RocketCheckTrig != null)
    //        {
    //            if (RocketCheckTrig.transform.position.y < 1100f)
    //            {
    //                laserTargetGameObjectSearch.transform.position = new Vector3(
    //                    -1080 + RocketCheckTrig.transform.position.x,
    //                    goParant.transform.position.y + (RocketCheckTrig.transform.position.y - PS.transform.position.y),
    //                    0f
    //                    );
    //            }
    //            else
    //            {
    //                laserTargetGameObjectSearch.transform.position = RocketCheckTrig.transform.position;
    //            }
    //        }
    //    }
    //    return laserTargetGameObjectSearch.transform;
    //}

    //Transform SearchTargetForOffLaser(GameObject goParant)
    //{
    //    if (goParant.name == "Ship-Player-1")
    //    {
    //        if (laserTargetGameObjectSearch == null)
    //        {
    //            return null;
    //        }
    //        GameObject ES = GameObject.Find("Ship-Player-2");
    //        IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().AllObjectsFromShip;
    //        if (ESt.Count() == 0 && findShipSystem)
    //        {
    //            laserTargetGameObjectSearch.transform.position = new Vector3(
    //                ShipPlayer2.transform.position.x,
    //                ShipPlayer2.transform.position.y,
    //                0f);
    //            findAimnOneMoreTime = false;
    //        }
    //        float maxV = 0;

    //        if (findAimnOneMoreTime)
    //        {
    //            findShipSystem = false;
    //            foreach (RectTransform item in ESt)
    //            {
    //                float valueTime = item.GetComponent<Rocket>().initializationTime;
    //                if (item.transform.position.y > 1100f)
    //                {
    //                    if (maxV < valueTime)
    //                    {
    //                        maxV = valueTime;
    //                        RocketCheckTrig = item.gameObject;
    //                        //findAimnOneMoreTime = false;
    //                        //count = 0;
    //                    }
    //                }
    //            }
    //            RocketUnderAttack = RocketCheckTrig;
    //        }

    //        if (RocketCheckTrig != null)
    //        {
    //            if (!RocketCheckTrig.GetComponent<Rocket>().triggerIsActived)
    //            {
    //                laserTargetGameObjectSearch.transform.position = RocketCheckTrig.transform.position;
    //            }
    //            else
    //            {
    //                if (RocketCheckTrig.GetComponent<Rocket>().triggerIsActived)
    //                {
    //                    LR.Points[1] = laserTargetGameObjectSearch.transform.position;
    //                }
    //            }
    //        }
    //    }
    //    if (goParant.name == "Ship-Player-2")
    //    {
    //        if (laserTargetGameObjectSearch == null)
    //        {
    //            return null;
    //        }
    //        GameObject PS = GameObject.Find("Ship-Player-1");
    //        IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().AllObjectsFromShip;
    //        if (PSt.Count() == 0 && findShipSystem)
    //        {
    //            laserTargetGameObjectSearch.transform.position = new Vector3(
    //                ShipPlayer1.transform.position.x,
    //                ShipPlayer1.transform.position.y,
    //                0f);
    //            findAimnOneMoreTime = false;
    //        }
    //        float maxV = 0;

    //        if (findAimnOneMoreTime)
    //        {
    //            foreach (RectTransform item in PSt)
    //            {
    //                findShipSystem = false;
    //                if (item.transform.position.y < 1100f)
    //                {
    //                    float valueTime = item.GetComponent<Rocket>().initializationTime;
    //                    if (maxV < valueTime)
    //                    {
    //                        maxV = valueTime;
    //                        RocketCheckTrig = item.gameObject;
    //                        findShipSystem = false;
    //                        //count = 0;
    //                    }
    //                }
    //            }
    //            RocketUnderAttack = RocketCheckTrig;
    //        }

    //        if (RocketCheckTrig != null)
    //        {
    //            if (!RocketCheckTrig.GetComponent<Rocket>().triggerIsActived)
    //            {
    //                laserTargetGameObjectSearch.transform.position = RocketCheckTrig.transform.position;
    //            }
    //            else
    //            {
    //                if (RocketCheckTrig.GetComponent<Rocket>().triggerIsActived)
    //                {
    //                    LR.Points[1] = laserTargetGameObjectSearch.transform.position;
    //                }
    //            }
    //        }
    //    }
    //    return laserTargetGameObjectSearch.transform;
    //}

    //IEnumerator destroyLaser()
    //{
    //    Destroy(laserTargetGameObjectSearch);
    //    yield return new WaitForSeconds(0.1f);
    //    Destroy(gameObject);

    //}

    //public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    //{
    //    Vector3 P = x * Vector3.Normalize(B - A) + A;
    //    return P;
    //}

    //void DrawLineBack()
    //{
    //    //if (countBack < dist)
    //    //{
    //        //countBack += LineDrawSpeed * Time.deltaTime;

    //        //float x = Mathf.Lerp(0, dist, countBack);

    //    Vector3 pointA = LR.Points[0];
    //    Vector3 pointB = LR.Points[1];

    //    Vector3 pointALongTime = LerpByDistance(pointA, pointB, 170f);

    //    LR.Points[0] = pointALongTime;

    //    if (oneTimeCor)
    //    {
    //        StartCoroutine(WaitCycle());
    //        oneTimeCor = false;
    //    }


    //    if (goParant.name == "Ship-Player-1")
    //    {
    //        if (LR.Points[0].x >= LR.Points[1].x)
    //        {
    //            LR.LineThickness = 0;
    //            //LR.Points[1] = LR.Points[0];
    //        }
    //    }
    //    else
    //    {
    //        if (LR.Points[0].x <= LR.Points[1].x)
    //        {
    //            LR.LineThickness = 0;
    //            //LR.Points[1] = LR.Points[0];
    //        }
    //    }

    //    //}
    //}
}
