using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Discard:AI {

    public Draw_Discard()
    {

    }

    public override void DecideDiscard(List<int> hand)
    {
        discard= hand.Count - 1;
    }

    public override bool DecidePon(List<int> hand, ref bool bonus)
    {
        bonus = true;
        return true;
    }

    public override bool DecideKan(List<int> hand)
    {
        return true;
    }

    public override bool DecideTi(List<int> hand, ref int[] indexes,int discard_other)
    {
        List<int> sameGroupCards = new List<int>();
        List<int> indexesOfHand = new List<int>();
        int[,] numbers = { { -2, -1 }, { -1, 1 }, { 1, 2 } };

        if (Rules.IdChangeSerialToCardImageId(discard_other) <= 29)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                if (Rules.SameGroup(hand[i], discard_other))
                {
                    sameGroupCards.Add(hand[i]);
                    indexesOfHand.Add(i);
                }
            }
            sameGroupCards.Sort();
            for (int c = 0; c < 3; c++)
            {
                int check = 0;
                for (int i = 0; i < sameGroupCards.Count; i++)
                {
                    if (Rules.Same_BonusEquate(sameGroupCards[i], discard_other + numbers[c, check] * 4))
                    {
                        indexes[check] = indexesOfHand[i];

                        check++;
                        if (check >= 2)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return true;
    }
}
