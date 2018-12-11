using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDiscard : State
{
    public override State SetTimer()
    {
        timer = 1.5f;
        return this;
    }

    protected override State MainFunction()
    {
        //行動を取得
        int decision = Main.players[Player.turn].AIDiscard();
        int index = decision % AI_Base.CYCLE;
        int action = decision / AI_Base.CYCLE;

        //捨て牌をする
        if (action== AI_Base.DISCARD)
        {
            Main.players[Player.turn].Discard(index);

            return StateObjects.call();
        }
        else
        {
            return StateObjects.waitInput();
        }
    }
}
