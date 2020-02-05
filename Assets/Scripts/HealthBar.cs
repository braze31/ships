using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image BarHPstatus;
    private bool damageStatus = false;
    private float UNTILdamage;
    private float AFTERdamage;

    private void Start()
    {
        UNTILdamage = 1f;
        AFTERdamage = 1f;
    }

    // vector3(1.35f,0.1f) - max or full HP player
    public void SetSize(float sizeNormalized)
    {
        damageStatus = true;
        AFTERdamage = BarHPstatus.fillAmount - sizeNormalized;
    }

    private void Update()
    {
        while (damageStatus)
        {
            if (UNTILdamage <= AFTERdamage)
            {
                damageStatus = false;
                break;
            }
            UNTILdamage -= Time.deltaTime * 0.1f;
            BarHPstatus.fillAmount = UNTILdamage;
        }
    }
}
