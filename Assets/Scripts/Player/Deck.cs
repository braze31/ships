using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Deck
{
    [SerializeField]
    private List<CardStats> cards;

    [SerializeField]
    private List<CardStats> hand;

    [SerializeField]
    private CardStats nextCard;

    public List<CardStats> Cards
    {
        get { return cards; }
    }

    public List<CardStats> Hand
    {
        get { return hand; }
    }

    public CardStats NextCard
    {
        get { return nextCard; }
    }

    public void Start()
    {
        nextCard = cards[0];
    }


    public CardStats DrawCard(IEnumerable<RectTransform> listCardInHand)
    {
        CardStats cs = nextCard;
        List<string> listHandStringCards = new List<string>();
        foreach (var item in listCardInHand)
        {
            listHandStringCards.Add(item.gameObject.GetComponent<Card>().CardInfo.Icon.name);
        }
        GameObject shipShield = GameObject.Find("Ship-Player-1");
        while (listHandStringCards.Contains(cs.Icon.name))
        {
            var item = cards[0];
            cards.Remove(nextCard);
            nextCard = cards[0];
            cs = nextCard;
            //Debug.Log(nextCard.Icon.texture.name == "shield");
            //Debug.Log(shipShield.GetComponent<Ship>().shieldActive);
            //Debug.Log(nextCard.Icon.texture.name);
            // add in end list cards idknow why list length same.
            cards.Add(item);
        }

        if (nextCard.Icon.texture.name == "rocket")
        {
            nextCard.Cost = 5;
        }
        if (nextCard.Icon.texture.name == "laser")
        {
            nextCard.Cost = 4;
        }
        if (nextCard.Icon.texture.name == "shield")
        {
            nextCard.Cost = 3;
        }
        if (nextCard.Icon.texture.name == "bomb")
        {
            nextCard.Cost = 6;
        }
        if (nextCard.Icon.texture.name == "flare")
        {
            nextCard.Cost = 2;
        }
        if (nextCard.Icon.texture.name == "def-ship")
        {
            nextCard.Cost = 4;
        }

        hand.Add(nextCard);
        cards.Remove(nextCard);
        nextCard = cards[0];

        return cs;
    }

    public void RemoveCard(int index)
    {
        foreach (CardStats cs in hand)
        {
            if (cs.Index == index)
            {
                hand.Remove(cs);
                cards.Add(cs);
                break;
            }
        }
    }
}
