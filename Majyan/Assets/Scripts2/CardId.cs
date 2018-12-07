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
}
