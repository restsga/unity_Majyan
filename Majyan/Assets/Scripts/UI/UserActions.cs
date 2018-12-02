using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserActions : MonoBehaviour
{
    static public readonly int TI_SELECTING = 1, PON_SELECTING = 2, KAN_SELECTING = 3,DISCARD_SELECTING=4;

    public delegate void UserInputMethod();
    static public UserInputMethod ti_PL;
    static public UserInputMethod pon_PL;
    static public UserInputMethod kan_PL;
    static public UserInputMethod discard_PL;
    static public UserInputMethod win_PL;

    static private int movingCardId = GameManagerScript.NULL_ID;
    static private int movingCardIndex = GameManagerScript.NULL_ID;
    static public int[] handIndexes_forCall = { GameManagerScript.NULL_ID, GameManagerScript.NULL_ID };
    static private int nextIndex_forCall = 0;

    static private int selecting = GameManagerScript.NULL_ID;

    static public bool canClosedKan = false;
    static public bool canAddKan = false;
    static public bool canOpenKan = false;
    static public bool canPon = false;
    static public bool canTi = false;
    static public bool canDraw = false;
    static public bool canWinCall = false;
    static public bool wantToCallRiichi = false;

    static private bool playing = true;


    // Update is called once per frame
    void Update()
    {
        if (Input.touchSupported)
        {
            //タッチ対応端末においてタッチが検出されなかった場合
            if (Input.touchCount <= 0)
            {
                ResetMoving();
            }
        }
        else
        {
            //タッチ非対応端末においてクリックが終わったことが検出された場合
            if (Input.GetMouseButtonUp(0))
            {
                ResetMoving();
            }
        }
    }

    static public bool Playing()
    {
        return playing;
    }

    static public bool WantToDiscard()
    {
        return playing && handIndexes_forCall[0]>=0;
    }

    static public int GetMovingCardIndex()
    {
        return movingCardIndex;
    }

    static public int GetIndexOnly()
    {
        return handIndexes_forCall[0];
    }

    static public int GetSelecting()
    {
        return selecting;
    }

    static public void SelectingDiscard()
    {
        selecting = DISCARD_SELECTING;
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

    static public void MouseClick(int cardIndex,Cards cards)
    {
        if (selecting!=GameManagerScript.NULL_ID) {
            int already = Array.IndexOf(handIndexes_forCall, cardIndex);
            if (already >= 0)
            {
                if (selecting == DISCARD_SELECTING)
                {
                    discard_PL();
                }
                else
                {
                    handIndexes_forCall[already] = GameManagerScript.NULL_ID;
                    nextIndex_forCall = already;
                }
            }
            else
            {
                if (selecting !=TI_SELECTING)
                {
                    handIndexes_forCall[0] = cardIndex;
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
                else if (selecting == PON_SELECTING)
                {
                    pon_PL();
                }
                else if (selecting == KAN_SELECTING)
                {
                    kan_PL();
                }
            }
            
            movingCardId = GameManagerScript.NULL_ID;
            movingCardIndex = GameManagerScript.NULL_ID;

            cards.ShowOrHideHand_Only(0);
        }
    }

    public void OnClickTiButton()
    {
        if (canTi)
        {
            selecting = TI_SELECTING;
        }
    }
    public void OnClickPonButton()
    {
        if (canPon)
        {
            selecting = PON_SELECTING;
        }
    }
    public void OnClickKanButton()
    {
        if (canOpenKan)
        {
            kan_PL();
        }
        else if (canClosedKan||canAddKan)
        {
            selecting = KAN_SELECTING;
        }
    }

    public void OnClickRiichiButton()
    {
        if (wantToCallRiichi)
        {
            wantToCallRiichi = false;
        }
        else
        {
            wantToCallRiichi = true;
        }

        RiichiButtonColor();
    }

    static private void RiichiButtonColor()
    {
        Color newColor;
        if (wantToCallRiichi)
        {
            newColor = new Color(0f, 1f, 0f, 1f);
        }
        else
        {
            newColor = new Color(1f, 1f, 1f, 1f);
        }
        GameObject.Find("Canvas/Riichi").GetComponentInChildren<Image>().color = newColor;

    }
    public void OnClickWinButton()
    {
        if (canWinCall)
        {
            win_PL();
        }
    }

    static public void ResetSelect()
    {
        selecting = GameManagerScript.NULL_ID;

        for (int i = 0; i < handIndexes_forCall.Length; i++)
        {
            handIndexes_forCall[i] = GameManagerScript.NULL_ID;
        }
    }

    static public void ResetCanCall()
    {
        canClosedKan = false;
        canAddKan = false;
        canOpenKan = false;
        canPon = false;
        canTi = false;
        canDraw = false;
        canWinCall = false;
        wantToCallRiichi = false;
        RiichiButtonColor();
    }

    static public void ResetMoving()
    {
        movingCardId = GameManagerScript.NULL_ID;
        movingCardIndex = GameManagerScript.NULL_ID;
    }
}
