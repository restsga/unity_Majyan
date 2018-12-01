using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messages
{
    static public readonly int
        PON = 0, TI = 1, KAN = 2, RIICHI = 3, WIN_SELF = 4,
        WIN_DISCARD = 5, DRAWN_GAME = 6, READY = 7, NOT_READY = 8;
    static private readonly float[] TIMES = { 1f, 1f, 1f, 2f, 2f, 2f, 2f, 2.5f, 2.5f };

    static private Sprite[] messages;

    static private SpriteRenderer[] texts = new SpriteRenderer[4];

    static public void Initialize()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i] = GameObject.Find("Messages/Message" + i).GetComponent<SpriteRenderer>();
        }

        messages = LoadSprites.SortedNumbers_Multiple("messages");
    }

    static public void ShowMessage(int messageId,int playerId)
    {
        texts[playerId].sprite = messages[messageId];

        Color newColor = texts[playerId].color;
        newColor.a = Messages.TIMES[messageId];
        texts[playerId].color = newColor;
    }
}
