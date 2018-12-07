using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayBase{

    // 配列のリセット //
    //int[]
    static public void ResetArray(int[] array, int value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
    }
    //bool[]
    static public void ResetArray(bool[] array, bool boolean)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = boolean;
        }
    }
    //bool[][]
    static public void ResetArray(bool[][] array,bool boolean)
    {
        for(int i = 0; i < array.Length; i++)
        {
            ResetArray(array[i], boolean);
        }
    }

    //スプライトのアルファ値のリセット
    static public void ResetAlphas(SpriteRenderer[] array,float value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Color newColor = array[i].color;
            newColor.a=value;
            array[i].color = newColor;
        }
    }

    // 二次元以上の配列、配列化したリストの初期化 //
    //bool[][]
    static public void Initialize(bool[][] array,int elementsCount,bool boolean)
    {
        for(int i = 0; i < array.Length; i++)
        {
            array[i] = new bool[elementsCount];
        }
        ResetArray(array, boolean);
    }
    //List<int>[]
    static public void Initialize(List<int>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new List<int>();
        }
        ListClear(array);
    }
    //List<GameObject>[]
    static public void Initialize(List<GameObject>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new List<GameObject>();
        }
        ListClear(array);
    }
    //List<CallCardSet>[]
    static public void Initialize(List<CallCardsSet>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new List<CallCardsSet>();
        }
        ListClear(array);
    }

    //SpriteRendererを読み込み、配列に格納
    static public void Initialize(SpriteRenderer[] array,string path)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = GameObject.Find(path + i).GetComponent<SpriteRenderer>();
        }
    }

    // 配列化されたリストのClear //
    //List<int>[]
    static public void ListClear(List<int>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].Clear();
        }
    }
    //List<GameObject>[]
    static public void ListClear(List<GameObject>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].Clear();
        }
    }
    //List<CallCardsSet>[]
    static public void ListClear(List<CallCardsSet>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].Clear();
        }
    }

    // 元のデータに影響を与えないためのコピーを作成 //
    //int[]
    static public int[] CopyForEdit(int[] array)
    {
        int[] copy = new int[array.Length];
        for(int i = 0; i < array.Length; i++)
        {
            copy[i] = array[i];
        }

        return copy;
    }
    //List<int[]>
    static public List<int[]> CopyForEdit(List<int[]> list)
    {
        List<int[]> copy = new List<int[]>();
        for (int countL = 0; countL < list.Count; countL++)
        {
            int[] array = new int[list[countL].Length];
            for (int countA = 0; countA < list[countL].Length; countA++)
            {
                array[countA] = list[countL][countA];
            }
            copy.Add(array);
        }
        return copy;
    }
}
