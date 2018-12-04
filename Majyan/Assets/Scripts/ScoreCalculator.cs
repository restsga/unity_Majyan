using System.Linq;
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

    static public int[] CalculateMoveScore
        (int[] fan_array, int bonus, int fu,int parent,int discardPlayer,bool self,int winner,ref int showScore)
    {
        int score_single = CalculateSingleScore(fan_array, bonus, fu);
        showScore = score_single;
        if (parent==winner)
        {
            score_single *= 3;
            score_single /= 2;
        }

        int[] scores = new int[4];
        ArrayBase.ResetArray(scores, 0);
        if (self)
        {
            if (winner== parent)
            {
                for (int i = 0; i < scores.Length; i++)
                {
                    if (i == winner)
                    {
                        scores[i] += score_single;
                    }
                    else
                    {
                        scores[i] -= score_single / 3;
                    }
                }
            }
            else
            {
                for (int i = 0; i < scores.Length; i++)
                {
                    if (i == winner)
                    {
                        scores[i] += score_single;
                    }
                    else
                    {
                        if (i==parent)
                        {
                            scores[i] -= score_single / 2;
                        }
                        else
                        {
                            scores[i] -= score_single / 4;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < scores.Length; i++)
            {
                if (i == winner)
                {
                    scores[i] += score_single;
                }
                else if(i==discardPlayer)
                {
                    scores[i] -= score_single ;
                }
            }
        }

        return scores;
    }

    static public int CalculateSingleScore(int[] fan_array,int bonus,int fu)
    {
        int fan = fan_array.Sum() + bonus;
        int score = 32000;
        if (fan < 13)
        {
            switch (fan)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    score = fu * Calculate.NthPowerOf2(fan + 2) * 4;
                    if (score % 100 != 0)
                    {
                        score = (score / 100 + 1) * 100;
                    }
                    if (score < 1000)
                    {
                        score = 1000;
                    }
                    if (score > 8000)
                    {
                        score = 8000;
                    }
                    break;
                case 5:
                    score = 8000;
                    break;
                case 6:
                case 7:
                    score = 12000;
                    break;
                case 8:
                case 9:
                case 10:
                    score = 16000;
                    break;
                case 11:
                case 12:
                    score = 24000;
                    break;
            }
        }

        return score;
    }
}
