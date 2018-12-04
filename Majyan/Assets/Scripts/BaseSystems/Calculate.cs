using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculate{

    static public int NthPowerOf2(int n)
    {
        int value = 2;
        for(int i = 2; i <= n; i++)
        {
            value *= 2;
        }
        return value;
    }
}
