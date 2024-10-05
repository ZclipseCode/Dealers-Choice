using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBattleMode : MonoBehaviour
{
    [SerializeField] Deck deck;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform handTransform;
    List<Card> hand = new List<Card>();
    List<GameObject> cardObjects = new List<GameObject>();

    private void Start()
    {
        hand = new List<Card>(new Card[deck.GetHandSize()]);

        foreach(Card card in hand)
        {
            GameObject cardObject = Instantiate(cardPrefab, handTransform);
            cardObjects.Add(cardObject);
        }
    }

    public void StartBattle()
    {
        hand = deck.Draw();

        for (int i = 0; i < hand.Count; i++)
        {
            DisplayCard(cardObjects[i], hand[i]);
        }
    }

    void DisplayCard(GameObject cardObject, Card card)
    {
        cardObject.SetActive(true);
        SpriteRenderer spriteRenderer = cardObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.GetArt();
    }
}
