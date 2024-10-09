using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    CardBattleMode cardBattleMode;
    PlayerMovement playerMovement;

    public override void Death()
    {
        cardBattleMode.SetInputDisabled(true);
        playerMovement.DisableInput();
    }

    public void SetCardBattleMode(CardBattleMode value) => cardBattleMode = value;
    public void SetPlayerMovement(PlayerMovement value) => playerMovement = value;
}
