using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yaku{

    static readonly public Dictionary<string, int> keyValuePairs = new Dictionary<string, int>()
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

	static public void Judgements(List<int> hand)
    {

    }

    static private int Yakuhai(List<int> hand)
    {
        //白、発、中の各枚数をカウント
        int[] cardCounts = new int[3];
        ArrayBase.ResetArray(cardCounts, 0);
        for (int h = 0; h < hand.Count; h++)
        {
            for (int c = 0; c < cardCounts.Length; c++)
            {
                if (hand[h]/2 == 9 * 3 + 4 + c)
                {
                    cardCounts[c]++;
                }
            }
        }

        //役の数を取得
        int yakuCount = 0;
        for (int c = 0; c < cardCounts.Length; c++)
        {
            if (cardCounts[c] >= 3)
            {
                yakuCount++;
            }
        }

        return yakuCount;
    }

    static private int Tanyao(List<int> hand)
    {
        for(int i = 0; i < hand.Count; i++)
        {
            //字牌なら判定終了
            if (hand[i] / 2 >= 3 * 9)
            {
                return 0;
            }

            //1または9ならば判定終了
            if((hand[i]/2)%9==1-1|| (hand[i] / 2) % 9 == 9 - 1)
            {
                return 0;
            }
        }

        return 1;
    }

}
