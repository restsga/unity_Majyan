using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageImages : SpriteImages
{
    static public readonly Dictionary<string, int> messageAndIndex = new Dictionary<string, int>()
        { {"ポン",0},{"チー",1},{"カン",2},{"リーチ",3},{"ツモ",4},
        { "ロン",5},{"流局",6},{"テンパイ",7},{"ノーテン",8} };
    static public readonly Dictionary<string, float> messageAndTime = new Dictionary<string, float>()
        { {"ポン",2f},{"チー",2f},{"カン",2f},{"リーチ",2.5f},{"ツモ",2.5f},
        { "ロン",2.5f},{"流局",2.5f},{"テンパイ",3f},{"ノーテン",3f} };

    public MessageImages()
    {
        LoadMultipleSprites_SortedId("messages");
    }

    public override Sprite GetSprite(int id)
    {
        return sprites[id];
    }
}
