using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    public Card.Slot typeOfItem = Card.Slot.WEAPON;
    [SerializeField]
    public bool SlotForCardEmpty = true;
    public delegate void SelectAction(Image target, GameObject cardStats, float currRes, Image iconCard, int timeID);
    public static event SelectAction OnSelectedEvent;

    GameObject ParentCanvas;
    public RectTransform posForR;
    public RectTransform StartForShip;
    [SerializeField]
    public HealthBarSystem healthBar;
    public Image iconSystem;

    private GameObject ShipThisDropZone;

    // DROPZONE instruction how this WORKS!
    // every gameobjects which they are have dropzone, 
    // must have IMAGE or RAWIMAGE with bool value -> RAYCASTTARGET = true

    void Start()
    {
        ShipThisDropZone = gameObject.transform.parent.gameObject;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log(eventData.pointerDrag.name + "was drop to " + gameObject.name);
        Card d = eventData.pointerDrag.GetComponent<Card>();
        GameObject p = eventData.pointerDrag;
        ParentCanvas = gameObject.transform.root.gameObject;
        float GetCurRes = ParentCanvas.GetComponent<PlayerStats>().GetCurrResource;
        float EnoughResForCardDrop = GetCurRes - Convert.ToInt32(d.Cost.text);
        if (gameObject.tag == "SlotGun")
        {
            if (ShipThisDropZone.GetComponent<Ship>().shieldActive && d.Icon.sprite.name == "shield")
            {
                SlotForCardEmpty = false;
            }
            else
            {
                SlotForCardEmpty = true;
            }
            //gameObject.tag = "SlotGunFull";
            if (d != null && SlotForCardEmpty)
            {
                Image iconGun = d.Icon;
                //image = gameObject.GetComponent<Image>();
                iconSystem.color = new Color(iconSystem.color.r, iconSystem.color.g, iconSystem.color.b, 255f);
                iconSystem.GetComponent<Image>().sprite = iconGun.sprite;
                d.parentToReturnTo = posForR.transform;
                // this bad solution
                // something wrong on this eventData? Can't do set point parentToReturn correct
                // object Card exist, but not delete
                iconGun.SetNativeSize();
                Destroy(d.Icon);
                Destroy(d.Cost);
                Destroy(d.CardName);
                Destroy(d.GetComponent<Image>());
                SlotForCardEmpty = false;
                StartCoroutine(ResetSlotDeleteIcon(iconGun));
                //Invoke Event check in playerControls script

                if (OnSelectedEvent != null)
                {
                    //RocketId idR = new RocketId();

                    int timeID = RandomIDforRocket();
                    OnSelectedEvent(iconSystem, eventData.pointerDrag, EnoughResForCardDrop, iconGun, timeID);
                }
            }
        }
    }

    int RandomIDforRocket()
    {
        int idRocket = UnityEngine.Random.Range(1000000, 9999999);
        return idRocket;
    }

    IEnumerator ResetSlotDeleteIcon(Image icon)
    {
        yield return new WaitForSeconds(GameConstants.TIME_ROCKET_SYSTEM);
        SlotForCardEmpty = true;
        //image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        //gameObject.tag = "SlotGun";
    }

    void Update()
    {
        if (healthBar.startReduce)
        {
            healthBar.gameObject.GetComponent<Canvas>().enabled = true;
            //gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            healthBar.gameObject.GetComponent<Canvas>().enabled = false;
            //gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit");
    }
}
