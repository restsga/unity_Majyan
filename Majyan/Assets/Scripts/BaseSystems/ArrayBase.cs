using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayBase{

    static public void ResetArray(int[] array)
    {
        ResetArray(array, 0);
    }
    static public void ResetArray(int[] array, int value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
    }
    static public void ResetArray(bool[] array)
    {
        ResetArray(array, false);
    }
    static public void ResetArray(bool[] array, bool boolean)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = boolean;
        }
    }
    static public void ResetArray(bool[][] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            ResetArray(array[i], false);
        }
    }
    static public void ResetArray(bool[][] array,bool boolean)
    {
        for(int i = 0; i < array.Length; i++)
        {
            ResetArray(array[i], boolean);
        }
    }
    static public void ResetAlphas(SpriteRenderer[] array,float value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Color newColor = array[i].color;
            newColor.a=value;
            array[i].color = newColor;
        }
    }

    static public void Initialize(bool[][] array, int elementsCount)
    {
        Initialize(array, elementsCount, false);
    }
    static public void Initialize(bool[][] array,int elementsCount,bool boolean)
    {
        for(int i = 0; i < array.Length; i++)
        {
            array[i] = new bool[elementsCount];
        }
        ResetArray(array, boolean);
    }
    static public void Initialize(List<int>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new List<int>();
        }
        ListClear(array);
    }
    static public void Initialize(List<GameObject>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new List<GameObject>();
        }
        ListClear(array);
    }
    static public void Initialize(List<CallCardsSet>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new List<CallCardsSet>();
        }
        ListClear(array);
    }
    static public void Initialize(SpriteRenderer[] array,string path)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = GameObject.Find(path + i).GetComponent<SpriteRenderer>();
        }
    }

    static public void ListClear(List<int>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].Clear();
        }
    }
    static public void ListClear(List<GameObject>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].Clear();
        }
    }
    static public void ListClear(List<CallCardsSet>[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].Clear();
        }
    }

    //元のデータに影響を与えないためのコピーを作成
    static public int[] CopyForEdit(int[] array)
    {
        int[] copy = new int[array.Length];
        for(int i = 0; i < array.Length; i++)
        {
            copy[i] = array[i];
        }

        return copy;
    }
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
