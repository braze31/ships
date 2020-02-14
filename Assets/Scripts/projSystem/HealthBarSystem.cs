using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSystem : MonoBehaviour
{
    public Image bar;
    public bool startReduce;
    float fill = 1f;
    float time;
    public bool stopSystem;
    private Image icon;

    void Start()
    {
        bar = transform.Find("BarRed").GetComponent<Image>();
        icon = gameObject.transform.parent.GetComponent<Image>();
    }

    public void EnableImageAndStartReduceHp(float liveTime)
    {
        fill = 1f;
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 255f);
        startReduce = true;
        time = 1/liveTime;
    }

    public void ReduceHPSystem(float damage)
    {
        if (startReduce)
        {
            fill -= damage;
            if (fill<=0)
            {
                StopSystem();
            }
        }
    }

    public void StopSystem()
    {
        startReduce = false;
        stopSystem = true;
        gameObject.GetComponent<Canvas>().enabled = false;
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        icon.gameObject.tag = "SlotGun";
        icon.gameObject.GetComponent<DropZone>().SlotForCardEmpty = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startReduce)
        {
            fill -= Time.deltaTime * time;
            bar.fillAmount = fill;
            if (bar.fillAmount <= 0)
            {
                startReduce = false;
                gameObject.GetComponent<Canvas>().enabled = false;
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
                icon.gameObject.tag = "SlotGun";
                icon.gameObject.GetComponent<DropZone>().SlotForCardEmpty = true;
            }
        }
    }
}
