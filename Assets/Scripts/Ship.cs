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
        rt = gameObject.transform.GetComponentsInChildren<RectTransform>().Where(i => i.tag == "SlotGunFull");
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
