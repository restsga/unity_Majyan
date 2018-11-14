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
}
