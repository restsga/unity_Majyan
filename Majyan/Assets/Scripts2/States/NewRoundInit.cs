using System;

public class NewRoundInit : State
{
    public override State SetTimer()
    {
        timer = 0f;
        return this;
    }

    protected override State MainFunction()
    {
        //牌山を初期化
        Main.deck.Initialize();

        //各競技者の情報について初期化
        Array.ForEach<Player>(Main.players, player => player.Initialize_NewRound());

        return StateObjects.deal();
    }
}
