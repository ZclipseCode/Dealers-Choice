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
    bool suitMode;
    bool valueMode;

    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCamera playerCamera;
    EnemyHealth enemyHealth;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.CardBattle.Enable();
        playerControls.CardBattle.Cycle.performed += ControllerCycled;
        playerControls.CardBattle.Select.performed += Selected;
        playerControls.CardBattle.Confirm.performed += Confirmed;
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
            Vector3 position = cardInfos[i].transform.localPosition;
            cardInfos[i].transform.localPosition = new Vector3(step * (i + .5f) - totalWidth / 2f, position.y, position.z);
        }

        originalCardSize = cardPrefab.transform.localScale;
        highlightedCardSize = cardPrefab.transform.localScale * 1.2f;

        cardInfos[highlightedCardIndex].transform.localScale = highlightedCardSize;

        originalY = cardInfos[highlightedCardIndex].transform.localPosition.y;
        selectedY = originalY + 0.05f;
    }

    private void Update()
    {
        if (!inputDisabled)
        {
            PlayerInput();
        }
    }

    public void StartBattle(EnemyHealth enemy)
    {
        enemyHealth = enemy;

        ResetHand();

        for (int i = 0; i < hand.Count; i++)
        {
            cardInfos[i].gameObject.SetActive(true);
        }

        inputDisabled = false;

        highlightedCardIndex = 0;
        cardInfos[highlightedCardIndex].transform.localScale = highlightedCardSize;
    }

    void PlayerInput()
    {
        switch (cardBattleState)
        {
            case CardBattleState.pickCards:
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
        if (!inputDisabled && cardBattleState.Equals(CardBattleState.pickCards))
        {
            CardInfo cardInfo = cardInfos[highlightedCardIndex];

            if (!selectedCards.Contains(cardInfo) && CanCombo(cardInfo))
            {
                SelectCard(cardInfo);
            }
            else if (selectedCards.Contains(cardInfo))
            {
                DeselectCard(cardInfo);
            }
        }
    }

    void SelectCard(CardInfo cardInfo)
    {
        cardInfo.SetSelected(true);
        selectedCards.Add(cardInfo);
        cardInfo.transform.localPosition = new Vector3(cardInfo.transform.localPosition.x, selectedY, cardInfo.transform.localPosition.z);
    }

    void DeselectCard(CardInfo cardInfo)
    {
        cardInfo.SetSelected(false);
        selectedCards.Remove(cardInfo);
        cardInfo.transform.localPosition = new Vector3(cardInfo.transform.localPosition.x, originalY, cardInfo.transform.localPosition.z);
    }

    bool CanCombo(CardInfo cardInfo)
    {
        if (selectedCards.Count == 1)
        {
            suitMode = selectedCards[0].GetSuit().Equals(cardInfo.GetSuit());
            valueMode = selectedCards[0].GetValue().Equals(cardInfo.GetValue());

            if (!suitMode && !valueMode)
            {
                return false;
            }
        }
        else if (selectedCards.Count > 1 && suitMode && !selectedCards[0].GetSuit().Equals(cardInfo.GetSuit()))
        {
            return false;
        }
        else if (selectedCards.Count > 1 && valueMode && !selectedCards[0].GetValue().Equals(cardInfo.GetValue()))
        {
            return false;
        }

        return true;
    }

    void ResetHand()
    {
        for (int i = selectedCards.Count - 1; i >= 0 ; i--)
        {
            DeselectCard(selectedCards[i]);
        }

        hand = deck.Draw();

        for (int i = 0; i < hand.Count; i++)
        {
            cardInfos[i].SetInfo(hand[i]);
        }
    }

    void Confirmed(InputAction.CallbackContext context)
    {
        if (!inputDisabled)
        {
            if (selectedCards.Count > 0)
            {
                foreach (CardInfo cardInfo in selectedCards)
                {
                    switch (cardInfo.GetSuit())
                    {
                        case Suit.spades:
                            cardInfo.ActivateAbility(enemyHealth);
                            break;
                        case Suit.clubs:
                            break;
                        case Suit.diamonds:
                            break;
                        case Suit.hearts:
                            break;
                    }
                }
            }

            deck.ReturnToDeck(hand);
            ResetHand();

            // combat box
        }
    }

    public void EndBattle()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            cardInfos[i].gameObject.SetActive(false);
        }

        inputDisabled = true;
        playerMovement.EnableInput();
        playerCamera.EnableInput();
    }

    public void SetInputDisabled(bool value) => inputDisabled = value;

    private void OnDestroy()
    {
        playerControls.CardBattle.Disable();
        playerControls.CardBattle.Cycle.performed -= ControllerCycled;
        playerControls.CardBattle.Select.performed -= Selected;
        playerControls.CardBattle.Confirm.performed -= Confirmed;
    }
}
