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
    List<CardInfo> cardInfos = new List<CardInfo>();

    PlayerControls playerControls;
    bool inputDisabled = true;
    CardBattleState cardBattleState = CardBattleState.pickCards;
    Vector3 originalCardSize;
    Vector3 highlightedCardSize;
    int highlightedCardIndex;
    List<CardInfo> selectedCards = new List<CardInfo>();
    float originalY;
    float selectedY;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.CardBattle.Enable();
        playerControls.CardBattle.ControllerCycle.performed += ControllerCycled;
        playerControls.CardBattle.Select.performed += Selected;
    }

    private void Start()
    {
        hand = new List<Card>(new Card[deck.GetHandSize()]);

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, handTransform);
            CardInfo cardInfo = cardObject.GetComponent<CardInfo>();
            cardInfos.Add(cardInfo);
        }

        float cardWidth = cardPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float spacing = 0.025f;
        float totalWidth = (cardWidth + spacing) * hand.Count;
        float step = totalWidth / hand.Count;

        for (int i = 0; i < hand.Count; i++)
        {
            Vector3 position = cardInfos[i].transform.position;
            cardInfos[i].transform.position = new Vector3(step * (i + .5f) - totalWidth / 2, position.y, position.z);
        }

        originalCardSize = cardPrefab.transform.localScale;
        highlightedCardSize = cardPrefab.transform.localScale * 1.2f;

        cardInfos[highlightedCardIndex].transform.localScale = highlightedCardSize;

        originalY = cardInfos[highlightedCardIndex].transform.position.y;
        selectedY = originalY + 0.05f;
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
            DisplayCard(cardInfos[i], hand[i]);
        }

        inputDisabled = false;

        highlightedCardIndex = 0;
        cardInfos[highlightedCardIndex].transform.localScale = highlightedCardSize;
    }

    void DisplayCard(CardInfo cardInfo, Card card)
    {
        GameObject cardObject = cardInfo.gameObject;
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
        if (!inputDisabled && cardBattleState == CardBattleState.pickCards)
        {
            CycleCards((int)context.ReadValue<float>());
        }
    }

    void CycleCards(int value)
    {
        cardInfos[highlightedCardIndex].transform.localScale = originalCardSize;

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

        cardInfos[highlightedCardIndex].transform.localScale = highlightedCardSize;
    }

    void Selected(InputAction.CallbackContext context)
    {
        if (!inputDisabled && cardBattleState == CardBattleState.pickCards)
        {
            CardInfo cardInfo = cardInfos[highlightedCardIndex];

            if (!selectedCards.Contains(cardInfo))
            {
                cardInfo.SetSelected(true);
                selectedCards.Add(cardInfo);
                cardInfo.transform.position = new Vector3(cardInfo.transform.position.x, selectedY, cardInfo.transform.position.z);
            }
            else
            {
                cardInfo.SetSelected(false);
                selectedCards.Remove(cardInfo);
                cardInfo.transform.position = new Vector3(cardInfo.transform.position.x, originalY, cardInfo.transform.position.z);
            }
        }
    }

    private void OnDestroy()
    {
        playerControls.CardBattle.Disable();
        playerControls.CardBattle.ControllerCycle.performed -= ControllerCycled;
        playerControls.CardBattle.Select.performed -= Selected;
    }
}
