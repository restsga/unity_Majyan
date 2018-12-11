using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Call : State
{
    public override State SetTimer()
    {
        timer = 1.5f;
        return this;
    }

    protected override State MainFunction()
    {
        int[] indexes=new int[1];
        Player callPlayer= Main.players.ToList().
            FirstOrDefault(player => (indexes=player.AICall()).Length >= 3 - 1&&player.IsThisPlayerTurn()==false);
        if (callPlayer != null)
        {
            Main.players[Player.turn].Called();
            callPlayer.Call(indexes);
            return StateObjects.aiDiscard();
        }

        Player.NextTurn();
        if (Player.IsPlayingUserTurn())
        {
            StateObjects.waitInput_object.CanDrawTi();
            return StateObjects.waitInput();
        }
        else
        {
            return StateObjects.aiDraw(); 
        }
    }
}
