using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YakuJudgementDatas
{
    internal List<int> hand;
    internal List<CallCardsSet> call;
    internal int winCard;
    internal int fieldWind;
    internal int seatWind;

    internal bool riichi;
    internal bool ippatsu;
    internal bool doubleRiichi;
    internal bool selfDraw;
    internal bool deckLast;
    internal bool kanCard;

    public YakuJudgementDatas(List<int> hand,List<CallCardsSet> call, int winCard,
        int fieldWind,int seatWind,bool riichi,bool ippatsu, bool doubleRiichi,bool selfDraw,
        bool deckLast,bool kanCard)
    {
        this.hand = hand;
        this.call = call;
        this.winCard = winCard;
        this.fieldWind = fieldWind;
        this.seatWind = seatWind;

        this.riichi = riichi;
        this.ippatsu = ippatsu;
        this.doubleRiichi = doubleRiichi;
        this.selfDraw = selfDraw;
        this.deckLast = deckLast;
        this.kanCard = kanCard;
    }
}

public class Yaku {

    static readonly public Dictionary<string, int> yaku_name_value = new Dictionary<string, int>()
    {
        { "立直",0 },{"一発",1 },{"自摸",2 },{"役牌",3 },{"タンヤオ",4 },
        { "平和",5 },{"一盃口",6 },{"海底撈月",7 },{"ホウテイ",8 },{"嶺上開花",9 },
        { "槍槓",10 },{"ダブル立直",11 },{"三色同順",12 },{"三色同刻",13 },{"三暗刻",14 },
        { "一気通貫",15 },{"七対子",16 },{"対々和",17 },{"チャンタ",18 },{"三槓子",19 },
        { "二盃口",20 },{"ジュンチャン",21 },{"ホンイツ",22 },{"小三元",23 },{"混老頭",24 },
        { "清一色",25 },{"四暗刻",26 },{"大三元",27 },{"国士無双",28 },{"緑一色",29 },
        { "字一色",30 },{"チンロウトウ",31 },{"四槓子",32 },{"小四喜",33 },{"大四喜",34 },
        { "チューレンポウトウ",35 },{"地和",36 },{"天和",37 }
    };

