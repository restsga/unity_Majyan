using System;
using System.Collections.Generic;

public class Rules
{

    static private int[] serialToCardImageId = {0,0,0,0,1,1,1,1,2,2,2,2,3,3,3,3,4,5,5,5,6,6,6,6,7,7,7,7,8,8,8,8,9,9,9,9,
    10,10,10,10,11,11,11,11,12,12,12,12,13,13,13,13,14,15,15,15,16,16,16,16,17,17,17,17,18,18,18,18,19,19,19,19,
    20,20,20,20,21,21,21,21,22,22,22,22,23,23,23,23,24,25,25,25,26,26,26,26,27,27,27,27,28,28,28,28,29,29,29,29,
    30,30,30,30,31,31,31,31,32,32,32,32,33,33,33,33,34,34,34,34,35,35,35,35,36,36,36,36};
    
    static public int IdChangeSerialToCardImageId(int serialNumber)
    {
        return serialToCardImageId[serialNumber];
    }

    static public int PlayerIdToHouseId(int startPlayer, int round, int player)
    {
        return (Math.Abs(startPlayer - (player+4)) + round ) % 4;
    }

    static public int HouseIdToPlayerId(int startPlayer, int round, int house)
    {
        return (startPlayer + round + house) % 4;
    }

    static public bool[] CanPonOrKan(List<int> hand, int discard)
    {
        int count = 1;

        for (int i = 0; i < hand.Count; i++)
        {
            if(Same_BonusEquate(hand[i],discard))
            {
                count++;
            }
        }

        bool[] pon_kan = new bool[2];

        if (count == 3)
        {
            pon_kan[0] = true;
            pon_kan[1] = false;
        }
        else if (count == 4)
        {
            pon_kan[0] = true;
            pon_kan[1] = true;
        }
        else
        {
            pon_kan[0] = false;
            pon_kan[1] = false;
        }
        return pon_kan;
    }

    static public bool Same_BonusEquate(int card1,int card2)
    {
        return card1/4==card2/4;
    }
}
