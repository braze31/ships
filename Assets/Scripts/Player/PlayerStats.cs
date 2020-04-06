using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Deck playerDeck;
    [SerializeField]
    private List<Image> resources;
    [SerializeField]
    private int score;
    [SerializeField]
    private float currResource;
    [SerializeField]
    private Text textCurrResource;
    [SerializeField]
    private Text textMaxResource;
    [SerializeField]
    private Text textScore;
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private Transform handParent;
    [SerializeField]
    private Card nextCard;
    [SerializeField]
    private bool onDragging;
    [SerializeField]
    private Transform unitTransform;

    public Transform UnitTransform
    {
        get { return unitTransform; }
        set { unitTransform = value; }
    }
    public bool OnDragging
    {
        get { return onDragging; }
        set { onDragging = value; }
    }
    public Deck PlayerDeck 
    {
        get { return playerDeck; }
    }
    public List<Image> Resources
    {
        get { return resources; }
    }
    public int Score
    {
        get { return score; }
        set { score = value; }
    }
    public Text TextCurrResource
    {
        get { return textCurrResource; }
        set { textCurrResource = value; }
    }
    public Text TextMaxResource
    {
        get { return textMaxResource; }
        set { textMaxResource = value; }
    }
    public Text TextScore
    {
        get { return textScore; }
        set { textScore = value; }
    }
    public float CurrResource
    {
        get
        {
            return currResource;
        }
        set
        {
            currResource = value;
        }
    }
    public int GetCurrResource
    {
        get
        {
            return (int)currResource;
        }
    }
    public GameObject FullRes;
    private GameObject ShipPlayer;
    public GameObject HandOnScreen;
    IEnumerable<RectTransform> allCards;
    IEnumerable<string> NameCards; 

    private void Start()
    {
        playerDeck.Start();
        ShipPlayer = GameObject.Find("Ship-Player-1");
    }

    private void Update()
    {
        if (GetCurrResource < GameConstants.RESOURCE_MAX +1)
        {
            resources[GetCurrResource].fillAmount = currResource - GetCurrResource;
            currResource += Time.deltaTime * GameConstants.RESOURCE_SPEED;
        }
        if (currResource >= GameConstants.RESOURCE_MAX)
        {
            FullRes.SetActive(true);
        }
        if (currResource < GameConstants.RESOURCE_MAX)
        {
            FullRes.SetActive(false);
        }
        allCards = HandOnScreen.GetComponentsInChildren<RectTransform>().Where(a => a.gameObject.tag == "Card");
        UpdateText();
        UpdateDeck();
    }

    public void RemoveResources(float cost, float target)
    {
        currResource -= cost;
        for (int i = 0; i < resources.Count; i++)
        {
            resources[i].fillAmount = 0;
            if (i <= GetCurrResource)
            {
                resources[i].fillAmount = 1;
            }
        }
        PlayerDeck.RemoveCard((int)target);
    }

    void UpdateText()
    {
        textCurrResource.text = GetCurrResource.ToString();
        textMaxResource.text = (GameConstants.RESOURCE_MAX + 1).ToString();
        textScore.text = score.ToString();
    }

    void UpdateDeck()
    {
        if (allCards!=null)
        {
            if (allCards.Count() < GameConstants.MAX_HAND_SIZE && !onDragging)
            {
                CardStats cs = playerDeck.DrawCard();
                GameObject go = Instantiate(cardPrefab, handParent);
                Card c = go.GetComponent<Card>();
                c.PlayerInfo = this;
                c.CardInfo = cs;
            }
        }

        //nextCard.CardInfo = playerDeck.NextCard;
        //nextCard.PlayerInfo = this;
        //Debug.Log(nextCard.Icon.sprite.name);
        CheckCardInHand();
    }

    public void CheckCardInHand()
    {
        GameObject lastCard = allCards.Last().gameObject;
        GameObject shipShield = GameObject.Find("Ship-Player-1");
        if (allCards.Count() >= 1)
        {
            for (int i = 0; i < allCards.Count()-1; i++)
            {
                if (allCards.ToArray()[i].gameObject.GetComponent<Card>().CardInfo.Icon.name == lastCard.gameObject.GetComponent<Card>().CardInfo.Icon.name
                    || (lastCard.gameObject.GetComponent<Card>().CardInfo.Icon.name == "shield" && shipShield.GetComponent<Ship>().shieldActive))
                {
                    int t = lastCard.gameObject.GetComponent<Card>().CardInfo.Index;
                    PlayerDeck.RemoveCard((int)t);
                    allCards.ToList().Remove(lastCard.GetComponent<RectTransform>());
                    Destroy(lastCard);
                    break;
                }
            }
        }
    }
}
