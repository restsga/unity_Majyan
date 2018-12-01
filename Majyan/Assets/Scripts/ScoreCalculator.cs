using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator{

    static public int[] NoReadyPenalty(bool[] ready)
    {
        int readyCount = 0;
        for (int i = 0; i < ready.Length; i++)
        {
            if (ready[i])
            {
                readyCount++;
            }
        }

        int[] scores = new int[ready.Length];
        ArrayBase.ResetArray(scores, 0);
        int add, remove;
        switch (readyCount)
        {

            case 1:
                add = 3000;
                remove = -1000;
                break;
            case 2:
                add = 1500;
                remove = -1500;
                break;
            case 3:
                add = 1000;
                remove = -3000;
                break;
            case 0:
            case 4:
            default:
                add = 0;
                remove = 0;
                break;
        }

        for (int i = 0; i < ready.Length; i++)
        {
            if (ready[i])
            {
                scores[i] = add;
            }
            else
            {
                scores[i] = remove;
            }
        }

        return scores;
    }
}
