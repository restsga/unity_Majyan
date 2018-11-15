using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Discard:AI {

    public Draw_Discard()
    {

    }

    public override void DecideDiscard(List<int> hand)
    {
        discard= hand.Count - 1;
    }

    public override bool DecidePon(List<int> hand, ref bool bonus)
    {
        bonus = true;
        return true;
    }

    public override bool DecideKan(List<int> hand)
    {
        return true;
    }

    public override bool DecideTi(List<int> hand, ref int[] indexes)
    {
        return true;
    }
}
