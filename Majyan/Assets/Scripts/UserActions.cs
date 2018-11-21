using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserActions : MonoBehaviour
{
    static public readonly int TI_SELECTING = 1, PON_SELECTING = 2, KAN_SELECTING = 3;

    public delegate void UserInputMethod();
    static public UserInputMethod mouseUpMethod;
    static public UserInputMethod ti_PL;
    static public UserInputMethod pon_PL;
    static public UserInputMethod kan_PL;

    static private int movingCardId = GameManagerScript.NULL_ID;
    static private int movingCardIndex = GameManagerScript.NULL_ID;
    static public int[] handIndexes_forCall = { GameManagerScript.NULL_ID, GameManagerScript.NULL_ID };
    static private int nextIndex_forCall = 0;

    static private bool discardArea = false;
    static private int selecting = GameManagerScript.NULL_ID;

    static private bool playing = true;


    // Update is called once per frame
    void Update()
    {
        if (mouseUpMethod != null)
        {
            if (Input.touchSupported)
            {
                //タッチ対応端末においてタッチが検出されなかった場合
                if (Input.touchCount <= 0)
                {
                    MouseUp();
                }
            }
            else
            {
                //タッチ非対応端末においてクリックが終わったことが検出された場合
                if (Input.GetMouseButtonUp(0))
                {
                    MouseUp();
                }
            }
        }
    }

    static public bool Playing()
    {
        return playing;
    }

    static public bool WantToDiscard()
    {
        return playing && discardArea && movingCardId >= 0;
    }

    static public int GetMovingCardIndex()
    {
        return movingCardIndex;
    }

    static public int GetLatestIndex()
    {
        return handIndexes_forCall[(nextIndex_forCall - 1) % 2];
    }

    static public int GetSelecting()
    {
        return selecting;
    }

    //
    static public void MouseDown(int cardId, int cardIndex, Cards cards)
    {
        movingCardId = cardId;
        movingCardIndex = cardIndex;
        //cards.ShowOrHideHand_Only(0);
    }

    //
    static public void MouseMove(int cursorIndex, Cards cards, ref List<int> playerHand)
    {
        if (movingCardId >= 0&&selecting==GameManagerScript.NULL_ID)
        {
            playerHand.RemoveAt(movingCardIndex);
            playerHand.Insert(cursorIndex, movingCardId);
            movingCardIndex = cursorIndex;

            cards.ShowOrHideHand_Only(0);
        }
    }

    //
    public void MouseUp()
    {
        mouseUpMethod();

        movingCardId = GameManagerScript.NULL_ID;
    }

    static public void MouseClick(int cardIndex,Cards cards)
    {
        if (selecting!=GameManagerScript.NULL_ID) {
            int already = Array.IndexOf(handIndexes_forCall, cardIndex);
            if (already >= 0)
            {
                handIndexes_forCall[already] = GameManagerScript.NULL_ID;
                nextIndex_forCall = already;
            }
            else
            {
                handIndexes_forCall[nextIndex_forCall % 2] = cardIndex;
                nextIndex_forCall++;
            }

            if (selecting == TI_SELECTING)
            {
                ti_PL();
            }
            else if(selecting==PON_SELECTING)
            {
                pon_PL();
            }
            else if (selecting == KAN_SELECTING)
            {
                kan_PL();
            }

            cards.ShowOrHideHand_Only(0);
        }
    }

    public void InDiscardArea()
    {
        discardArea = true;
    }

    public void OutDiscardArea()
    {
        discardArea = false;
    }

    public void OnClickTiButton()
    {
        selecting = TI_SELECTING;
    }
    public void OnClickPonButton()
    {
        selecting = PON_SELECTING;
    }
    public void OnClickKanButton()
    {
        selecting = KAN_SELECTING;
    }

    static public void ResetSelect()
    {
        selecting = GameManagerScript.NULL_ID;

        for (int i = 0; i < handIndexes_forCall.Length; i++)
        {
            handIndexes_forCall[i] = GameManagerScript.NULL_ID;
        }
    }
}
