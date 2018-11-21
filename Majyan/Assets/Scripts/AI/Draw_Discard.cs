using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Discard:AI {

    public Draw_Discard()
    {

    }
    
    public override int DecideDiscardOrKan(List<int> hand,List<CallCardsSet> call)
    {
        List<int> closedKanIndexes = CanClosedKan(hand);
        if (closedKanIndexes.Count>=1)
        {
            call_index = closedKanIndexes[0];
            return CLOSED_KAN;
        }

        List<int> addKanIndexes = CanAddKan(hand, call);
        if (addKanIndexes.Count >= 1)
        {
            call_index = addKanIndexes[0];
            return ADD_KAN;
        }

        discard_index = hand.Count - 1;
        return DISCARD;
    }

    public override int DecideCallKanOrPon(List<int> hand,int discard)
    {
        List<int> ponOrKanIndexes = CanCallKanOrPon(hand, discard);

        if (ponOrKanIndexes.Count >= 3 - 1)
        {
            call_index = ponOrKanIndexes[0];

            if (ponOrKanIndexes.Count >= 4-1)
            {
                return OPEN_KAN;
            }
            else
            {
                discard_index = hand.Count - 1-2;
                useBonusCard_forPon = true;
                return PON;
            }
        }

        return NOT_CALL;
    }

    public override int DecideDrawCardOrTi(List<int> hand,int discard)
    {
        List<int[]> indexSets = CanCallTi(hand, discard);

        if (indexSets.Count >= 1)
        {
            call_index = indexSets[0][0];
            call_index2_forTi= indexSets[0][1];

            discard_index = hand.Count - 1-2;
            return TI;
        }

        return DRAW_CARD;
    }
}
