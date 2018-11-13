﻿using System;

public class Rules{

    static private int[] serialToCard = {0,0,0,0,1,1,1,1,2,2,2,2,3,3,3,3,4,5,5,5,6,6,6,6,7,7,7,7,8,8,8,8,9,9,9,9,
    10,10,10,10,11,11,11,11,12,12,12,12,13,13,13,13,14,15,15,15,16,16,16,16,17,17,17,17,18,18,18,18,19,19,19,19,
    20,20,20,20,21,21,21,21,22,22,22,22,23,23,23,23,24,25,25,25,26,26,26,26,27,27,27,27,28,28,28,28,29,29,29,29,
    30,30,30,30,31,31,31,31,32,32,32,32,33,33,33,33,34,34,34,34,35,35,35,35,36,36,36,36};

    static private string[] compass = { "東", "南", "西", "北" };

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    static public int IdChangeSerialToCard(int serialNumber)
    {
        return serialToCard[serialNumber];
    }

    static public string CountsToString(int round,int parentCount,int betCount)
    {
        return compass[round / 4] + (round % 4 + 1) + "局" + Environment.NewLine +
            "　　  ×" + parentCount + Environment.NewLine +
            "　　  ×" + betCount;
    }

    static public string PlayerIdToHouseString(int startPlayer, int round, int player)
    {
        return compass[(startPlayer+round+player)%4];
    }
}
