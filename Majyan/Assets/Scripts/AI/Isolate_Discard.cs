using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Isolate_Discard : AI
{
    public Isolate_Discard()
    {
        callRiichi = true;
    }

    public override int DecideCallKanOrPon(List<int> hand, List<CallCardsSet> call, int discard)
    {
        /*List<int> ponOrKanIndexes = CanCallKanOrPon(hand, discard);

        if (ponOrKanIndexes.Count >= 3 - 1)
        {
            call_index = ponOrKanIndexes[0];

            if (ponOrKanIndexes.Count >= 4 - 1)
            {
                return OPEN_KAN;
            }
            else
            {
                discard_index = hand.Count - 1 - 2;
                useBonusCard_forPon = true;
                return PON;
            }
        }*/

        return NOT_CALL;
    }

    public override int DecideDiscardOrKan(List<int> hand, List<CallCardsSet> call)
    {
        List<int> closedKanIndexes = CanClosedKan(hand);
        if (closedKanIndexes.Count >= 1)
        {
            call_index = closedKanIndexes[0];
            return CLOSED_KAN;
        }

        /*List<int> addKanIndexes = CanAddKan(hand, call);
        if (addKanIndexes.Count >= 1)
        {
            call_index = addKanIndexes[0];
            return ADD_KAN;
        }*/

        int[] cardCounts = new int[34];
        ArrayBase.ResetArray(cardCounts, 0);
        for (int i = 0; i < hand.Count; i++)
        {
            cardCounts[hand[i] / 2]++;
        }

        bool isolate = false;
        discard_index = hand.Count - 1;
        for (int i = 0; i < hand.Count; i++)
        {
            if (Enable(cardCounts, hand[i] / 2)==false)
            {
                if (isolate == false)
                {
                    discard_index = i;
                    isolate = true;
                }
                else
                {
                    if (hand[i] / 2 >= 3 * 9)
                    {
                        discard_index = i;
                    }
                    else if (Math.Abs((hand[i] / 2) % 9 - (5 - 1)) >= Math.Abs((hand[discard_index] / 2) % 9 - (5 - 1)))
                    {
                        if (hand[discard_index] / 2 < 3 * 9)
                        {
                            discard_index = i;
                        }
                    }
                }
            }
        }

        return DISCARD;
    }

    public override int DecideDrawCardOrTi(List<int> hand, List<CallCardsSet> call, int discard)
    {
        /*List<int[]> indexSets = CanCallTi(hand, discard);

        if (indexSets.Count >= 1)
        {
            call_index = indexSets[0][0];
            call_index2_forTi = indexSets[0][1];

            discard_index = hand.Count - 1 - 2;
            return TI;
        }*/

        return DRAW_CARD;
    }

    public override bool DecideWin_SelfDraw(List<int> hand,List<CallCardsSet> call)
    {
        return true;
    }

    public override bool DecideWin_OnDiscard(List<int> hand, List<CallCardsSet> call)
    {
        return true;
    }
}
