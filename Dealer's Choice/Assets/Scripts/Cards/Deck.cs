using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] List<Card> cards = new List<Card>();
    [SerializeField] int handSize = 5;

    public List<Card> Draw()
    {
        List<Card> hand = new List<Card>();

        for (int i = 0; i < handSize; i++)
        {
            int cardIndex = Random.Range(0, cards.Count);
            Card drawnCard = cards[cardIndex];

            cards.Remove(drawnCard);

            hand.Add(drawnCard);
        }

        return hand;
    }

    public int GetHandSize() => handSize;
}
