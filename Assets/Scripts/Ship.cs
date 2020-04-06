using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    [SerializeField]
    private HealthBar healthBar;
    // max value or full hp bar player is 1f;
    float health = 1f;
    public float curHealth = 1f;
    public Text textHP;
    public bool DamageDone = false;
    [SerializeField]
    public IEnumerable<RectTransform> rt;
    [SerializeField]
    public IEnumerable<RectTransform> slotGunsEmpty;
    [SerializeField]
    public IEnumerable<RectTransform> AllObjectsFromShip;
    [SerializeField]
    public IEnumerable<RectTransform> AllRocketsFromShip;
    [SerializeField]
    public IEnumerable<RectTransform> AllBombFromShip;
    [SerializeField]
    public IEnumerable<RectTransform> AllFlareFromShip;
    public RawImage shieldImage;
    public bool shieldActive = false;
    public Transform ShieldPosforShip;
    private void Start()
    {
        curHealth = health;
    }

    void OnCollision2D(Collider2D other)
    {
        Debug.Log(other.name);
    }

    public void ShipTakeDamage(float damage)
    {
        curHealth = health - damage;
        health = curHealth;
        healthBar.SetSize(damage);
        DamageDone = true;
    }

    void Update()
    {
        textHP.GetComponent<Text>().text = curHealth.ToString();
        //rt = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGunFull" && i != null);
        //slotGunsEmpty = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGun" && i != null);
    }

    public IEnumerable<RectTransform> FindAllObjectsFromShip()
    {
        AllObjectsFromShip = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(
            a => a.tag == "Rocket" || a.tag == "RocketS" 
            || a.tag == "Bomb" || a.tag == "Flare"
            && a != null && !a.Equals(null)
            );
        return AllObjectsFromShip;
    }

    public IEnumerable<RectTransform> FindAllSlotGunsFull()
    {
        rt = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGunFull" && i != null && !i.Equals(null));
        return rt;
    }

    public IEnumerable<RectTransform> FindAllSlotGunsEmpty()
    {
        slotGunsEmpty = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGun" && i != null && !i.Equals(null));
        return slotGunsEmpty;
    }

    void FixUpdate()
    {
        AllRocketsFromShip = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(
            a => a.tag == "Rocket" || a.tag == "RocketS" && a != null && !a.Equals(null)
            );
    }

    public IEnumerable<RectTransform> FindAllRocketromShip()
    {
        AllRocketsFromShip = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(
            a => a.tag == "Rocket" || a.tag == "RocketS" && a != null && !a.Equals(null)
            );
        return AllRocketsFromShip;
    }

    public IEnumerable<RectTransform> FindAllBombFromShip()
    {
        AllBombFromShip = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(
            a => a.tag == "Bomb" && a != null && !a.Equals(null)
            );
        return AllBombFromShip;
    }

    public IEnumerable<RectTransform> FindAllFlareFromShip()
    {
        AllFlareFromShip = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(
            a => a.tag == "Flare" && a != null && !a.Equals(null) //&& !a.GetComponent<Flare>().flareONAttack
            );
        return AllFlareFromShip;
    }

    //void OnEnable()
    //{
    //    //subscribe to event
    //    Rocket.OnSelectedEvent += SelectAction;
    //}

    //void OnDisable()
    //{
    //    //Un-subscribe to event
    //    Rocket.OnSelectedEvent -= SelectAction;
    //}

    //void SelectAction(int id)
    //{

    //}
}
