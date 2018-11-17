using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Discard:AI {

    public Draw_Discard()
    {

    }

    public override void DecideDiscard(List<int> hand)
    {
        discard_index= hand.Count - 1;
    }

    public override bool DecidePon(List<int> hand, ref bool bonus)
    {
        bonus = true;
        return true;
    }

    public override bool DecideTi(List<int> hand, ref int[] indexes,int discard_other)
    {
        List<int> sameGroupCards = new List<int>();
        List<int> indexesOfHand = new List<int>();
        int[,] numbers = { { -2, -1 }, { -1, 1 }, { 1, 2 } };

        if (Rules.IdChangeSerialToCardImageId(discard_other) <= 29)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                if (Rules.SameGroup(hand[i], discard_other))
                {
                    sameGroupCards.Add(hand[i]);
                    indexesOfHand.Add(i);
                }
            }
            sameGroupCards.Sort();
            for (int c = 0; c < 3; c++)
            {
                int check = 0;
                for (int i = 0; i < sameGroupCards.Count; i++)
                {
                    if (Rules.Same_BonusEquate(sameGroupCards[i], discard_other + numbers[c, check] * 4))
                    {
                        indexes[check] = indexesOfHand[i];

                        check++;
                        if (check >= 2)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return true;
    }

    public override bool DecideOpenKan(List<int> hand)
    {
        return true;
    }

    public override bool DecideClosedKan(List<int> hand)
    {
        int[] sortedHand = new int[hand.Count];     //元のリストを変更しないためのコピー

        //リストから編集用の配列に情報を複製
        for (int i = 0; i < hand.Count; i++)
        {
            sortedHand[i] = hand[i];
        }

        Array.Sort(sortedHand);     //並べ替え

        int chain = 1;

        //4枚以上同じ牌が連続するか判定
        for (int i = 1; i < sortedHand.Length; i++)
        {
            if (Rules.Same_BonusEquate(sortedHand[i - 1], sortedHand[i]))
            {
                chain++;

                if (chain >= 4)
                {
                    kan_cardIdOrCallIndex = sortedHand[i];
                    break;
                }
            }
            else
            {
                chain = 1;
            }
        }

        return true;
    }

    public override bool DecideAddKan(List<int> hand, List<CallCardsSet> call)
    {
        List<int> ponCards = new List<int>();

        for (int s = 0; s < call.Count; s++)
        {
            if (Rules.CallCardKinds(call[s]) == Rules.PON)
            {
                for (int i = 0; i < hand.Count; i++)
                {
                    if (Rules.Same_BonusEquate(hand[i], call[s].callCards[0].card))
                    {
                        kan_cardIdOrCallIndex = s;

                        return true;
                    }
                }
            }
        }

        return true;
    }
}
