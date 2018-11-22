using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//時間経過で実行する関数
public class MethodsTimer
{
    public delegate void Method();
    internal List<Method> methods = new List<Method>();     //関数
    internal List<float> timers = new List<float>();        //タイマー

    //時間経過
    internal void PassTime(float deltaTime)
    {
        if (UserActions.GetSelecting() == GameManagerScript.NULL_ID)
        {
            if (methods.Count >= 1)
            {
                timers[0] -= deltaTime;

                if (timers[0] <= 0f)
                {
                    methods[0]();

                    methods.RemoveAt(0);
                    timers.RemoveAt(0);
                }
            }
        }
    }

    //追加
    internal void AddTimer(Method method, float wait)
    {
            methods.Add(method);
            timers.Add(wait);
    }

    internal void Reset()
    {
        methods.Clear();
        timers.Clear();
    }
}

public class GameManagerScript : MonoBehaviour {

    //メモ(完成後に変更)
    //乱数のseedの取得方法
    //プレイヤーの手牌のソートを無効化
    //プレイヤー情報に合わせての牌表示の真偽値

        //カンドラ実装時に変更
        //嶺上牌を引く

    // オブジェクト //
    // 画像読み込み
    public Sprite[] cardImages = new Sprite[38];        //牌画像

    // クラス
    private Cards cards = new Cards();
    private Scores scores = new Scores();   //得点
    private Phases phases = new Phases();   //進行
    
    //AI
    private AI[] ai = { new Draw_Discard(), new Draw_Discard(), new Draw_Discard(), new Draw_Discard() };

    // 定数 //
    static public readonly int NULL_ID = -1000;  //nullとして扱う数値

    // 変数 //    
    private MethodsTimer methodsTimer = new MethodsTimer();       //一定時間経過後に実行する関数

    private int callPlayer=NULL_ID;
    
    private int seed;       //乱数のseed値(表示用)


    // Use this for initialization
    void Start()
    {
        //Random.SetSeed(DateTime.Now.Millisecond);

        //272:PL明カン→PL加カン
        seed = 272;
        seed = DateTime.Now.Millisecond;    //seed値決定
        Random.SetSeed(10000 + seed);         //seed値を入力

        GameObject.Find("Canvas/SeedText").GetComponent<Text>().text = ""+seed;  //seed値を表示

        CardImages.Initialize();
        UserActions.ti_PL = Ti_PL;
        UserActions.pon_PL = Pon_PL;
        UserActions.kan_PL = Kan_PL;
        UserActions.discard_PL = Discard_PL;

        Initialize_NewGame();       //ゲーム開始時用の初期化処理
    }

    // Update is called once per frame
    void Update()
    {
        methodsTimer.PassTime(Time.deltaTime);      //時間経過
    }

    //ゲーム開始時の初期化
    private void Initialize_NewGame()
    {
        //ゲーム開始時用の初期化処理
        cards.Initialize_NewGame();
        phases.Initialize_NewGame();
        scores.Initialize();
        
        Initialize_NextRound();     //局単位の初期化処理
    }

    //局単位での初期化処理
    private void Initialize_NextRound()
    {
        cards.Initialize_NextRound();
        phases.Initialize_NextRound();

        scores.ShowScores_All(phases.SeatWinds());       //全員分の得点を表示
       
        methodsTimer.AddTimer(Deal, 0f);
    }

    //配牌
    private void Deal()
    {
        cards.Deal();

        methodsTimer.AddTimer(DrawCard, Times.Wait_DealToDraw());
    }

    //牌を引く
    private void DrawCard()
    {
        int actionId= cards.DrawCard(phases.GetTurn(),ai);

        if (phases.GetTurn() != 0 || UserActions.Playing()==false)
        {
            if (actionId == AI.DISCARD)
            {
                methodsTimer.AddTimer(Discard, Times.Wait_DrawToDiscard());
            }
            else if (actionId == AI.CLOSED_KAN)
            {
                methodsTimer.AddTimer(ClosedKan, Times.Wait_HandKan());
            }
            else if (actionId == AI.ADD_KAN)
            {
                methodsTimer.AddTimer(AddKan, Times.Wait_HandKan());
            }
        }

        UserActions.ResetCanCall();
    }

    public void DrawCard_PL()
    {
        if ((phases.GetTurn() + 1) % 4 == 0 && UserActions.Playing()&&
            UserActions.canDraw&&UserActions.GetSelecting()==NULL_ID)
        {
            UserActions.ResetSelect();
            NextTurn();
        }
    }

    //捨て牌
    private void Discard()
    {
        int actionId;

        if (phases.GetTurn() == 0 && UserActions.Playing())
        {
            actionId = cards.Discard(UserActions.GetMovingCardIndex(), 0, ai,ref callPlayer);
        }
        else
        {
            actionId = cards.Discard(ai[phases.GetTurn()].GetDiscardIndex(), phases.GetTurn(), ai,ref callPlayer);
        }

        methodsTimer.AddTimer(OpenAddBonus, Times.Wait_OpenAddBonus());

        if (actionId == AI.OPEN_KAN)
        {
            methodsTimer.AddTimer(OpenKan, Times.Wait_Call());
        }
        else if (actionId == AI.PON)
        {
            methodsTimer.AddTimer(Pon, Times.Wait_Call());
        }
        else if ((phases.GetTurn() + 1) % 4 == 0 && UserActions.Playing())
        {
            UserActions.canDraw = true;
        }
        else
        {
            if (actionId == AI.TI)
            {
                methodsTimer.AddTimer(Ti, Times.Wait_Call());
            }

            else if (actionId == AI.DRAW_CARD)
            {
                methodsTimer.AddTimer(NextTurn, Times.Wait_NextTurn());
            }
        }
    }

