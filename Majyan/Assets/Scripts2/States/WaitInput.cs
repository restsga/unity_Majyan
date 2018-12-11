using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitInput : State
{
    private int discardIndex=Main.NULL;
    private bool canDrawTi = false;
    private bool draw = false;

    public override State SetTimer()
    {
        timer = 0f;
        return this;
    }

    protected override State MainFunction()
    {
        if (discardIndex != Main.NULL)
        {
            Main.players[0].Discard(discardIndex);
            discardIndex = Main.NULL;
            return StateObjects.call();
        }

        if (draw)
        {
            draw = false;
            return StateObjects.draw();
        }

        return this;
    }

    public void Discard(int index)
    {
        if (Main.players[0].CanPLDiscard())
        {
            discardIndex = index;
        }
    }

    public void CanDrawTi()
    {
        canDrawTi = true;
    }

    public void Draw()
    {
        if (canDrawTi)
        {
            draw = true;
            canDrawTi = false;
        }
    }
}
