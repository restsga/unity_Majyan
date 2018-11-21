using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AI {

    static public readonly int 
        DRAW_CARD = 1, TI = 2, DISCARD = 3, CLOSED_KAN = 4, ADD_KAN = 5, OPEN_KAN = 6, PON = 7,
        NOT_CALL=10,
        WAIT_INPUT=11;

    protected int discard_index;
    protected int call_index;
    protected int call_index2_forTi;
    protected bool useBonusCard_forPon;

    public abstract int DecideDiscardOrKan(List<int> hand, List<CallCardsSet> call);
    public abstract int DecideCallKanOrPon(List<int> hand, int discard);
    public abstract int DecideDrawCardOrTi(List<int> hand, int discard);

    public int GetDiscardIndex()
    {
        return discard_index;
    }
    public int GetCallIndex()
    {
        return call_index;
    }
    public int[] GetCallIndexesForTi()
    {
        int[] indexes = { call_index, call_index2_forTi };
        return indexes;
    }
    public bool GetUseBonusCardForPon()
    {
        return useBonusCard_forPon;
    }

    protected List<int> CanClosedKan(List<int> hand)
    {
        List<int> indexes = new List<int>();
        indexes.Clear();

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
            if (Same(sortedHand[i - 1], sortedHand[i]))
            {
                chain++;

                if (chain >= 4)
                {
                    indexes.Add(hand.FindIndex((card) => card == sortedHand[i]));

                    chain = 1;
                }
            }
            else
            {
                chain = 1;
            }
        }

        return indexes;
    }

    protected List<int> CanAddKan(List<int> hand, List<CallCardsSet> call)
    {
        List<int> indexes = new List<int>();
        indexes.Clear();

        for (int s = 0; s < call.Count; s++)
        {
            if (call[s].IsThisPon())
            {
                for (int i = 0; i < hand.Count; i++)
                {
                    if (Same(hand[i], call[s].callCards[0].cardId))
                    {
                        indexes.Add(i);
                    }
                }
            }
        }

        return indexes;
    }

    static public List<int> CanCallKanOrPon(List<int> hand,int discard)
    {
        List<int> indexes = new List<int>();

        for (int i = 0; i < hand.Count; i++)
        {
            if (Same(hand[i], discard))
            {
                indexes.Add(i);
            }
        }

        return indexes;
    }

    static public List<int[]> CanCallTi(List<int> hand, int discard)
    {
        List<int> sameGroupCards = new List<int>();
        int[,] numbers = { { -2, -1 }, { -1, 1 }, { 1, 2 } };   //チーのパターン

        //捨て牌と同じグループの牌のみを抽出
        for (int i = 0; i < hand.Count; i++)
        {
            if (SameGroup(hand[i], discard))
            {
                sameGroupCards.Add(hand[i]);
            }
        }
        sameGroupCards.Sort();  //並べ替え

        List<int[]> indexSets = new List<int[]>();
        for (int c = 0; c < 3; c++)
        {
            int[] handIndexes = new int[2];
            int check = 0;
            int bonus = GameManagerScript.NULL_ID;

            for (int i = 0; i < sameGroupCards.Count; i++)
            {
                if (Same(sameGroupCards[i], discard + numbers[c, check] * 2))
                {
                    handIndexes[check] = hand.FindIndex((card) =>card== sameGroupCards[i]);
                    if (Bonus5(sameGroupCards[i]))
                    {
                        bonus = check;
                    }

                    check++;

                    if (check >= 2)
                    {
                        indexSets.Add(handIndexes);

                        if (bonus >= 0)
                        {
                            handIndexes[bonus]++;
                            indexSets.Add(handIndexes);
                        }

                        break;
                    }
                }
            }
        }

        return indexSets;
    }

    static public bool IsThisSetCanTi(List<int> hand, int discard, int[] indexes)
    {
        List<int[]> canCallIndexes = CanCallTi(hand, discard);

        for(int i = 0; i < indexes.Length; i++)
        {
            if (indexes[i] < 0)
            {
                return false;
            }
        }

        for (int s = 0; s < canCallIndexes.Count; s++)
        {
            int count = 0;
            for (int i = 0; i < indexes.Length; i++)
            {
                if (Same(hand[canCallIndexes[s][i]], hand[indexes[0]])|| Same(hand[canCallIndexes[s][i]], hand[indexes[1]]))
                {
                    count++;

                    if (count >= 2)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static public  bool Same(int cardA,int cardB)
    {
        return cardA / 2 == cardB / 2;
    }

    static public bool IsNumberCard(int card)
    {
        return card / 2 < 9 * 3;
    }

    static public bool SameGroup(int cardA, int cardB)
    {
        if (IsNumberCard(cardA) && IsNumberCard(cardB))
        {
            return cardA / 2 / 9 == cardB / 2 / 9;
        }
        else
        {
            return false;
        }
    }

    static public bool Bonus5(int card)
    {
        if (IsNumberCard(card))
        {
            return card / 2 / 9 == 5 - 1 && card % 2 == 1;
        }
        else
        {
            return false;
        }
    }
}