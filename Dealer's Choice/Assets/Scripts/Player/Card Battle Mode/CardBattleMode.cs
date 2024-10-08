using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardBattleMode : MonoBehaviour
{
    [SerializeField] Deck deck;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform handTransform;
    List<Card> hand = new List<Card>();
    List<GameObject> cardObjects = new List<GameObject>();

    PlayerControls playerControls;
    bool inputDisabled = true;
    CardBattleState cardBattleState = CardBattleState.pickCards;
    Vector3 originalCardSize;
    Vector3 highlightedCardSize;
    int highlightedCardIndex;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.CardBattle.Enable();
        playerControls.CardBattle.ControllerCycle.performed += ControllerCycled;
    }

    private void Start()
    {
        hand = new List<Card>(new Card[deck.GetHandSize()]);

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, handTransform);
            cardObjects.Add(cardObject);
        }

        float cardWidth = cardPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float spacing = 0.025f;
        float totalWidth = (cardWidth + spacing) * hand.Count;
        float step = totalWidth / hand.Count;

        for (int i = 0; i < hand.Count; i++)
        {
            Vector3 position = cardObjects[i].transform.position;
            cardObjects[i].transform.position = new Vector3(step * (i + .5f) - totalWidth / 2, position.y, position.z);
        }

        originalCardSize = cardPrefab.transform.localScale;
        highlightedCardSize = cardPrefab.transform.localScale * 1.2f;

        cardObjects[highlightedCardIndex].transform.localScale = highlightedCardSize;
    }

    private void Update()
    {
        if (!inputDisabled)
        {
            PlayerInput();
        }
    }

    public void StartBattle()
    {
        hand = deck.Draw();

        for (int i = 0; i < hand.Count; i++)
        {
            DisplayCard(cardObjects[i], hand[i]);
        }

        inputDisabled = false;

        highlightedCardIndex = 0;
        cardObjects[highlightedCardIndex].transform.localScale = highlightedCardSize;
    }

    void DisplayCard(GameObject cardObject, Card card)
    {
        cardObject.SetActive(true);
        SpriteRenderer spriteRenderer = cardObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.GetArt();
    }

    void PlayerInput()
    {
        switch (cardBattleState)
        {
            case CardBattleState.pickCards:
                // refer to ControllerCycled() for controller
                float mouseScroll = Mouse.current.scroll.ReadValue().normalized.y;
                CycleCards((int)mouseScroll);
                break;
            case CardBattleState.defend:
                break;
        }
    }

    void ControllerCycled(InputAction.CallbackContext context)
    {
        CycleCards((int)context.ReadValue<float>());
    }

    void CycleCards(int value)
    {
        cardObjects[highlightedCardIndex].transform.localScale = originalCardSize;

        if (highlightedCardIndex + value >= hand.Count)
        {
            highlightedCardIndex = 0;
        }
        else if (highlightedCardIndex + value < 0)
        {
            highlightedCardIndex = hand.Count - 1;
        }
        else
        {
            highlightedCardIndex += value;
        }

        cardObjects[highlightedCardIndex].transform.localScale = highlightedCardSize;
    }

    private void OnDestroy()
    {
        playerControls.CardBattle.Disable();
        playerControls.CardBattle.ControllerCycle.performed += ControllerCycled;
    }
}
