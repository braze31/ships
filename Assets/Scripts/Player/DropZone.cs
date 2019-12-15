using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    public Card.Slot typeOfItem = Card.Slot.WEAPON;
    [SerializeField]
    private bool SlotForCardEmpty = true;
    public delegate void SelectAction(GameObject target);
    public static event SelectAction OnSelectedEvent;
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log(eventData.pointerDrag.name + "was drop to " + gameObject.name);
        Card d = eventData.pointerDrag.GetComponent<Card>();

        if (gameObject.tag == "SlotGun")
        {
            if (d != null && SlotForCardEmpty)
            {
                Image iconGun = d.Icon;
                image = gameObject.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, 255f);
                gameObject.GetComponent<Image>().sprite = iconGun.sprite;
                d.parentToReturnTo = this.transform;
                // this bad solution
                // something wrong on this eventData? Can't do set point parentToReturn correct
                // object Card exist, but not delete
                Destroy(d.Icon);
                Destroy(d.Cost);
                Destroy(d.CardName);
                Destroy(d.GetComponent<Image>());
                SlotForCardEmpty = false;
                //Invoke Event check in playerControls script
                if (OnSelectedEvent != null)
                {
                    OnSelectedEvent(this.gameObject);
                }
            }
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
