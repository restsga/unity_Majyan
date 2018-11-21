using System;
using UnityEngine;

public class LoadSprites{

    static public Sprite[] SortedNumbers_Multiple(string path)
    {
        Sprite[] raw= Resources.LoadAll<Sprite>(path);
        Sprite[] sorted = new Sprite[raw.Length];
        for (int i = 0; i < sorted.Length; i++)
        {
            string number = "" + i;
            sorted[i] = Array.Find<Sprite>(raw, (sprite) => sprite.name.Equals(number));
        }
        return sorted;
    }
}