    //
    public void Discard_PL()
    {
        if (phases.GetTurn() == 0&&UserActions.WantToDiscard())
        {
            Discard();

            UserActions.ResetSelect();
        }
    }

    //手番移動処理
    private void NextTurn()
    {
        phases.ChangeTurn_Default();
        DrawCard();     //牌を引く
    }

    //チー
    private void Ti()
    {
        int tiPlayer = (phases.GetTurn() + 1) % 4;
        if (tiPlayer == 0 && UserActions.Playing())
        {
            if(cards.Ti(phases.GetTurn(), UserActions.handIndexes_forCall) == false)
            {
                return;
            }

            UserActions.ResetSelect();
            methodsTimer.Reset();

            phases.ChangeTurn_Default();

            UserActions.SelectingDiscard();
        }
        else
        {
            cards.Ti(phases.GetTurn(), ai[tiPlayer].GetCallIndexesForTi());
            phases.ChangeTurn_Default();
            methodsTimer.AddTimer(Discard, Times.Wait_DrawToDiscard());
        }

        UserActions.ResetCanCall();
    }

    public void Ti_PL()
    {
        if(((phases.GetTurn() + 1) % 4 == 0 && UserActions.Playing()))
        {
            Ti();
        }
    }
    //ポン
    private void Pon()
    {
        if (callPlayer == 0 && UserActions.Playing())
        {
            if(cards.Pon(phases.GetTurn(), callPlayer, AI.Bonus5(UserActions.GetIndexOnly()))==false)
            {
                return;
            }

            methodsTimer.Reset();

        }
        else
        {
            cards.Pon(phases.GetTurn(), callPlayer, ai[callPlayer].GetUseBonusCardForPon());
            methodsTimer.AddTimer(Discard, Times.Wait_DrawToDiscard());
        }
        phases.ChangeTurn_Call(callPlayer);
        UserActions.ResetCanCall();
        UserActions.ResetSelect();

        if (callPlayer == 0 && UserActions.Playing())
        {
            UserActions.SelectingDiscard();
        }

    }

    public void Pon_PL()
    {
        if (UserActions.Playing())
        {
            callPlayer = 0;
            Pon();
        }
    }

    //暗カン
    private void ClosedKan()
    {
        if (phases.GetTurn() == 0 && UserActions.Playing())
        {
            if( cards.ClosedKan(phases.GetTurn(), UserActions.GetIndexOnly())==false)
            {
                return;
            }
        }
        else
        {
            cards.ClosedKan(phases.GetTurn(), ai[phases.GetTurn()].GetCallIndex());
        }
        methodsTimer.AddTimer(OpenAddBonus,Times.Wait_OpenAddBonus());
        methodsTimer.AddTimer(DrawKanCard, Times.Wait_DrawKanCard());

        UserActions.ResetCanCall();
        UserActions.ResetSelect();
    }

    //加カン
    private void AddKan()
    {
        if (phases.GetTurn() == 0 && UserActions.Playing())
        {
            if(cards.AddKan(phases.GetTurn(), UserActions.GetIndexOnly()) == false)
            {
                return;
            }
        }
        else
        {
            cards.AddKan(phases.GetTurn(), ai[phases.GetTurn()].GetCallIndex());
        }

        methodsTimer.AddTimer(DrawKanCard, Times.Wait_DrawKanCard());
        UserActions.ResetCanCall();
        UserActions.ResetSelect();
    }
    
    //明カン
    private void OpenKan()
    {
        cards.OpenKan(phases.GetTurn(), callPlayer);
        phases.ChangeTurn_Call(callPlayer);

        methodsTimer.AddTimer(DrawKanCard, Times.Wait_DrawKanCard());

        UserActions.ResetCanCall();
        UserActions.ResetSelect();
    }

    public void Kan_PL()
    {
        if (UserActions.canOpenKan)
        {
            callPlayer = 0;
            OpenKan();
        }
        else if(UserActions.canClosedKan|| UserActions.canAddKan)
        {
            callPlayer = 0;
            ClosedKan();
            AddKan();
        }
    }

    //ドラ表示牌をめくる
    private void OpenAddBonus()
    {
        cards.OpenAddBonusCard();
    }

    //嶺上牌を引く
    private void DrawKanCard()
    {
        cards.DrawKanCard(phases.GetTurn(),ai);

        if (phases.GetTurn() == 0 && UserActions.Playing())
        {
            UserActions.SelectingDiscard();
        }
        else
        {
            methodsTimer.AddTimer(Discard, Times.Wait_DrawToDiscard());
        }
    }
    
    //牌のゲームオブジェクトを削除
    static public void DestroyGameObjects(ref List<GameObject> gameObjects)
    {
        for (int i = gameObjects.Count - 1; 0 <= i; i--)
        {
            Destroy(gameObjects[i]);
        }

        gameObjects.Clear();    //ゲームオブジェクトの格納変数を初期化
    }
}

