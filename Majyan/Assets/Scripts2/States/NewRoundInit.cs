using System;

public class NewRoundInit : State
{
    public NewRoundInit()
    {
        timer = 0f;
    }

    protected override State MainFunction()
    {
        //牌山を初期化
        Main.deck.Initialize();

        //各競技者の情報について初期化
        Array.ForEach<Player>(Main.players, player => player.Initialize_NewRound());

        return StateObjects.deal;
    }
}
