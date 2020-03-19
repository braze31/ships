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
        rt = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGunFull" && i != null);
        //slotGunsEmpty = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGun" && i != null);
        AllObjectsFromShip = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(
            a => a.tag == "Rocket" || a.tag == "RocketS" && a != null
            );
    }

    //public IEnumerable<RectTransform> FindAllSlotGunsFull()
    //{
    //    rt = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGunFull" && i != null);
    //    return rt;
    //}

    public IEnumerable<RectTransform> FindAllSlotGunsEmpty()
    {
        slotGunsEmpty = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGun" && i != null);
        return slotGunsEmpty;
    }

    //public IEnumerable<RectTransform> FindAllObjectsFromShip()
    //{
    //    AllObjectsFromShip = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(
    //        a => a.tag == "Rocket" || a.tag == "RocketS" && a != null
    //        );
    //    return AllObjectsFromShip;
    //}

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
