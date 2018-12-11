using System;
using UnityEngine;

abstract public class SpriteImages
{
    protected Sprite[] sprites;

    abstract public Sprite GetSprite(int id);

    //Idで定義したSprite(Multiple)をソートして読み込む
    protected void LoadMultipleSprites_SortedId(string path)
    {
        LoadMultipleSprites_SortedId(path, "");
    }
    protected void LoadMultipleSprites_SortedId(string path, string head)
    {
        Sprite[] raw = Resources.LoadAll<Sprite>(path);
        Sprite[] sorted = new Sprite[raw.Length];
        for (int i = 0; i < sorted.Length; i++)
        {
            string name = head + i;
            sorted[i] = Array.Find<Sprite>(raw, sprite => sprite.name.Equals(name));
        }
        sprites = sorted;
    }
}

static public class ImageObjects
{
    static public CardImages card;
    static public JapaneseImages japaneseImages;
    static public NormalNumberImages normalNumberImages;
    static public MessageImages messages;

    static public void Initialize()
    {
        card = new CardImages();
        japaneseImages = new JapaneseImages();
        normalNumberImages = new NormalNumberImages();
        messages = new MessageImages();
    }
}
