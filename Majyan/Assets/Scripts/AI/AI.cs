using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AI
{

    static public readonly int
        DRAW_CARD = 1, TI = 2, DISCARD = 3, CLOSED_KAN = 4, ADD_KAN = 5, OPEN_KAN = 6, PON = 7,RIICHI=8,WIN_SELF_DRAW=9,
        WIN_ON_DISCARD=10,NOT_CALL = 20,
        WAIT_INPUT = 21,
        DRAWN_GAME = 31;

    protected int discard_index;
    protected int call_index;
    protected int call_index2_forTi;
    protected bool useBonusCard_forPon;
    protected bool callRiichi;

    public abstract int DecideDiscardOrKan(List<int> hand, List<CallCardsSet> call);
    public abstract int DecideCallKanOrPon(List<int> hand, List<CallCardsSet> call, int discard);
    public abstract int DecideDrawCardOrTi(List<int> hand, List<CallCardsSet> call, int discard);
    public abstract bool DecideWin_SelfDraw(List<int> hand, List<CallCardsSet> call);
    public abstract bool DecideWin_OnDiscard(List<int> hand, List<CallCardsSet> call);

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
    public bool GetCallRiichi()
    {
        return callRiichi;
    }

    static public List<int> CanClosedKan(List<int> hand)
    {
        List<int> indexes = new List<int>();
        indexes.Clear();

        int[] sortedHand = ArrayBase.CopyForEdit(hand.ToArray());     //元のリストを変更しないためのコピー
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

    static public List<int> CanAddKan(List<int> hand, List<CallCardsSet> call)
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

    static public List<int> CanCallKanOrPon(List<int> hand, int discard)
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
        List<int[]> indexSets = new List<int[]>();
        indexSets.Clear();

        if (discard / 2 < 9 * 3)
        {
            List<int> sameGroupCards = new List<int>();
            int[,] numbers = { { -2, -1 }, { -1, 1 }, { 1, 2 } };   //チーのパターン

            sameGroupCards = Grouping_sorted(hand)[discard / 2 / 9];

            sameGroupCards.Sort();  //並べ替え

            for (int c = 0; c < 3; c++)
            {
                int[] handIndexes = new int[2];
                int check = 0;
                int bonus = GameManagerScript.NULL_ID;

                for (int i = 0; i < sameGroupCards.Count; i++)
                {
                    if (Same(sameGroupCards[i], discard + numbers[c, check] * 2))
                    {
                        handIndexes[check] = hand.FindIndex((card) => card == sameGroupCards[i]);
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
        }

        return indexSets;
    }

    static public bool IsThisSetCanTi(List<int> hand, int discard, int[] indexes)
    {
        List<int[]> canCallIndexes = CanCallTi(hand, discard);

        for (int i = 0; i < indexes.Length; i++)
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
                if (Same(hand[canCallIndexes[s][i]], hand[indexes[0]]) || Same(hand[canCallIndexes[s][i]], hand[indexes[1]]))
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

    static public bool Same(int cardA, int cardB)
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

    static private List<int>[] Grouping_sorted(List<int> hand)
    {
        List<int>[] groupAndSorted = new List<int>[4];
        for (int i = 0; i < groupAndSorted.Length; i++)
        {
            groupAndSorted[i] = new List<int>();
        }

        for (int i = 0; i < hand.Count; i++)
        {
            groupAndSorted[hand[i] / 2 / 9].Add(hand[i]);
        }

        for (int i = 0; i < groupAndSorted.Length; i++)
        {
            groupAndSorted[i].Sort();
        }

        return groupAndSorted;
    }

    static public bool[] ReadyAndWaiting(List<int> hand,ref bool ready)
    {
        //待ち牌
        bool[] waitingCards = new bool[34];
        for (int i = 0; i < waitingCards.Length; i++)
        {
            waitingCards[i] = false;
        }

        //聴牌フラグ
        ready = false;

        //各牌の枚数
        int[] cardCounts_row = CardCount(hand);
        int[] cardCounts = ArrayBase.CopyForEdit(cardCounts_row);

        for (int h = 0; h < cardCounts.Length; h++)
        {
            if (cardCounts[h] >= 2)
            {
                for (int w = 0; w < cardCounts.Length; w++)
                {
                    //雀頭(対子)決定
                    cardCounts[h] -= 2;

                    if (cardCounts_row[w] < 4)
                    {
                        if (Enable(cardCounts, w))
                        {
                            //待ち牌決定
                            cardCounts[w]++;

                            if (Can4Sets(cardCounts))
                            {
                                waitingCards[w] = true;
                                ready = true;
                            }
                        }
                    }

                    cardCounts = ArrayBase.CopyForEdit(cardCounts_row);
                }
            }

            if (cardCounts[h] == 1)
            {
                //雀頭(単騎待ち)決定
                cardCounts[h] -= 1;

                if (Can4Sets(cardCounts))
                {
                    waitingCards[h] = true;
                    ready = true;
                }

                cardCounts = ArrayBase.CopyForEdit(cardCounts_row);
            }
        }

        return waitingCards;
    }

    static public int[] CardCount(List<int> hand)
    {
        //各牌の枚数
        int[] cardCounts = new int[34];
        ArrayBase.ResetArray(cardCounts, 0);
        for (int i = 0; i < hand.Count; i++)
        {
            cardCounts[hand[i] / 2]++;
        }

        return cardCounts;
    }

    static protected bool Enable(int[] cardCounts, int card)
    {
        if (cardCounts[card] >= 3 - 1)
        {
            return true;
        }

        if (card < 9 * 3)
        {
            switch (card % 9)
            {
                case 0:
                    if (cardCounts[card + 1] >= 1 && cardCounts[card + 2] >= 1)
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (cardCounts[card - 1] >= 1 && cardCounts[card + 1] >= 1)
                    {
                        return true;
                    }
                    if (cardCounts[card + 1] >= 1 && cardCounts[card + 2] >= 1)
                    {
                        return true;
                    }
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    if (cardCounts[card - 2] >= 1 && cardCounts[card - 1] >= 1)
                    {
                        return true;
                    }
                    if (cardCounts[card - 1] >= 1 && cardCounts[card + 1] >= 1)
                    {
                        return true;
                    }
                    if (cardCounts[card + 1] >= 1 && cardCounts[card + 2] >= 1)
                    {
                        return true;
                    }
                    break;
                case 7:
                    if (cardCounts[card - 2] >= 1 && cardCounts[card - 1] >= 1)
                    {
                        return true;
                    }
                    if (cardCounts[card - 1] >= 1 && cardCounts[card + 1] >= 1)
                    {
                        return true;
                    }
                    break;
                case 8:
                    if (cardCounts[card - 2] >= 1 && cardCounts[card - 1] >= 1)
                    {
                        return true;
                    }
                    break;
            }
        }

        return false;
    }

    static private bool Can4Sets(int[] cardCounts)
    {
        for (int i = 0; i < cardCounts.Length; i++)
        {
            if (cardCounts[i] != 0)
            {
                if (cardCounts[i] < 0)
                {
                    break;
                }

                if (cardCounts[i] >= 3)
                {
                    cardCounts[i] -= 3;
                }

                if (0 < cardCounts[i] && cardCounts[i] <= 2)
                {
                    if (i >= 9 * 3)
                    {
                        break;
                    }

                    if (i % 9 >= 8 - 1)
                    {
                        break;
                    }
                    cardCounts[i + 1] -= cardCounts[i];
                    cardCounts[i + 2] -= cardCounts[i];
                    cardCounts[i] = 0;
                }
            }
        }

        if ((Array.FindIndex<int>(cardCounts, count => count != 0) >= 0) == false)
        {
            return true;
        }

        return false;
    }

    /*
    static private int[] CopyCardCountsForEdit(int[] cardCounts)
    {
        int[] copy = new int[cardCounts.Length];
        for(int i = 0; i < cardCounts.Length; i++)
        {
            copy[i] = cardCounts[i];
        }
        return copy;
    }
    
    static public int Syanten_01(List<int> hand)
    {
        int isolate = 0;
        int[,] group_count = Group_RemoveIsolate(hand,ref isolate);
            Syanten_0(group_count, isolate);

        return -1;
    }
    
    static private int[,] Group_RemoveIsolate(List<int> hand,ref int isolateCount)
    {
        List<int>[] group_sort = Grouping_sorted(hand);
        int count = 0;

        int[] allowances = { 2, 2, 2, 0 };
        for (int g = 0; g < group_sort.Length; g++)
        {
            for (int i = group_sort[g].Count - 1 - 1; 0 + 1 <= i; i--)
            {
                if (Math.Abs(group_sort[g][i - 1] / 2 - group_sort[g][i] / 2) > allowances[g] &&
                    Math.Abs(group_sort[g][i] / 2 - group_sort[g][i + 1] / 2) > allowances[g])
                {
                    group_sort[g].RemoveAt(i);
                    count++;
                }
            }

            if (group_sort[g].Count >= 2)
            {
                if (Math.Abs(group_sort[g][group_sort[g].Count - 1 - 1] / 2 -
                    group_sort[g][group_sort[g].Count - 1] / 2) > allowances[g])
                {
                    group_sort[g].RemoveAt(group_sort[g].Count - 1);
                    count++;
                }
            }
            if (group_sort[g].Count >= 2)
            {
                if (Math.Abs(group_sort[g][0] / 2 -
                    group_sort[g][1] / 2) > allowances[g])
                {
                    group_sort[g].RemoveAt(0);
                    count++;
                }
            }
        }

        isolateCount = count;
        return ;
    }

    static private void removeIsolatedCards(ref List<int>[] groupAndSorted)
    {
        for (int g = 0; g < groupAndSorted.Length; g++)
        {
            for (int i = groupAndSorted[g].Count - 1 - 1; 0 + 1 <= i; i--)
            {
                if (Math.Abs(groupAndSorted[g][i - 1] / 2 - groupAndSorted[g][i] / 2) > 2 &&
                    Math.Abs(groupAndSorted[g][i] / 2 - groupAndSorted[g][i + 1] / 2) > 2)
                {
                    groupAndSorted[g].RemoveAt(i);
                }
            }

            if (groupAndSorted[g].Count >= 2)
            {
                if (Math.Abs(groupAndSorted[g][groupAndSorted[g].Count - 1 - 1] / 2 -
                    groupAndSorted[g][groupAndSorted[g].Count - 1] / 2) > 2)
                {
                    groupAndSorted[g].RemoveAt(groupAndSorted[g].Count - 1);
                }
            }
            if (groupAndSorted[g].Count >= 2)
            {
                if (Math.Abs(groupAndSorted[g][0] / 2 -
                    groupAndSorted[g][1] / 2) > 2)
                {
                    groupAndSorted[g].RemoveAt(0);
                }
            }
        }
    }

    static private bool Syanten_0(List<int>[] group_sort_removeIsolate, int isolate)
    {
        if (isolate <= 1)
        {
            int head = 0;
            if (isolate == 1)
            {
                head = 1+1;
            }
            else
            {
                for (int g = 0; g < group_sort_removeIsolate.Length; g++)
                {
                    head += group_sort_removeIsolate[g].Count;
                }
            }
            for (int h = 1; h < head; h++)
            {
                for (int g = 0; g < group_sort_removeIsolate.Length; g++)
                {
                    if (group_sort_removeIsolate[g].Count % 3 == 0)
                    {
                        for (int i = 0; i < group_sort_removeIsolate[g].Count / 3; i++)
                        {
                            if (Set3(group_sort_removeIsolate[g][i * 3 + 0],
                                group_sort_removeIsolate[g][i * 3 + 1],
                                group_sort_removeIsolate[g][i * 3 + 2]) == false)
                            {
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        return false;
    }

    static private bool Set3(int card1,int card2,int card3)
    {
        return Straight3(card1, card2, card3) || Same3(card1, card2, card3);
    }

    static private bool Straight3(int card1,int card2,int card3)
    {
        if (card1 / 2 / 9 == card2 / 2 / 9 - 1 && card2 / 2 / 9 + 1 == card3 / 2 / 9)
        {
            return true;
        }
        return false;
    }

    static private bool Same3(int card1,int card2,int card3)
    {
        if (card1 / 2 / 9 == card2 / 2 / 9 && card2 / 2 / 9 == card3 / 2 / 9)
        {
            return true;
        }
        return false;
    }*/
}