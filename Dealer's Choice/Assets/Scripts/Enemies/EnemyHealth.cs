using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    CardBattleMode cardBattleMode;

    public override void Death()
    {
        cardBattleMode.EndBattle();
    }

    public void SetCardBattleMode(CardBattleMode value) => cardBattleMode = value;
}
