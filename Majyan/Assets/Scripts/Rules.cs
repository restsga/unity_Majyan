using System;
using System.Collections.Generic;

public class Rules
{

    //牌idを画像idに変換するための配列
    static private int[] serialToCardImageId = {0,0,0,0,1,1,1,1,2,2,2,2,3,3,3,3,4,5,5,5,6,6,6,6,7,7,7,7,8,8,8,8,9,9,9,9,
    10,10,10,10,11,11,11,11,12,12,12,12,13,13,13,13,14,15,15,15,16,16,16,16,17,17,17,17,18,18,18,18,19,19,19,19,
    20,20,20,20,21,21,21,21,22,22,22,22,23,23,23,23,24,25,25,25,26,26,26,26,27,27,27,27,28,28,28,28,29,29,29,29,
    30,30,30,30,31,31,31,31,32,32,32,32,33,33,33,33,34,34,34,34,35,35,35,35,36,36,36,36};

    //牌idを画像idに変換
    static public int IdChangeSerialToCardImageId(int serialNumber)
    {
        return serialToCardImageId[serialNumber];
    }

    //プレイヤーidを方角idに変換
    static public int PlayerIdToHouseId(int startPlayer, int round, int player)
    {
        return (Math.Abs(startPlayer - (player + 4)) + round) % 4;
    }

    //方角idをプレイヤーidに変換
    static public int HouseIdToPlayerId(int startPlayer, int round, int house)
    {
        return (startPlayer + round + house) % 4;
    }

    //ポンや明カンが出来るか判定
    static public bool[] CanPonOrOpenKan(List<int> hand, int discard)
    {
        int count = 1;  //捨て牌を含めた同一牌数カウンタ

        for (int i = 0; i < hand.Count; i++)
        {
            if (Same_BonusEquate(hand[i], discard))   //捨て牌と同一の牌が手札にあればカウントを増やす
            {
                count++;
            }
        }

        bool[] pon_kan = new bool[2];

        if (count == 3)
        {
            //ポンのみ可
            pon_kan[0] = true;
            pon_kan[1] = false;
        }
        else if (count == 4)
        {
            //ポン、カンどちらも可
            pon_kan[0] = true;
            pon_kan[1] = true;
        }
        else
        {
            //ポン、カンどちらも不可
            pon_kan[0] = false;
            pon_kan[1] = false;
        }

        return pon_kan;
    }

    //赤ドラを含めて同一の牌か判定
    static public bool Same_BonusEquate(int card1, int card2)
    {
        if (card1 < 0 || card2 < 0)
        {
            return false;
        }
        return card1 / 4 == card2 / 4;
    }

    //赤ドラか判定
    static public bool Bonus5(int card)
    {
        return (IdChangeSerialToCardImageId(card) % 10 == 4 && IdChangeSerialToCardImageId(card) <= 30);
    }

    //チーが出来るか判定
    static public bool CanTi(List<int> hand, int discard)
    {
        List<int> sameGroupCards = new List<int>();
        int[,] numbers = { { -2, -1 }, { -1, 1 }, { 1, 2 } };   //チーのパターン

        if (IdChangeSerialToCardImageId(discard) <= 29)     //字牌以外の場合
        {
            //捨て牌と同じグループの牌のみを抽出
            for (int i = 0; i < hand.Count; i++)
            {
                if (SameGroup(hand[i], discard))
                {
                    sameGroupCards.Add(hand[i]);
                }
            }
            sameGroupCards.Sort();  //並べ替え

            for (int c = 0; c < 3; c++) {
                int check = 0;
                for (int i = 0; i < sameGroupCards.Count; i++)
                {
                    if (Same_BonusEquate(sameGroupCards[i], discard + numbers[c, check] * 4))
                    {
                        check++;
                        if (check >= 2)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    //グループが同じか判定
    static public bool SameGroup(int card1, int card2)
    {
        if (card1 < 0 || card2 < 0)
        {
            return false;
        }
        return card1 / 4 / 9 == card2 / 4 / 9;
    }

    //暗カンが出来るか判定
    static public bool CanClosedKan(List<int> hand)
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
            if (Same_BonusEquate(sortedHand[i - 1], sortedHand[i]))
            {
                chain++;

                if (chain >= 4)
                {
                    return true;
                }
            }
            else
            {
                chain = 1;
            }
        }
         
        return false;
    }

    //加カンが出来るか判定
    static public bool CanAddKan(List<int> hand, List<CallCardsSet> call)
    {
        for (int s = 0; s < call.Count; s++)
        {
            if (CallCardKinds(call[s]) == PON)
            {
                for(int i = 0; i < hand.Count; i++)
                {
                    if (Same_BonusEquate(hand[i], call[s].callCards[0].card))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //鳴き牌の種類判定
    readonly public static int ERROR=0, TI = 1, PON = 2, KAN = 3;
    static public int CallCardKinds(CallCardsSet call)
    {
        if (call.callCards.Count == 4)
        {
            return KAN;
        }
        if (call.callCards.Count == 3)
        {
            if (Same_BonusEquate(call.callCards[0].card, call.callCards[1].card))
            {
                return PON;
            }
            else
            {
                return TI;
            }
        }

        return ERROR;
    }
}
