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

    List<string> NameCard = new List<string>();

    public CardStats DrawCard()
    {
        CardStats cs = nextCard;

        //foreach (var item in hand)
        //{
        //    GameObject shipShield = GameObject.Find("Ship-Player-1");

        //    if (nextCard.Icon.texture.name == "shield" && shipShield.GetComponent<Ship>().shieldActive)
        //    {
        //        if (shipShield != null)
        //        {
        //            cards.Remove(nextCard);
        //            nextCard = cards[0];
        //        }
        //    }
        //}
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
        //Debug.Log(NameCard.Contains(nextCard));
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
