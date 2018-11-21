using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardImages {

    // 牌画像 //
    static private Sprite[] cardImages;

    static public void Initialize()
    {
        cardImages= LoadSprites.SortedNumbers_Multiple("cards");
    }

    static public int Index(int card)
    {
        int kind = card / 2;
        int bonus = card % 2;
        if (bonus == 0)
        {
            return kind;
        }
        else
        {
            return 34 + kind / 9;
        }
    }

    static public Sprite Image_Front(int card)
    {
        return cardImages[Index(card)];
    }

    static public Sprite Image_Back()
    {
        return cardImages[cardImages.Length - 1];
    }
}
