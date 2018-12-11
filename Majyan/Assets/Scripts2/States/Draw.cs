using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : State
{
    public override State SetTimer()
    {
        timer = 0f;
        return this;
    }

    protected override State MainFunction()
    {
        //牌山から牌を引く
        Main.players[Player.turn].DrawCard(Main.deck.DrawCard(false));

        if (Player.IsPlayingUserTurn())
        {
            return StateObjects.waitInput();
        }
        else
        {
            return StateObjects.aiDiscard();
        }
    }
}
