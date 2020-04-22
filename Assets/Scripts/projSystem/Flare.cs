using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Flare : MonoBehaviour
{
    public float initializationTime;
    private GameObject goPar;
    Transform TargetForRocket;
    public bool trueRocket;
    [SerializeField]
    public float HealthRocket = 100f;
    float randomNumber;
    public static int movespeed = 280;
    public Vector3 userDirection = Vector3.right;
    private float SystemRocketDamageOnHPbar = 0.15f;
    public bool flareONAttack;
    public bool onDestoyTime;
    public GameObject AIMed;

    void Start()
    {
        StartCoroutine(disableCollider());
        initializationTime = Time.time;
        goPar = gameObject.transform.parent.gameObject;

        if (!trueRocket && goPar.name == "Ship-Player-1")
        {
            GameObject go = GameObject.Find("Ship-Player-2");
            //SearchTargetEmptySystem(goPar);
            gameObject.transform.position = new Vector3(
                -1080f + gameObject.transform.position.x,
                go.transform.position.y + (gameObject.transform.position.y - goPar.transform.position.y),
                0f
                );
        }
        if (!trueRocket && goPar.name == "Ship-Player-2")
        {
            GameObject go = GameObject.Find("Ship-Player-1");
            //SearchTargetEmptySystem(goPar);
            gameObject.transform.position = new Vector3(
                1080f + gameObject.transform.position.x,
                go.transform.position.y + (gameObject.transform.position.y - goPar.transform.position.y),
                0f
                );
        }
        Invoke("CheckAimed", 1.47f);
        Destroy(gameObject, 1.5f);
    }

    IEnumerator disableCollider()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.03f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    IEnumerator DestroyFlarebyTime(float t)
    {
        yield return new WaitForSeconds(t);
        onDestoyTime = true;
        Destroy(gameObject);
    }

    public float TakeRandomNumberForSearch(float _randomNumber)
    {
        randomNumber = _randomNumber;
        return randomNumber;
    }

    public void GetAimedRocket(GameObject go)
    {
        AIMed = go;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (TargetForRocket != null)
        {
            if (other.tag == "SlotGun" && other.name == TargetForRocket.name)
            {
                gameObject.SetActive(false);
                HealthRocket = 0;
                DestroyFlarebyTime(0.05f);
            }
            if (other.tag == "SlotGunFull" && other.name == TargetForRocket.name)
            {
                DropZone dz = other.GetComponent<DropZone>();
                dz.healthBar.ReduceHPSystem(SystemRocketDamageOnHPbar, gameObject);
                GameObject icon = other.gameObject.GetComponentInChildren<DropZone>().iconSystem.gameObject;
                Ship go = other.transform.parent.gameObject.GetComponent<Ship>();
                if (go.shieldActive)
                {
                    if (icon.GetComponent<Image>().sprite.name == "shield")
                    {
                        go.shieldImage.gameObject.GetComponent<Shield>().TakeNewColorA();
                        go.shieldImage.gameObject.GetComponent<Shield>().TakeDamage(SystemRocketDamageOnHPbar);
                    }
                }
                gameObject.SetActive(false);
                HealthRocket = 0;
                DestroyFlarebyTime(0.05f);
            }
        }
        //if (other.tag == "Rocket")
        //{
        //    other.gameObject.GetComponent<Rocket>().flareInCircle = true;
        //}
        if (other.tag=="Destroyer")
        {
            CheckAimed();
            Destroy(gameObject);
        }
    }

    void CheckAimed()
    {
        if (AIMed != null)
        {
            AIMed.GetComponent<Rocket>().changeTargetAgain = true;
        }
    }

    void Update()
    {
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;
        gameObject.transform.Translate(userDirection * movespeed * Time.deltaTime);
        if (goPar.name == "Ship-Player-2")
        {
            if (randomNumber >= -90 && randomNumber <= 0)
            {
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -180 - randomNumber);
            }
            if (randomNumber <= 90 && randomNumber >= 0)
            {
                gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 180 - randomNumber);
            }
        }
        else
        {
            gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, randomNumber);
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
}
