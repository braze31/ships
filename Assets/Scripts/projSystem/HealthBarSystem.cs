using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSystem : MonoBehaviour
{
    private Image bar;
    public bool startReduce;
    float fill = 1f;
    float time;

    void Start()
    {
        bar = transform.Find("BarRed").GetComponent<Image>();
    }

    public void EnableImageAndStartReduceHp(float liveTime)
    {
        startReduce = true;
        time = 1/liveTime;
    }

    public void ReduceHPSystem(float damage)
    {
        if (startReduce)
        {
            fill -= damage;
        }
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
                fill = 1f;
                bar.fillAmount = 1f;
            }
        }
    }
}
