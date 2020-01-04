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
    private bool canDrag = false;
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
    private GameObject playerCanvas;
    private CanvasGroup checkForDrag;
    public Image blockImage;

    public enum Slot { WEAPON, SUPPLY};
    public Slot typeOfItem = Slot.WEAPON;

    void Start()
    {
        playerCanvas = gameObject.transform.parent.root.gameObject;
        checkForDrag = gameObject.GetComponent<CanvasGroup>();
    }

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
                blockImage.gameObject.GetComponent<Image>().enabled = false;
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
        if (playerCanvas.GetComponent<PlayerStats>().CurrResource >= CardInfo.Cost)
        {
            canDrag = true;
            checkForDrag.blocksRaycasts = true;
            blockImage.gameObject.SetActive(false);
        }
        if (playerCanvas.GetComponent<PlayerStats>().CurrResource < CardInfo.Cost)
        {
            blockImage.gameObject.SetActive(true);
        }

        // ?? need fix after drag and drop red zone on card
        // this fix simple code to delete problem, but !
        // bug, again show if i drag card


    }
}