    static public int[] Judgements(YakuJudgementDatas judgementDatas, ref int returnFu)
    {
        int[] fan = new int[27];
        ArrayBase.ResetArray(fan, 0);

        fan[yaku_name_value["ダブル立直"]] =
            DoubleRiichi(judgementDatas.doubleRiichi);
        if (fan[yaku_name_value["ダブル立直"]] <= 0)
        {
            fan[yaku_name_value["立直"]] =
                RiichiAndDoubleRiichi(judgementDatas.riichi);
        }
        fan[yaku_name_value["一発"]] =
            Ippatsu(judgementDatas.ippatsu);
        fan[yaku_name_value["自摸"]] =
            Tumo(judgementDatas.call, judgementDatas.selfDraw);
        fan[yaku_name_value["役牌"]] =
            Yakuhai(judgementDatas.hand, judgementDatas.call, judgementDatas.fieldWind, judgementDatas.seatWind);
        fan[yaku_name_value["タンヤオ"]] =
            Tanyao(judgementDatas.hand,judgementDatas.call);
        fan[yaku_name_value["海底撈月"]] =
            Haitei(judgementDatas.deckLast, judgementDatas.selfDraw);
        fan[yaku_name_value["ホウテイ"]] =
            Houtei(judgementDatas.deckLast, judgementDatas.selfDraw);
        fan[yaku_name_value["嶺上開花"]] =
            Rinsyankaihou(judgementDatas.kanCard);
        //fan[yaku_name_value["槍槓"]] =
        fan[yaku_name_value["三槓子"]] =
            Sankantsu(judgementDatas.call);
        fan[yaku_name_value["清一色"]] =
            Tinitu(judgementDatas.hand, judgementDatas.call);
        if (fan[yaku_name_value["清一色"]] <= 0)
        {
            fan[yaku_name_value["ホンイツ"]] =
                Honitu(judgementDatas.hand, judgementDatas.call);
        }
        fan[yaku_name_value["小三元"]] =
            Syousangen(judgementDatas.hand, judgementDatas.call);
        fan[yaku_name_value["混老頭"]] =
            Honroutou(judgementDatas.hand,judgementDatas.call);



        List < List<int[]>> analyzedHand = HandAnalysis(judgementDatas.hand);
        int[] fan_draft_max = new int[27];
        ArrayBase.ResetArray(fan_draft_max, 0);
        int max_fan = 0, max_fu = 0; ;
        for (int i = 0; i < analyzedHand.Count; i++)
        {
            int[] fan_draft = new int[27];
            ArrayBase.ResetArray(fan_draft, 0);

            fan_draft[yaku_name_value["平和"]] =
                Pinfu(analyzedHand[i], judgementDatas.call, judgementDatas.winCard,
                judgementDatas.fieldWind, judgementDatas.seatWind);
            fan_draft[yaku_name_value["二盃口"]] =
                Ryanpeikou(analyzedHand[i], judgementDatas.call);
            if (fan_draft[yaku_name_value["二盃口"]] <= 0)
            {
                fan_draft[yaku_name_value["一盃口"]] =
                    Iipeikou(analyzedHand[i], judgementDatas.call);
            }
            fan_draft[yaku_name_value["三色同順"]] =
                Sansyokudoujyun(analyzedHand[i], judgementDatas.call);
            fan_draft[yaku_name_value["三色同刻"]] =
                Sansyokudoukou(analyzedHand[i], judgementDatas.call);
            fan_draft[yaku_name_value["三暗刻"]] =
                Sanankou(analyzedHand[i], judgementDatas.winCard, judgementDatas.selfDraw);
            fan_draft[yaku_name_value["一気通貫"]] =
                Ikkituukan(analyzedHand[i], judgementDatas.call);
            fan_draft[yaku_name_value["対々和"]] =
                Toitoihou(analyzedHand[i], judgementDatas.call);
            fan_draft[yaku_name_value["ジュンチャン"]] =
                Jyuntyan(analyzedHand[i], judgementDatas.call);
            if (fan_draft[yaku_name_value["ジュンチャン"]] <= 0 &&
                fan_draft[yaku_name_value["混老頭"]] <= 0)
            {
                fan_draft[yaku_name_value["チャンタ"]] =
                    Tyanta(analyzedHand[i], judgementDatas.call);
            }

            if (fan_draft.Sum() >= max_fan)
            {
                int fu =
                        Fu(analyzedHand[i], judgementDatas.call, judgementDatas.winCard,
                        judgementDatas.fieldWind, judgementDatas.seatWind, judgementDatas.selfDraw,
                        fan_draft[yaku_name_value["平和"]]);

                if (fan_draft.Sum() == max_fan)
                {
                    if (fu > max_fu)
                    {
                        fan_draft_max = fan_draft;
                        max_fan = fan_draft.Sum();
                        max_fu = fu;
                    }
                }
                else
                {
                    fan_draft_max = fan_draft;
                    max_fan = fan_draft.Sum();
                    max_fu = fu;
                }
            }
        }

        for (int i = 0; i < fan.Length; i++)
        {
            fan[i] += fan_draft_max[i];
        }
        returnFu = max_fu;

        //fan[yaku_name_value["七対子"]] =

        return fan;
    }

    static private List<List<int[]>> HandAnalysis(List<int> hand)
    {
        int[] cardCount_row = AI.CardCount(hand);
        int[] cardCount;
        List<List<int[]>> analyzed_all = new List<List<int[]>>();

        for (int h = 0; h < cardCount_row.Length; h++)
        {
            cardCount = ArrayBase.CopyForEdit(cardCount_row);
            List<int[]> analyzed_child = new List<int[]>();

            if (cardCount[h] >= 2)
            {
                cardCount[h] -= 2;
                analyzed_child.Add(new int[] { h, h });
                RecursiveAnalysis(cardCount, analyzed_child, analyzed_all);
            }
        }

        return analyzed_all;
    }

    static private void RecursiveAnalysis(int[] cardCount, List<int[]> analyzed_child, List<List<int[]>> analyzed_all)
    {
        if ((Array.FindIndex<int>(cardCount, n => n > 0) >= 0) == false)
        {
            List<int[]> analyzed_child_copy=ArrayBase.CopyForEdit(analyzed_child);
            analyzed_all.Add(analyzed_child_copy);
            return;
        }
        for (int i = 0; i < cardCount.Length; i++)
        {
            if (cardCount[i] >= 3)
            {
                analyzed_child.Add(new int[] { i, i, i });
                cardCount[i] -= 3;
                RecursiveAnalysis(cardCount, analyzed_child, analyzed_all);

                analyzed_child.RemoveAt(analyzed_child.Count - 1);
                cardCount[i] += 3;
            }
            if (cardCount[i] >= 1)
            {
                if (i < 3 * 9 && i % 9 <= 7 - 1)
                {
                    if (cardCount[i + 1] >= 1 && cardCount[i + 2] >= 1)
                    {
                        analyzed_child.Add(new int[] { i, i + 1, i + 2 });
                        cardCount[i + 0]--;
                        cardCount[i + 1]--;
                        cardCount[i + 2]--;
                        RecursiveAnalysis(cardCount, analyzed_child, analyzed_all);

                        analyzed_child.RemoveAt(analyzed_child.Count - 1);
                        cardCount[i + 0]++;
                        cardCount[i + 1]++;
                        cardCount[i + 2]++;
                    }
                }
            }
        }
    }

