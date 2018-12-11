using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDraw : State
{
    public override State SetTimer()
    {
        timer = 0f;
        return this;
    }

    protected override State MainFunction()
    {
        int[] indexes = Main.players[Player.turn].AIDraw();
        if (indexes.Length <= 1)
        {
            return StateObjects.draw();
        }

        return StateObjects.waitInput();
    }
}
