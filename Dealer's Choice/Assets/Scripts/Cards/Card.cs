using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card")]
public class Card : ScriptableObject
{
    [SerializeField] int value;
    [SerializeField] Suit suit;
    [SerializeField] Sprite art;

    public int GetValue() => value;
    public Suit GetSuit() => suit;
    public Sprite GetArt() => art;
}