    static private int Fu(List<int[]> analyzedHand,List<CallCardsSet> call,int winCard,
        int fieldWind,int seatWind,bool selfDraw,int pinfu)
    {
        if (pinfu >= 1)
        {
            if (selfDraw)
            {
                return 20;
            }
            else
            {
                return 30;
            }
        }
        int fu = 20;
        if (IsThisClosedHand(call))
        {
            if (selfDraw == false)
            {
                fu += 10;
            }
        }
        for(int i = 1; i < analyzedHand.Count; i++)
        {
            if(analyzedHand[i][0]== analyzedHand[i][1])
            {
                if (analyzedHand[i][0] < 3 * 9&& analyzedHand[i][0]%9!=1-1&& analyzedHand[i][0]%9!=9-1)
                {
                    if(analyzedHand[i][0]== winCard&&selfDraw==false)
                    {
                        fu += 2;
                    }
                    else
                    {
                        fu += 4;
                    }
                }
                else
                {
                    if (analyzedHand[i][0] == winCard&&selfDraw==false)
                    {
                        fu += 4;
                    }
                    else
                    {
                        fu += 8;
                    }
                }
            }
        }
        for(int i = 0; i < call.Count; i++)
        {
            if (call[i].IsThisPon())
            {
                if(call[i].callCards[0].cardId< 3 * 9 &&
                    call[i].callCards[0].cardId % 9 != 1 - 1 &&
                    call[i].callCards[0].cardId % 9 != 9 - 1)
                {
                    fu += 2;
                }
                else
                {
                    fu += 4;
                }
            }
            if (call[i].IsThisClosedKan())
            {
                if (call[i].callCards[0].cardId < 3 * 9 &&
                    call[i].callCards[0].cardId % 9 != 1 - 1 &&
                    call[i].callCards[0].cardId % 9 != 9 - 1)
                {
                    fu += 16;
                }
                else
                {
                    fu += 32;
                }
            }
            else if (call[i].IsThisKan())
            {
                if(call[i].callCards[0].cardId < 3 * 9 &&
                    call[i].callCards[0].cardId % 9 != 1 - 1 &&
                    call[i].callCards[0].cardId % 9 != 9 - 1)
                {
                    fu += 8;
                }
                else
                {
                    fu += 16;
                }
            }
        }
        if (selfDraw)
        {
            fu += 2;
        }
        if (analyzedHand[0][0] == winCard)
        {
            fu += 2;
        }
        else
        {
            if (winCard < 3 * 9) {
                for (int i = 1; i < analyzedHand.Count; i++)
                {
                    if (analyzedHand[i][1] == winCard)
                    {
                        fu += 2;
                        break;
                    }
                    if (analyzedHand[i][0] == winCard &&
                        analyzedHand[i][2] % 9 == 9 - 1)
                    {
                        fu += 2;
                        break;
                    }
                    if (analyzedHand[i][0]%9==1-1&&
                        analyzedHand[i][2] == winCard)
                    {
                        fu += 2;
                        break;
                    }
                }
            }
        }
        int head = analyzedHand[0][0];
        if(head == 9 * 3 + fieldWind || head == 9 * 3 + seatWind ||
                head == 9 * 3 + 4 + 0 || head == 9 * 3 + 4 + 1 || head == 9 * 3 + 4 + 2)
        {
            fu += 2;
        }

        if (fu % 10 != 0)
        {
            fu = ((fu / 10) + 1) * 10;
        }
        return fu;
    }

    static public bool IsThisClosedHand(List<CallCardsSet> call)
    {
        for (int i = 0; i < call.Count; i++)
        {
            if (call[i].IsThisClosedKan() == false)
            {
                return false;
            }
        }

        return true;
    }

