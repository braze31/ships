using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        curHealth = health;
        //FunctionPeriodic.Create(() =>
        //{
        //    if (health > curHealth)
        //    {
        //        health -= 0.01f;
        //        healthBar.SetSize(health);
        //    }
        //},
        //0.03f);
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
    }

    void Update()
    {
        textHP.GetComponent<Text>().text = curHealth.ToString();
    }
}
