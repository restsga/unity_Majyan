using System;

public class Deal : State
{
    public Deal()
    {
        timer = 1f;
    }

    protected override State MainFunction()
    {
        //各プレイヤーに対して配牌を行う
        Array.ForEach(Main.players, player => player.Deal(Main.deck.Deal()));

        return StateObjects.waitInput;
    }
}
