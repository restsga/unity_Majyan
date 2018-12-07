using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardImages:SpriteImages {

    public CardImages()
    {
        LoadMultipleSprites_SortedId("cards");
    }

    //表面
    public override Sprite GetSprite(int id)
    {
        return sprites[Index(id)];
    }

    //裏面
    public Sprite GetBackSprite()
    {
        return sprites[sprites.Length - 1];
    }

    private int Index(int id)
    {
        int kind = CardId.Kind(id);

        if (CardId.RedBonusCard(id) == false)
        {
            //通常の牌
            return kind;
        }
        else
        {
            //赤ドラ
            return 34 + kind / 9;
        }
    }
}
