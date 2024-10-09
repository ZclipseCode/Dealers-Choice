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
    [SerializeField] SpriteRenderer spriteRenderer;

    public void SetInfo(Card c)
    {
        card = c;
        value = card.GetValue();
        suit = card.GetSuit();
        art = card.GetArt();
        spriteRenderer.sprite = art;
    }

    public void ActivateAbility(EnemyHealth enemyHealth)
    {
        enemyHealth.Damage(value);
    }

    public int GetValue() => value;
    public Suit GetSuit() => suit;
    public void SetSelected(bool value) => selected = value;
}
