using System;
using System.Collections.Generic;

public class Rules
{
    //方角idをプレイヤーidに変換
    static public int HouseIdToPlayerId(int startPlayer, int round, int house)
    {
        return (startPlayer + round + house) % 4;
    }
    
}
