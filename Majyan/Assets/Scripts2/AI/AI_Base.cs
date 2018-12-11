using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AI_Base
{
    //行動毎のindex補正値
    static public readonly int DISCARD = 0, CLOSEDKAN = 1, ADDKAN = 2,CYCLE=20;

    //null用の値
    protected readonly int NULL = -9999;

    public abstract int[] ActionDecision_Draw(List<int> hand, List<int[]> call);
    public abstract int ActionDecision_Discard(List<int> hand,List<int[]> call);
    public abstract int[] ActionDecision_Call(List<int> hand, List<int[]> call,int discard);

    static public int[] CanPonKanIndexes(List<int> hand,int discard)
    {
        return Enumerable.Range(0, hand.Count).Where(i => CardId.Same(hand[i], discard)).ToArray();
    }
}
