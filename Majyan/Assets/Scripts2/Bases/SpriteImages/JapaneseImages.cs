using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JapaneseImages : SpriteImages
{
    public JapaneseImages()
    {
        LoadMultipleSprites_SortedId("fonts/japanese");
    }

    public override Sprite GetSprite(int id)
    {
        return sprites[id];
    }
}
