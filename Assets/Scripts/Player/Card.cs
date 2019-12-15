using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private PlayerStats playerInfo;
    [SerializeField]
    private CardStats cardInfo;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text cardName;
    [SerializeField]
    private Text cost;
    [SerializeField]
    private bool canDrag;
    public Transform parentToReturnTo = null;

    public bool CanDrag
    {
        get { return canDrag; }
        set { canDrag = value; }
    }
    public PlayerStats PlayerInfo
    {
        get { return playerInfo; }
        set { playerInfo = value; }
    }
    public CardStats CardInfo
    {
        get { return cardInfo; }
        set { cardInfo = value; }
    }
    public Image Icon
    {
        get { return icon; }
    }
    public Text CardName
    {
        get { return cardName; }
    }
    public Text Cost
    {
        get { return cost; }
    }

    public enum Slot { WEAPON, SUPPLY};
    public Slot typeOfItem = Slot.WEAPON;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!playerInfo.OnDragging)
        {
            if (canDrag)
            {
                playerInfo.OnDragging = true;
                parentToReturnTo = this.transform.parent;
                this.transform.SetParent(GameFunctions.GetCanvas());
                GetComponent<CanvasGroup>().blocksRaycasts = false;

                //this.transform.SetParent(GameFunctions.GetCanvas());
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (playerInfo.OnDragging)
        {
            //transform.position = Input.mousePosition;
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);
        //Debug.Log(parentToReturnTo.name);
        //GameObject go = eventData.pointerCurrentRaycast.gameObject;
        //if (go != null)
        //{
        //    GameFunctions.SpawnUI(cardInfo.PreFab, playerInfo.UnitTransform, Input.mousePosition);
        //}
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        playerInfo.OnDragging = false;
    }

    private void Update()
    {
        icon.sprite = cardInfo.Icon;
        cardName.text = cardInfo.Name;
        cost.text = cardInfo.Cost.ToString();
    }
}
