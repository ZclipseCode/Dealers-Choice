using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    Card card;
    int value;
    Suit suit;
    Sprite art;
    bool selected;

    public void SetInfo(Card c)
    {
        card = c;
        value = card.GetValue();
        suit = card.GetSuit();
        art = card.GetArt();
    }

    public void SetSelected(bool value) => selected = value;
}
