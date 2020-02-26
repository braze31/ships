using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Laser : MonoBehaviour
{
    GameObject ShipPlayer1;
    GameObject ShipPlayer2;
    GameObject goParant;
    GameObject RocketCheckTrig;
    Transform target;
    public UILineRenderer LR;
    public float LineDrawSpeed;

    private bool offCameraLasser;
    private GameObject laserTargetGameObjectSearch;
    private bool findShipSystem;
    private bool findAimnOneMoreTime = true;
    private bool ShowDrawBackStopSearch = false;
    private float dist;
    private float count;
    private Vector2 secondPoint;
    private float countBack;

    private bool RememberStartPoint;
    private bool RememberSecondPoint;
    Vector3 firstPosPoint;
    Vector3 SecondPosPoint;

    private float timeForBackDraw = 0.0f;
    public float interpolationPeriod = 4f;
    private float checkDistCor;
    private bool repeatCycle;
    private bool disAppear;
    private bool WAITFULLLASER;

    public bool STOPTHIS = false;
    public bool SwapPoints = false;

    void Awake()
    {
        ShipPlayer1 = GameObject.Find("Ship-Player-1");
        ShipPlayer2 = GameObject.Find("Ship-Enemy");
    }
    void Start()
    {
        goParant = gameObject.transform.parent.gameObject;
        laserTargetGameObjectSearch = new GameObject("MyLaser", typeof(RectTransform));
    }

    public void TakeStartPosition(Vector3 pos)
    {
        if (!RememberStartPoint)
        {
            firstPosPoint = pos;
            RememberStartPoint = true;
        }
        LR.Points[0] = pos;
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
        }
        if (pos.y > 1100f)
        {
            LR.Points[0] = new Vector3(
                1080 + pos.x, 
                ShipPlayer1.transform.position.y + (pos.y - ShipPlayer2.transform.position.y),
                pos.z);
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
        dist = Vector3.Distance(LR.Points[0], LR.Points[1]);
    }

    IEnumerator WaitStartDrawBack()
    {
        yield return new WaitForSeconds(1.5f);
        WAITFULLLASER = true;
    }

    IEnumerator DisappearLine()
    {
        yield return new WaitForSeconds(0.3f);
        disAppear = true;
        yield return new WaitForSeconds(0.21f);
        STOPTHIS = true;
    }

    void DrawLine()
    {
        if (count < dist)
        {
            if (LR.LineThickness < 100)
            {
                LR.LineThickness += Time.deltaTime * 150;
            }
            count += LineDrawSpeed * Time.deltaTime;

            float x = Mathf.Lerp(0, dist, count);

            Vector3 pointA = LR.Points[0];
            Vector3 pointB = laserTargetGameObjectSearch.transform.position;

            Vector3 pointALongTime = x * Vector3.Normalize(pointB - pointA) + pointA;

            LR.Points[1] = pointALongTime;

            StartCoroutine(WaitStartDrawBack());
            if (WAITFULLLASER)
            {
                ShowDrawBackStopSearch = true;
            }
            if (ShowDrawBackStopSearch)
            {
                StartCoroutine(DisappearLine());
            }
        }
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    void DrawLineBack()
    {
        if (countBack < dist)
        {
            countBack += LineDrawSpeed * Time.deltaTime;

            float x = Mathf.Lerp(0, dist, countBack);

            Vector3 pointA = LR.Points[0];
            Vector3 pointB = laserTargetGameObjectSearch.transform.position;

            Vector3 pointALongTime = LerpByDistance(pointA, pointB, 140f);

            LR.Points[0] = pointALongTime;

            if (goParant.name == "Ship-Player-1")
            {
                if (LR.Points[0].x > LR.Points[1].x)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if (LR.Points[0].x < LR.Points[1].x)
                {
                    Destroy(gameObject);
                }
            }

        }
    }
    
    void Update()
    {
        if (!ShowDrawBackStopSearch)
        {
            if (!offCameraLasser)
            {
                target = SearchTargetForLaser(goParant);
            }
            if (offCameraLasser)
            {
                target = SearchTargetForOffLaser(goParant);
            }
            if (target != null)
            {
                TakeSecondPosition(target.position);
                DrawLine();
            }
        }
        if (STOPTHIS)
        {
            DrawLineBack();
        }

        if (disAppear)
        {
            if (LR.LineThickness > 0)
            {
                LR.LineThickness -= Time.deltaTime * 150;
            }
        }

        if (RocketCheckTrig != null)
        {
            RocketCheckTrig.GetComponent<Rocket>().HealthRocket -= Time.deltaTime * 50;
            if (RocketCheckTrig.GetComponent<Rocket>().HealthRocket <= 0)
            {
                RocketCheckTrig.GetComponent<Rocket>().DestroyAndAnimate(gameObject);
                count = 0;
            }
        }
        dist = Vector3.Distance(LR.Points[0], LR.Points[1]);
        LR.SetAllDirty();
    }

    Transform SearchTargetForLaser(GameObject goParant)
    {
        if (goParant.name == "Ship-Player-1")
        {
            GameObject ES = GameObject.Find("Ship-Player-2");
            IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().AllObjectsFromShip;
            float maxV = 0;
            if (ESt.Count() == 0 && !findShipSystem)
            {
                laserTargetGameObjectSearch.transform.position = new Vector3(
                    1080 + ShipPlayer2.transform.position.x, 
                    goParant.transform.position.y, 
                    0f);
                count = 0;
                findShipSystem = true;
            }
            if (findAimnOneMoreTime)
            {
                foreach (RectTransform item in ESt)
                {
                    float valueTime = item.GetComponent<Rocket>().initializationTime;
                    if (maxV < valueTime)
                    {
                        maxV = valueTime;
                        RocketCheckTrig = item.gameObject;
                        findAimnOneMoreTime = false;
                        findShipSystem = false;
                        //count = 0;
                    }
                }
            }
            if (RocketCheckTrig != null)
            {
                if (RocketCheckTrig.transform.position.y > 1100f)
                {
                    laserTargetGameObjectSearch.transform.position = new Vector3(
                        1080 + RocketCheckTrig.transform.position.x,
                        goParant.transform.position.y + (RocketCheckTrig.transform.position.y - ES.transform.position.y),
                        0f
                        );
                }
                else
                {
                    laserTargetGameObjectSearch.transform.position = RocketCheckTrig.transform.position;
                }
            }
        }
        if (goParant.name == "Ship-Player-2")
        {
            GameObject PS = GameObject.Find("Ship-Player-1");
            IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().AllObjectsFromShip;
            if (PSt.Count() == 0 && !findShipSystem)
            {
                laserTargetGameObjectSearch.transform.position = new Vector3(
                    -1080 + ShipPlayer1.transform.position.x,
                    goParant.transform.position.y, 
                    0f);
                count = 0;
                findShipSystem = true;
            }
            float maxV = 0;

            if (findAimnOneMoreTime)
            {
                foreach (RectTransform item in PSt)
                {
                    float valueTime = item.GetComponent<Rocket>().initializationTime;
                    if (maxV < valueTime)
                    {
                        maxV = valueTime;
                        RocketCheckTrig = item.gameObject;
                        findAimnOneMoreTime = false;
                        findShipSystem = false;
                        //count = 0;
                    }
                }

            }
            if (RocketCheckTrig != null)
            {
                if (RocketCheckTrig.transform.position.y < 1100f)
                {
                    laserTargetGameObjectSearch.transform.position = new Vector3(
                        -1080 + RocketCheckTrig.transform.position.x,
                        goParant.transform.position.y + (RocketCheckTrig.transform.position.y - PS.transform.position.y),
                        0f
                        );
                }
                else
                {
                    laserTargetGameObjectSearch.transform.position = RocketCheckTrig.transform.position;
                }
            }
        }
        findAimnOneMoreTime = true;
        return laserTargetGameObjectSearch.transform;
    }

    Transform SearchTargetForOffLaser(GameObject goParant)
    {
        if (goParant.name == "Ship-Player-1")
        {
            GameObject ES = GameObject.Find("Ship-Player-2");
            IEnumerable<RectTransform> ESt = ES.GetComponent<Ship>().AllObjectsFromShip;
            if (ESt.Count() == 0 && !findShipSystem)
            {
                laserTargetGameObjectSearch.transform.position = new Vector3(
                    ShipPlayer2.transform.position.x,
                    ShipPlayer2.transform.position.y,
                    0f);
                count = 0;
                findShipSystem = true;
            }
            float maxV = 0;
            if (findAimnOneMoreTime)
            {
                foreach (RectTransform item in ESt)
                {
                    float valueTime = item.GetComponent<Rocket>().initializationTime;
                    if (item.transform.position.y > 1100f)
                    {
                        if (maxV < valueTime)
                        {
                            maxV = valueTime;
                            RocketCheckTrig = item.gameObject;
                            findAimnOneMoreTime = false;
                            findShipSystem = false;
                            //count = 0;
                        }
                    }
                }
            }
            if (RocketCheckTrig != null)
            {
                if (!RocketCheckTrig.GetComponent<Rocket>().triggerIsActived)
                {
                    laserTargetGameObjectSearch.transform.position = RocketCheckTrig.transform.position;
                }
                else
                {
                    if (RocketCheckTrig.GetComponent<Rocket>().triggerIsActived)
                    {
                        LR.Points[1] = new Vector3(0, 0);
                    }
                }
            }
        }
        if (goParant.name == "Ship-Player-2")
        {
            GameObject PS = GameObject.Find("Ship-Player-1");
            IEnumerable<RectTransform> PSt = PS.GetComponent<Ship>().AllObjectsFromShip;
            if (PSt.Count() == 0 && !findShipSystem)
            {
                laserTargetGameObjectSearch.transform.position = new Vector3(
                    ShipPlayer1.transform.position.x,
                    ShipPlayer1.transform.position.y,
                    0f);
                count = 0;
                findShipSystem = true;
            }
            float maxV = 0;
            if (findAimnOneMoreTime)
            {
                foreach (RectTransform item in PSt)
                {
                    if (item.transform.position.y < 1100f)
                    {
                        float valueTime = item.GetComponent<Rocket>().initializationTime;
                        if (maxV < valueTime)
                        {
                            maxV = valueTime;
                            RocketCheckTrig = item.gameObject;
                            findAimnOneMoreTime = false;
                            findShipSystem = false;
                            //count = 0;
                        }
                    }
                }
            }
            if (RocketCheckTrig != null)
            {
                if (!RocketCheckTrig.GetComponent<Rocket>().triggerIsActived)
                {
                    laserTargetGameObjectSearch.transform.position = RocketCheckTrig.transform.position;
                }
                else
                {
                    if (RocketCheckTrig.GetComponent<Rocket>().triggerIsActived)
                    {
                        LR.Points[1] = new Vector3(1200, 0);
                    }
                }
            }
        }
        findAimnOneMoreTime = true;
        return laserTargetGameObjectSearch.transform;
    }



    //public UILineRenderer UILineScript;
    //public float WaitLineThicknessFull = 1.5f;
    //GameObject goParant;
    //private float time;
    //bool animateLaserFill;
    //Transform targetForLaser;
    //GameObject laserTargetGameObjectSearch;
    //private Transform searchCorrectTarget;
    //GameObject RocketCheckTrig;
    //private bool findAimnOneMoreTime = true;
    //GameObject ShipEnemy;
    //GameObject ShipPlayer1;
    //GameObject ShipPlayer2;
    //private bool offCameraLasser;
    //Transform startPoint;
    //bool checkLine;
    //public bool StopDrawLine { get; private set; }

    //private float count;
    //private float countBack;
    //private float dist;
    //public float LineDrawSpeed;
    //private Transform secondPoint;
    //private bool findShipSystem;
    //private bool startBackLine;

    //// Use this for initialization
    //void Awake()
    //{
    //    ShipPlayer1 = GameObject.Find("Ship-Player-1");
    //    ShipPlayer2 = GameObject.Find("Ship-Enemy");
    //}
    //void Start()
    //{
    //    goParant = gameObject.transform.parent.gameObject;
    //    laserTargetGameObjectSearch = new GameObject("MyLaser", typeof(RectTransform));
    //    //laserTargetGameObjectSearch.AddComponent<Rigidbody2D>();
    //    //laserTargetGameObjectSearch.GetComponent<Rigidbody2D>().gravityScale = 0f;
    //    //if (!offCameraLasser)
    //    //{
    //    //    //targetForLaser = laserTargetGameObjectSearch.transform;
    //    //    if (goParant.name == "Ship-Player-1")
    //    //    {
    //    //        ShipEnemy = GameObject.Find("Ship-Enemy");
    //    //        targetForLaser.transform.position = new Vector3(1080 + ShipEnemy.transform.position.x, goParant.transform.position.y, 0f);
    //    //    }
    //    //    else
    //    //    {
    //    //        ShipEnemy = GameObject.Find("Ship-Player-1");
    //    //        targetForLaser.transform.position = new Vector3(-1080 + ShipEnemy.transform.position.x, goParant.transform.position.y, 0f);
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    //targetForLaser = laserTargetGameObjectSearch.transform;
    //    //    if (goParant.name == "Ship-Player-1")
    //    //    {
    //    //        ShipEnemy = GameObject.Find("Ship-Enemy");
    //    //        targetForLaser.transform.position = new Vector3(ShipEnemy.transform.position.x, ShipEnemy.transform.position.y, 0f);
    //    //    }
    //    //    else
    //    //    {
    //    //        ShipEnemy = GameObject.Find("Ship-Player-1");
    //    //        targetForLaser.transform.position = new Vector3(ShipEnemy.transform.position.x, ShipEnemy.transform.position.y, 0f);
    //    //    }
    //    //}
    //}

    //public void TakeSecondPosition(Transform GameObjectPos)
    //{
    //    //secondPoint = GameObjectPos;
    //    UILineScript.Points[1] = GameObjectPos.position;
    //    //Debug.Log(UILineScript.Points[1]);
    //   // dist = Vector3.Distance(UILineScript.Points[0], GameObjectPos.position);
    //}

    //public void TakeStartPosition(Transform pos)
    //{
    //    UILineScript.Points[0] = pos.position;
    //}

    //public void TakeStartPositionOFFLaser(Transform pos)
    //{
    //    offCameraLasser = true;
    //    if (pos.position.y < 1100f)
    //    {
    //        UILineScript.Points[0] = new Vector3(-1080 + pos.position.x, ShipPlayer2.transform.position.y + (pos.position.y - ShipPlayer1.transform.position.y), pos.position.z);
    //    }
    //    if (pos.position.y > 1100f)
    //    {
    //        UILineScript.Points[0] = new Vector3(1080 + pos.position.x, ShipPlayer1.transform.position.y + (pos.position.y - ShipPlayer2.transform.position.y), pos.position.z);
    //    }
    //}


    ////void DrawLineBack()
    ////{
    ////    if (countBack < dist)
    ////    {
    ////        countBack += LineDrawSpeed * Time.deltaTime;

    ////        float x = Mathf.Lerp(0, dist, countBack);

    ////        Vector3 pointA = UILineScript.Points[0];
    ////        Vector3 pointB = secondPoint.transform.position;

    ////        Vector3 pointALongTime = x * Vector3.Normalize(pointB - pointA) + pointA;
    ////        UILineScript.Points[0] = pointALongTime;
    ////    }
    ////}
    //void DrawLine()
    //{
    //    if (count < dist)
    //    {
    //        if (UILineScript.LineThickness < 100)
    //        {
    //            UILineScript.LineThickness += Time.deltaTime * 300;
    //        }
    //        count += LineDrawSpeed * Time.deltaTime;

    //        float x = Mathf.Lerp(0, dist, count);

    //        Vector3 pointA = UILineScript.Points[0];
    //        Vector3 pointB = laserTargetGameObjectSearch.transform.position;

    //        Vector3 pointALongTime = x * Vector3.Normalize(pointB - pointA) + pointA;

    //        UILineScript.Points[1] = pointALongTime;
    //    }
    //}

    //public void TimeLife(float time)
    //{
    //    this.time = time;
    //}

    //void Update()
    //{
    //    targetForLaser = SearchTargetForLaser(goParant);

    //        Transform positionA = targetForLaser.transform;
    //        Debug.Log(positionA.position);
    //        TakeSecondPosition(positionA);

    //    //if (!offCameraLasser)
    //    //{

    //    //    //DrawLine();
    //    //    //TakeSecondPosition(targetForLaser.transform);
    //    //}
    //    //if (offCameraLasser)
    //    //{
    //    //    targetForLaser = SearchTargetForOffLaser(goParant);
    //    //    if (targetForLaser != null)
    //    //    {
    //    //        Transform positionA = targetForLaser.transform;
    //    //        TakeSecondPosition(positionA);
    //    //    }
    //    //    //targetForLaser = SearchTargetForOffLaser(goParant);
    //    //    //DrawLine();
    //    //    //TakeSecondPosition(targetForLaser.transform);
    //    //}

    //    //if (RocketCheckTrig!=null)
    //    //{
    //    //    RocketCheckTrig.GetComponent<Rocket>().HealthRocket -= Time.deltaTime * 50;
    //    //    if (RocketCheckTrig.GetComponent<Rocket>().HealthRocket <= 0)
    //    //    {
    //    //        RocketCheckTrig.GetComponent<Rocket>().DestroyAndAnimate(gameObject);
    //    //        findAimnOneMoreTime = true;
    //    //        //UILineScript.Points[0] = startPoint.transform.position;
    //    //    }
    //    //}

    //    //if (!offCameraLasser)
    //    //{
    //    //    targetForLaser = SearchTargetForLaser(goParant);
    //    //    if (targetForLaser != null)
    //    //    {
    //    //        Transform positionA = targetForLaser.transform;
    //    //        TakeStartPosition(positionA);
    //    //    }
    //    //}
    //    //if (offCameraLasser)
    //    //{
    //    //    targetForLaser = SearchTargetForOffLaser(goParant);
    //    //    if (targetForLaser != null)
    //    //    {
    //    //        Transform positionA = targetForLaser.transform;
    //    //        TakeStartPosition(positionA);
    //    //    }
    //    //}


    //    //if (!animateLaserFill)
    //    //{
    //    //    if (UILineScript.LineThickness < 100)
    //    //    {
    //    //        UILineScript.LineThickness += Time.deltaTime * 150;

    //    //    }
    //    //    else
    //    //    {
    //    //        //StartCoroutine(HoldLineThickness(WaitLineThicknessFull));
    //    //        animateLaserFill = true;
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    if (UILineScript.LineThickness > 0)
    //    //    {
    //    //        UILineScript.LineThickness -= Time.deltaTime * 150;
    //    //        //DrawLineBack();
    //    //    }
    //    //    else
    //    //    {
    //    //        Destroy(gameObject);
    //    //    }
    //    //}

    //}

    //IEnumerator HoldLineThickness(float sec)
    //{
    //    yield return new WaitForSeconds(sec);
    //    animateLaserFill = true;
    //}
}
