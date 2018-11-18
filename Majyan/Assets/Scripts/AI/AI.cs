using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AI {

    protected int discard_index;
    protected int kan_cardIdOrCallIndex;



    public abstract void DecideDiscard(List<int> hand);
    public abstract bool DecidePon(List<int> hand,ref bool bonus);
    public abstract bool DecideTi(List<int> hand, ref int[] indexes,int discard_other);
    public abstract bool DecideOpenKan(List<int> hand);
    public abstract bool DecideClosedKan(List<int> hand);
    public abstract bool DecideAddKan(List<int> hand, List<CallCardsSet> call);

    public int GetDiscardIndex()
    {
        return discard_index;
    }

    public int GetKanCardId()
    {
        return kan_cardIdOrCallIndex;
    }
}
