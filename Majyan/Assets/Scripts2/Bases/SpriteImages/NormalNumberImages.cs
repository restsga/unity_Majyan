using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalNumberImages : SpriteImages
{
    public NormalNumberImages()
    {
        LoadMultipleSprites_SortedId("fonts/numbers");
    }

    public override Sprite GetSprite(int id)
    {
        return sprites[id];
    }
}
