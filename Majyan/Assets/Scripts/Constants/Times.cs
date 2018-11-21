using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Times{

    private static float speed = 1.0f;

    public static float Wait_Deal()
    {
        return 0f/speed;
    }
    public static float Wait_DealToDraw()
    {
        return 1.0f/speed;
    }
    public static float Wait_DrawToDiscard()
    {
        return 1.0f/speed;
    }
    public static float Wait_CallToDiscard()
    {
        return 0.5f / speed;
    }
    public static float Wait_Call()
    {
        return 0.9f/speed;
    }
    public static float Wait_NextTurn()
    {
        return 1.0f/speed;
    }
    public static float Wait_HandKan()
    {
        return 0.9f / speed;
    }
    public static float Wait_OpenAddBonus()
    {
        return 0.8f / speed;
    }
    public static float Wait_DrawKanCard()
    {
        return 1.0f / speed;
    }
}
