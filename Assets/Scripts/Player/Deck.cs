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

    public CardStats DrawCard()
    {
        CardStats cs = nextCard;
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

    public void RemoveShield(CardStats cs)
    {
        hand.Remove(cs);
    }
}