    static private int RiichiAndDoubleRiichi(bool riichi)
    {
        if (riichi)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    static private int Ippatsu(bool ippatsu)
    {
        if (ippatsu)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    static private int Tumo(List<CallCardsSet> call,bool selfDraw)
    {
        if (IsThisClosedHand(call))
        {
            if (selfDraw)
            {
                return 1;
            }
        }
        return 0;
    }
    static private int Yakuhai(List<int> hand, List<CallCardsSet> call, int fieldWind, int seatWind)
    {
        //手牌内の白、発、中、場風、自風の各枚数をカウント
        int[] cardCounts = new int[5];
        ArrayBase.ResetArray(cardCounts, 0);

        for (int h = 0; h < hand.Count; h++)
        {
            for (int c = 0; c < 3; c++)
            {
                if (hand[h] / 2 == 9 * 3 + 4 + c)
                {
                    cardCounts[c]++;
                }
            }
            if (hand[h] / 2 == 9 * 3 + fieldWind)
            {
                cardCounts[3]++;
            }
            if (hand[h] / 2 == 9 * 3 + seatWind)
            {
                cardCounts[4]++;
            }
        }

        //手牌内のみについて役の数を取得
        int yakuCount = 0;
        for (int c = 0; c < cardCounts.Length; c++)
        {
            if (cardCounts[c] >= 3)
            {
                yakuCount++;
            }
        }

        //鳴き牌内の白、発、中、場風、自風の面子数を役の数に足す
        for (int i = 0; i < call.Count; i++)
        {
            if (call[i].IsThisPon() || call[i].IsThisKan())
            {
                for (int c = 0; c < 3; c++)
                {
                    if (call[i].callCards[0].cardId / 2 == 9 * 3 + 4 + c)
                    {
                        yakuCount++;
                    }
                }
                if (call[i].callCards[0].cardId / 2 == 9 * 3 + fieldWind)
                {
                    yakuCount++;
                }
                if (call[i].callCards[0].cardId / 2 == 9 * 3 + seatWind)
                {
                    yakuCount++;
                }
            }
        }

        return yakuCount;
    }
    static private int Tanyao(List<int> hand,List<CallCardsSet> call)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            //字牌なら判定終了
            if (hand[i] / 2 >= 3 * 9)
            {
                return 0;
            }

            //1または9ならば判定終了
            if ((hand[i] / 2) % 9 == 1 - 1 || (hand[i] / 2) % 9 == 9 - 1)
            {
                return 0;
            }
        }
        for(int s = 0; s<call.Count; s++)
        {
            for(int i = 0; i < call[s].callCards.Count; i++)
            {
                if (call[s].callCards[i].cardId / 2 >= 3 * 9)
                {
                    return 0;
                }
                if((call[s].callCards[i].cardId / 2) % 9 == 1 - 1 ||
                    (call[s].callCards[i].cardId / 2) % 9 == 9 - 1)
                {
                    return 0;
                }
            }
        }

        return 1;
    }
    static private int Pinfu(List<int[]> analyzedHand, List<CallCardsSet> call, int winCard, int fieldWind, int seatWind)
    {
        //字牌で両面待ちは出来ない
        if (winCard >= 3 * 9)
        {
            return 0;
        }
        //暗槓以外で鳴いていたら門前が崩れ、暗槓の場合でも順子だけという条件を満たせないので鳴きは一切存在し得ない
        if (call.Count <= 0)
        {
            //雀頭が場風、自風、白、発、中ではない
            int head = analyzedHand[0][0];
            if (head != 9 * 3 + fieldWind && head != 9 * 3 + seatWind &&
                head != 9 * 3 + 4 + 0 && head != 9 * 3 + 4 + 1 && head != 9 * 3 + 4 + 2)
            {
                bool bothWaiting = false;
                for (int i = 1; i < analyzedHand.Count; i++)
                {
                    int[] set = analyzedHand[i];

                    //順子ではない場合は役の条件に反するため役無しで処理を終了
                    if ((set[0] + 1 == set[1] && set[1] == set[2] - 1) == false)
                    {
                        return 0;
                    }

                    //順子の端と上がり牌が一致する場合は両面待ち
                    if (set[0] == winCard || set[2] == winCard)
                    {
                        bothWaiting = true;
                    }
                }

                //ここまでたどり着いてかつ両面待ちなら平和の役がある
                if (bothWaiting)
                {
                    return 1;
                }
            }
        }

        return 0;
    }
    static private int Iipeikou(List<int[]> analyzedHand, List<CallCardsSet> call)
    {
        //門前である
        if (IsThisClosedHand(call))
        {
            for (int g_main = 1; g_main < analyzedHand.Count; g_main++)
            {
                for (int g_sub = 1; g_sub < analyzedHand.Count; g_sub++)
                {
                    if (g_main < g_sub)
                    {
                        //全く同じ順子が検出されたら役がある
                        if (analyzedHand[g_main][0] == analyzedHand[g_sub][0] &&
                            analyzedHand[g_main][1] == analyzedHand[g_sub][1] &&
                            analyzedHand[g_main][2] == analyzedHand[g_sub][2])
                        {
                            return 1;
                        }
                    }
                }
            }
        }
        return 0;
    }
    static private int Haitei(bool deckLast,bool selfDraw)
    {
        if (deckLast && selfDraw)
        {
            return 1;
        }
        return 0;
    }
    static private int Houtei(bool deckLast, bool selfDraw)
    {
        if (deckLast && selfDraw==false)
        {
            return 1;
        }
        return 0;
    }
    static private int Rinsyankaihou(bool kanCard)
    {
        if (kanCard)
        {
            return 1;
        }
        return 0;
    }
    static private int DoubleRiichi(bool doubleRiichi)
    {
        if (doubleRiichi)
        {
            return 2;
        }
        return 0;
    }
    static private int Sansyokudoujyun(List<int[]> analyzedHand_row, List<CallCardsSet> call)
    {
        List<int[]> analyzedHand = ArrayBase.CopyForEdit(analyzedHand_row);
        for (int g = 0; g < call.Count; g++)
        {
            if (call[g].IsThisTi())
            {
                int[] copy = new int[call[g].callCards.Count];
                for (int i = 0; i < copy.Length; i++)
                {
                    copy[i] = call[g].callCards[i].cardId / 2;
                }
                Array.Sort(copy);
                analyzedHand.Add(copy);
            }
        }

        int callPenalty = 1;
        if (IsThisClosedHand(call))
        {
            callPenalty = 0;
        }

        for (int g1 = 1; g1 < analyzedHand.Count; g1++)
        {
            for (int g2 = 1; g2 < analyzedHand.Count; g2++)
            {
                for (int g3 = 1; g3 < analyzedHand.Count; g3++)
                {
                    if (g1 < g2 && g2 < g3)
                    {
                        //全部字牌以外
                        if (analyzedHand[g1][0] < 3 * 9 &&
                            analyzedHand[g2][0] < 3 * 9 &&
                            analyzedHand[g3][0] < 3 * 9)
                        {
                            //全部違う色
                            if (analyzedHand[g1][0] / 9 != analyzedHand[g2][0] / 9 &&
                                analyzedHand[g2][0] / 9 != analyzedHand[g3][0] / 9 &&
                                analyzedHand[g3][0] / 9 != analyzedHand[g1][0] / 9)
                            {
                                //全部刻子ではない
                                if (analyzedHand[g1][0] != analyzedHand[g1][1] &&
                                    analyzedHand[g2][0] != analyzedHand[g2][1] &&
                                    analyzedHand[g3][0] != analyzedHand[g3][1])
                                {
                                    //全部同じ数字の順子
                                    if (analyzedHand[g1][0] % 9 == analyzedHand[g2][0] % 9 &&
                                        analyzedHand[g1][1] % 9 == analyzedHand[g2][1] % 9 &&
                                        analyzedHand[g1][2] % 9 == analyzedHand[g2][2] % 9 &&
                                        analyzedHand[g2][0] % 9 == analyzedHand[g3][0] % 9 &&
                                        analyzedHand[g2][1] % 9 == analyzedHand[g3][1] % 9 &&
                                        analyzedHand[g2][2] % 9 == analyzedHand[g3][2] % 9)
                                    {
                                        return 2 - callPenalty;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return 0;
    }
    static private int Sansyokudoukou(List<int[]> analyzedHand, List<CallCardsSet> call)
    {
        List<int> cards = new List<int>();
        cards.Clear();
        for (int i = analyzedHand.Count-1; 1 <= i; i--)
        {
            if (analyzedHand[i][0] == analyzedHand[i][1] &&
                analyzedHand[i][1] == analyzedHand[i][2])
            {
                cards.Add(analyzedHand[i][0]);
            }
        }
        for (int g = 0; g < call.Count; g++)
        {
            if (call[g].IsThisPon() || call[g].IsThisKan())
            {
                cards.Add(call[g].callCards[0].cardId / 2);
            }
        }
        for (int g1 = 1; g1 < cards.Count; g1++)
        {
            for (int g2 = 1; g2 < cards.Count; g2++)
            {
                for (int g3 = 1; g3 < cards.Count; g3++)
                {
                    if (g1 < g2 && g2 < g3)
                    {
                        //全部字牌以外
                        if (cards[g1] < 3 * 9 &&
                            cards[g2] < 3 * 9 &&
                            cards[g3] < 3 * 9)
                        {
                            //全部違う色
                            if (cards[g1] / 9 != cards[g2] / 9 &&
                                cards[g2] / 9 != cards[g3] / 9 &&
                                cards[g3] / 9 != cards[g1] / 9)
                            {
                                //全部同じ数字
                                if (cards[g1] % 9 == cards[g2] % 9 &&
                                    cards[g2] % 9 == cards[g3] % 9)
                                {
                                    return 2;
                                }
                            }
                        }
                    }
                }
            }
        }
        return 0;
    }
    static private int Sanankou(List<int[]> analyzedHand, int winCard, bool selfDraw)
    {
        int count = 0;
        for (int i = 1; i < analyzedHand.Count; i++)
        {
            if ((analyzedHand[i][0] != winCard || selfDraw) &&
                analyzedHand[i][0] == analyzedHand[i][1] &&
                analyzedHand[i][1] == analyzedHand[i][2])
            {
                count++;
            }
        }
        if (count >= 3)
        {
            return 2;
        }
        return 0;
    }
    static private int Ikkituukan(List<int[]> analyzedHand_row, List<CallCardsSet> call)
    {
        List<int[]> analyzedHand = ArrayBase.CopyForEdit(analyzedHand_row);
        for (int g = 0; g < call.Count; g++)
        {
            if (call[g].IsThisTi())
            {
                int[] copy = new int[call[g].callCards.Count];
                for (int i = 0; i < copy.Length; i++)
                {
                    copy[i] = call[g].callCards[i].cardId / 2;
                }
                Array.Sort(copy);
                analyzedHand.Add(copy);
            }
        }

        int callPenalty = 1;
        if (IsThisClosedHand(call))
        {
            callPenalty = 0;
        }

        int ittuuColor = -1;
        for (int c = 0; c < 3; c++)
        {
            int count = 0;
            for (int i = 1; i < analyzedHand.Count; i++)
            {
                if (analyzedHand[i][0] / 3 == c)
                {
                    count += 3;
                }
            }
            if (count >= 9)
            {
                ittuuColor = c;
                break;
            }
        }
        if (ittuuColor >= 0)
        {
            bool[] num147 = new bool[3];
            for (int g = 1; g < analyzedHand.Count; g++)
            {
                if (analyzedHand[g][0] / 3 == ittuuColor)
                {
                    //順子
                    if (analyzedHand[g][0] + 1 == analyzedHand[g][1] && analyzedHand[g][1] == analyzedHand[g][2] - 1)
                    {
                        for (int i = 0; i < num147.Length; i++)
                        {
                            if (analyzedHand[g][0]%9 == i * 3)
                            {
                                num147[i] = true;
                            }
                        }
                    }
                }
            }
            if ((Array.FindIndex<bool>(num147, boolean => boolean == false) >= 0) == false)
            {
                return 2 - callPenalty;
            }
        }
        return 0;
    }
    static private int Toitoihou(List<int[]> analyzedHand,List<CallCardsSet> call)
    {
        for(int i = 1; i < analyzedHand.Count; i++)
        {
            if (analyzedHand[i][0] != analyzedHand[i][1])
            {
                return 0;
            }
        }
        for(int i = 0; i < call.Count; i++)
        {
            if (call[i].IsThisTi())
            {
                return 0;
            }
        }

        return 2;
    }
    static private int Tyanta(List<int[]> analyzedHand_row,List<CallCardsSet> call)
    {
        List<int[]> analyzedHand = ArrayBase.CopyForEdit(analyzedHand_row);
        for (int g = 0; g < call.Count; g++)
        {
            int[] copy = new int[3];
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = call[g].callCards[i].cardId / 2;
            }
            Array.Sort(copy);
            analyzedHand.Add(copy);
        }

        int callPenalty = 1;
        if (IsThisClosedHand(call))
        {
            callPenalty = 0;
        }

        if (analyzedHand[0][0]<3*9&&analyzedHand[0][0]%9!=1-1&& analyzedHand[0][0] % 9 != 9 - 1)
        {
            return 0;
        }
        for(int i = 1; i < analyzedHand.Count; i++)
        {
            if (analyzedHand[i][0] < 3 * 9 && analyzedHand[i][0] % 9 != 1 - 1 && analyzedHand[i][0] % 9 != 9 - 1&&
                analyzedHand[i][2] < 3 * 9 && analyzedHand[i][2] % 9 != 1 - 1 && analyzedHand[i][2] % 9 != 9 - 1)
            {
                return 0;
            }
        }
        return 2 - callPenalty;
    }
    static private int Sankantsu(List<CallCardsSet> call)
    {
        int count = 0;
        for(int i = 0; i < call.Count; i++)
        {
            if (call[i].IsThisKan())
            {
                count++;
            }
        }
        if (count >= 3)
        {
            return 2;
        }
        return 0;
    }
    static private int Ryanpeikou(List<int[]> analyzedHand_row,List<CallCardsSet> call)
    {
        List<int[]> analyzedHand = ArrayBase.CopyForEdit(analyzedHand_row);
        int count = 0;
        if (IsThisClosedHand(call))
        {
            for (int g_main = analyzedHand.Count-1; 1<=g_main ; g_main--)
            {
                for (int g_sub = analyzedHand.Count-1; 1<=g_sub ; g_sub--)
                {
                    if (g_main < g_sub)
                    {
                        //全く同じ順子が検出された場合はカウントしてそれを削除
                        if (analyzedHand[g_main][0] == analyzedHand[g_sub][0] &&
                            analyzedHand[g_main][1] == analyzedHand[g_sub][1] &&
                            analyzedHand[g_main][2] == analyzedHand[g_sub][2])
                        {
                            analyzedHand.RemoveAt(g_sub);
                            analyzedHand.RemoveAt(g_main);
                            count++;

                            if (count >= 2)
                            {
                                return 3;
                            }
                        }
                    }
                }
            }
        }
        return 0;
    }
    static private int Jyuntyan(List<int[]> analyzedHand_row,List<CallCardsSet> call)
    {
        List<int[]> analyzedHand = ArrayBase.CopyForEdit(analyzedHand_row);
        for (int g = 0; g < call.Count; g++)
        {
            int[] copy = new int[3];
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = call[g].callCards[i].cardId / 2;
            }
            Array.Sort(copy);
            analyzedHand.Add(copy);
        }

        int callPenalty = 1;
        if (IsThisClosedHand(call))
        {
            callPenalty = 0;
        }

        if ((analyzedHand[0][0] < 3 * 9 && (analyzedHand[0][0] % 9 == 1 - 1 || analyzedHand[0][0] % 9 == 9 - 1))==false)
        {
            return 0;
        }
        for (int i = 1; i < analyzedHand.Count; i++)
        {
            if (((analyzedHand[i][0] < 3 * 9 && (analyzedHand[i][0] % 9 == 1 - 1 || analyzedHand[i][0] % 9 == 9 - 1))
                ==false)
                &&
                ((analyzedHand[i][2] < 3 * 9 && (analyzedHand[i][2] % 9 == 1 - 1 || analyzedHand[i][2] % 9 == 9 - 1))
                ==false))
            {
                return 0;
            }
        }
        return 3 - callPenalty;
    }
    static private int Honitu(List<int> hand,List<CallCardsSet> call)
    {
        int callPenalty = 1;
        if (IsThisClosedHand(call))
        {
            callPenalty = 0;
        }

        int c = -1;
        for(int i = 0; i < hand.Count; i++)
        {
            if (hand[i] / 2 < 3 * 9)
            {
                if (c >= 0)
                {
                    if (hand[i] / 2 / 9 != c)
                    {
                        return 0;
                    }
                }
                else 
                {
                    c = hand[i] / 2 / 9;
                }
            }
        }
        for(int i = 0; i < call.Count; i++)
        {
            if (call[i].callCards[0].cardId / 2 < 3 * 9)
            {
                if (c >= 0)
                {
                    if (call[i].callCards[0].cardId / 2 / 9 != c)
                    {
                        return 0;
                    }
                }
                else
                {
                    c = hand[i] / 2 / 9;
                }
            }
        }
        return 3 - callPenalty;
    }
    static private int Syousangen(List<int> hand,List<CallCardsSet> call)
    {
        //手牌内の白、発、中の各枚数をカウント
        int[] cardCounts = new int[3];
        ArrayBase.ResetArray(cardCounts, 0);

        for (int h = 0; h < hand.Count; h++)
        {
            for (int c = 0; c < 3; c++)
            {
                if (hand[h] / 2 == 9 * 3 + 4 + c)
                {
                    cardCounts[c]++;
                }
            }
        }

        //手牌内のみについて役の数を取得
        int yakuCount = 0;
        for (int c = 0; c < cardCounts.Length; c++)
        {
            if (cardCounts[c] >= 3)
            {
                yakuCount++;
            }
        }

        //鳴き牌内の白、発、中の面子数を役の数に足す
        for (int i = 0; i < call.Count; i++)
        {
            if (call[i].IsThisPon() || call[i].IsThisKan())
            {
                for (int c = 0; c < 3; c++)
                {
                    if (call[i].callCards[0].cardId / 2 == 9 * 3 + 4 + c)
                    {
                        yakuCount++;
                    }
                }
            }
        }

        if (yakuCount == 2)
        {
            if (Array.FindIndex<int>(cardCounts, n => n == 2) >= 0)
            {
                return 2;
            }
        }
        return 0;
    }
    static private int Honroutou(List<int> hand,List<CallCardsSet> call)
    {
        for(int i = 0; i < hand.Count; i++)
        {
            int card = hand[i] / 2;
            if (card<3*9&& card % 9 != 1 - 1 && card % 9 != 9 - 1)
            {
                return 0;
            }
        }
        for(int i = 0; i < call.Count; i++)
        {
            if (call[i].IsThisTi())
            {
                return 0;
            }
            else
            {
                int card = call[i].callCards[0].cardId / 2;
                if (card < 3 * 9 && card % 9 != 1 - 1 && card % 9 != 9 - 1)
                {
                    return 0;
                }
            }
        }
        return 2;
    }
    static private int Tinitu(List<int> hand,List<CallCardsSet> call)
    {
        int callPenalty = 1;
        if (IsThisClosedHand(call))
        {
            callPenalty = 0;
        }

        int c = -1;
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] / 2 < 3 * 9)
            {
                if (c >= 0)
                {
                    if (hand[i] / 2 / 9 != c)
                    {
                        return 0;
                    }
                }
                else
                {
                    c = hand[i] / 2 / 9;
                }
            }
            else
            {
                return 0;
            }
        }
        for (int i = 0; i < call.Count; i++)
        {
            if (call[i].callCards[0].cardId / 2 < 3 * 9)
            {
                if (c >= 0)
                {
                    if (call[i].callCards[0].cardId / 2 / 9 != c)
                    {
                        return 0;
                    }
                }
                else
                {
                    c = hand[i] / 2 / 9;
                }
            }
            else
            {
                return 0;
            }
        }
        return 6 - callPenalty;
    }

    static public int Bonus(List<int> hand,List<CallCardsSet> call,int[] bonusCards)
    {
        int count = 0;

        for (int b = 0; b < bonusCards.Length; b++) {
            int bonusId = bonusCards[b];
            for (int h = 0; h < hand.Count; h++)
            {
                //赤ドラ
                if (AI.Bonus5(hand[h]))
                {
                    count++;
                }

                int handId = hand[h] / 2;
                if (handId < 3 * 9)
                {
                    if (handId/ 3 == bonusId / 3)
                    {
                        if (handId % 9 == (bonusId + 1) % 9)
                        {
                            count++;
                        }
                    }
                }
                else
                {
                    if (handId < 3 * 9 + 4)
                    {
                        if ((handId - (3 * 9)) % 4 == (bonusId - (3 * 9) + 1) % 4)
                        {
                            count++;
                        }
                    }
                    else
                    {
                        if ((handId - (3 * 9 + 4)) % 3 == (bonusId - (3 * 9 + 3) + 1) % 4)
                        {
                            count++;
                        }
                    }
                }
            }
            for(int s = 0; s < call.Count; s++)
            {
                for(int i = 0; i < call[s].callCards.Count; i++)
                {
                    //赤ドラ
                    if (AI.Bonus5(call[s].callCards[i].cardId))
                    {
                        count++;
                    }

                    int handId = call[s].callCards[i].cardId / 2;
                    if (handId < 3 * 9)
                    {
                        if (handId / 3 == bonusId / 3)
                        {
                            if (handId % 9 == (bonusId + 1) % 9)
                            {
                                count++;
                            }
                        }
                    }
                    else
                    {
                        if (handId < 3 * 9 + 4)
                        {
                            if ((handId - (3 * 9)) % 4 == (bonusId - (3 * 9) + 1) % 4)
                            {
                                count++;
                            }
                        }
                        else
                        {
                            if ((handId - (3 * 9 + 4)) % 3 == (bonusId - (3 * 9 + 3) + 1) % 4)
                            {
                                count++;
                            }
                        }
                    }
                }
            }
        }

        return count;
    }


    /* メモ
     * 
     * analyzedHand.Count関連のカウンタは原則1スタートなので確認すること
     */
}


