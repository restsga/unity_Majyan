using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beginner_Call : AI_Base
{
    public override int[] ActionDecision_Call(List<int> hand, List<int[]> call,int discard)
    {
        int[] callIndexes= CanPonKanIndexes(hand, discard);
        if (callIndexes.Length >= 4 - 1)
        {

        }
        if (callIndexes.Length >= 3 - 1)
        {
            return callIndexes.ToList().GetRange(0, 3 - 1).ToArray();
        }
        return new int[] { NULL };
    }

    public override int ActionDecision_Discard(List<int> hand, List<int[]> call)
    {
        return hand.Count - 1;
    }

    public override int[] ActionDecision_Draw(List<int> hand, List<int[]> call)
    {
        return new int[] { NULL };
    }
}