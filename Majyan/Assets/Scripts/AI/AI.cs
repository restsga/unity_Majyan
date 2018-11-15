using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AI {

    protected int discard;

    public abstract void DecideDiscard(List<int> hand);
    public abstract bool DecidePon(List<int> hand,ref bool bonus);
    public abstract bool DecideTi(List<int> hand, ref int[] indexes);
    public abstract bool DecideKan(List<int> hand);

    public int GetDiscord()
    {
        return discard;
    }
}
