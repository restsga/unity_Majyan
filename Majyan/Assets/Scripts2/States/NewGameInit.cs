using System;

public class NewGameInit : State
{
    public NewGameInit()
    {
        timer = 0f;
    }

    protected override State MainFunction()
    {
        //各競技者の情報について初期化
        Array.ForEach<Player>(Main.players, player => player.Initialize_NewGame());

        return StateObjects.newRoundInit;
    }
}
