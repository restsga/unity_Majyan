using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardId {

    //牌の種類(赤ドラと通常牌は同一牌とみなす)
	static public int Kind(int id)
    {
        return id / 2;
    }

    //赤ドラ
    static public bool RedBonusCard(int id)
    {
        return id % 2 == 1;
    }

    //同一牌判定(赤ドラと通常牌は同一牌とみなす)
    static public bool Same(int card1,int card2)
    {
        return Kind(card1) == Kind(card2);
    }
}
