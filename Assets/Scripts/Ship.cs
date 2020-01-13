using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public class Ship : MonoBehaviour
{
    [SerializeField]
    private HealthBar healthBar;
    // max value or full hp bar player is 1.35f;
    float health = 1.35f;
    public float curHealth = 1.35f;

    private void Start()
    {
        curHealth = health;
        FunctionPeriodic.Create(() =>
        {
            if (health > curHealth)
            {
                health -= 0.01f;
                healthBar.SetSize(health);
            }
        },
        0.03f);
    }

    void OnCollision2D(Collider2D other)
    {
        Debug.Log(other.name);
    }

    public void ShipTakeDamage(float damage)
    {
        curHealth = health - damage;
    }

    void Update()
    {

    }
}
