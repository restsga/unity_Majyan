using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cards {

    // 牌 //
    private Deck deck = new Deck();
    private List<int>[] hands = new List<int>[4];
    private List<int>[] tables = new List<int>[4];
    private List<CallCardsSet>[] calls = new List<CallCardsSet>[4];
    private bool[][] waitingCards = new bool[4][];

    // ステータス //
    private bool[] ready = new bool[4];
    private bool[] riichi = new bool[4];
    private bool[] ippatsu = new bool[4];
    private bool[] doubleRiichi = new bool[4];
    private bool[] winner = new bool[4];

    // オブジェクト //
    private List<GameObject>[] handObjects = new List<GameObject>[4];       //手牌
    private List<GameObject>[] tableObjects = new List<GameObject>[4];      //捨て牌
    private List<GameObject>[] callObjects = new List<GameObject>[4];       //鳴き牌
    private SpriteRenderer[] riichiStick = new SpriteRenderer[4];
    
    public Cards()
    {
        // 配列に格納したリストの実体化 //
        ArrayBase.Initialize(hands);
        ArrayBase.Initialize(tables);
        ArrayBase.Initialize(calls);
        ArrayBase.Initialize(handObjects);
        ArrayBase.Initialize(tableObjects);
        ArrayBase.Initialize(callObjects);

        // 配列の初期化 //
        ArrayBase.Initialize(waitingCards,34,false);
    }

    //新規に対戦を開始するための初期化
    public void Initialize_NewGame()
    {
        ArrayBase.Initialize(riichiStick, "RiichiSticks/Stick");

        deck.Initialize_NewGame();

        Initialize_NextRound();
    }

    //ラウンド開始のための初期化
    public void Initialize_NextRound()
    {
        deck.Initialize_NextRound();

        ArrayBase.ListClear(hands);
        ArrayBase.ListClear(tables);
        ArrayBase.ListClear(calls);

        ArrayBase.ResetArray(waitingCards,false);
        ArrayBase.ResetArray(ready, false);
        ArrayBase.ResetArray(riichi, false);
        ArrayBase.ResetArray(ippatsu, false);
        ArrayBase.ResetArray(doubleRiichi, false);
        ArrayBase.ResetArray(winner, false);
        ArrayBase.ResetAlphas(riichiStick, 0f);

        ShowAndHideHands_Default();
        ShowTableCards_All(false);
        ShowCallCard_All(false);
    }

    //配牌
    public void Deal()
    {
        //各プレイヤーに13枚ずつ牌を配る
        for (int p = 0; p < hands.Length; p++)
        {
            for (int i = 0; i < 13; i++)
            {
                //山の一番上の牌を手牌として取得
                hands[p].Add(deck.DrawCard());   
            }

            //AIの手牌をソート
            if (p != 0 || UserActions.Playing() == false)
            {
                hands[p].Sort();
            }

            //手牌を表示
            ShowOrHideHand_Only(p); 
        }
    }

    //牌を引く(牌山)
    public int DrawDeckCard()
    {
        int turn = GameManagerScript.phases.GetTurn();
        return DrawCard(turn, GameManagerScript.ai, true);
    }
    //牌を引く(嶺上牌)
    public int DrawKanCard()
    {
        int turn = GameManagerScript.phases.GetTurn();
        return DrawCard(turn, GameManagerScript.ai, false);
    }
    //牌を引く(牌山、嶺上牌共通)
    private int DrawCard(int turn, AI[] ai,bool draw_deck)
    {
        if (draw_deck)
        {
            //牌山から牌を引く
            hands[turn].Add(deck.DrawCard());
        }
        else
        {
            //嶺上牌を引く
            hands[turn].Add(deck.DrawKanCard());
        }
        //手番プレイヤーの手牌を表示
        ShowOrHideHand_Only(turn);

        /* 条件を全て満たす場合は勝利宣言可能
         * 
         * 引いた牌が待ち牌
         * ドラを除く役が1つ以上ある
         */
        bool canWinCall=false;
        int winCard = hands[turn][hands[turn].Count - 1] / 2;
        if (waitingCards[turn][winCard])
        {
            Phases phases = GameManagerScript.phases;

            YakuJudgementDatas judgementDatas = new YakuJudgementDatas
                (hands[turn], calls[turn],winCard, phases.FieldWind(), phases.PlayerIdToSeatWind(turn),
                riichi[turn], ippatsu[turn], doubleRiichi[turn], true,deck.IsExhaustionDeck(),draw_deck==false);

            int fu = 0;
            int[] yaku = Yaku.Judgements(judgementDatas,ref fu);

            if (Array.FindIndex<int>(yaku, n => n >= 1) >= 0)
            {
                canWinCall = true;
            }
        }

        if (turn == 0 && UserActions.Playing())
        {
            /* プレイヤーの手番の場合
             * 
             * 入力待ちモードに移行
             * 勝利宣言、暗槓、加槓が出来るならフラグを立てる
             */

            UserActions.SelectingDiscard();

            if (canWinCall)
            {
                UserActions.canWinCall = true;
            }
            if (AI.CanClosedKan(hands[0]).Count >= 1)
            {
                UserActions.canClosedKan = true;
            }
            if (AI.CanAddKan(hands[0], calls[0]).Count >= 1)
            {
                UserActions.canAddKan = true;
            }

            return AI.WAIT_INPUT;
        }
        else
        {
            //AIの手番の場合、行動を決定して終了
            if (canWinCall&&ai[turn].DecideWin_SelfDraw(hands[turn]))
            {
                return AI.WIN_SELF_DRAW;
            }
            return ai[turn].DecideDiscardOrKan(hands[turn], calls[turn]);
        }
    }

    //捨て牌
    public int Discard(int discardIndex)
    {
        Scores scoresClass = GameManagerScript.scores;
        Phases phases = GameManagerScript.phases;
        AI[] ai = GameManagerScript.ai;
        int turn = phases.GetTurn();

        //立直している時は強制的にツモ切り
        if (riichi[turn])
        {
            discardIndex = hands[turn].Count - 1;
        }

        /* 捨て牌をする
         * 
         * 河に追加して手牌から削除する
         */
        int discard = hands[turn][discardIndex];
        tables[turn].Add(discard);
        hands[turn].RemoveAt(discardIndex);

        //AIの手牌をソート
        if (turn != 0 || UserActions.Playing() == false)
        {
            hands[turn].Sort();
        }

        //手牌と河の再表示
        ShowOrHideHand_Only(turn);
        ShowTableCard_Only(turn, false);

        //聴牌で門前かつ立直していない場合は立直可能
        waitingCards[turn] = AI.ReadyAndWaiting(hands[turn], ref ready[turn]);
        if (ready[turn] && riichi[turn] == false)
        {
            if (Yaku.IsThisClosedHand(calls[turn]))
            {
                bool callRiichi = false;
                if (turn == 0 && UserActions.Playing())
                {
                    if (UserActions.wantToCallRiichi)
                    {
                        //プレイヤーが立直しようとしている場合
                        callRiichi = true;
                    }
                }
                else
                {
                    if (ai[turn].GetCallRiichi())
                    {
                        //AIが立直しようとしている場合
                        callRiichi = true;
                    }
                }

                if (callRiichi)
                {
                    /* 立直する場合
                     * 
                     * 1000点減らす
                     * 立直フラグを立てる
                     * 一発フラグを立てる
                     * 1000点棒を表示
                     * 立直のメッセージを表示
                     */
                    if (scoresClass.Riichi(turn, phases))
                    {
                        riichi[turn] = true;
                        ippatsu[turn] = true;
                        riichiStick[turn].color = new Color(1f, 1f, 1f, 1f);
                        Messages.ShowMessage(Messages.RIICHI, turn);
                    }
                }
            }
        }

        //プレイヤーからの入力フラグ、実行可能行動フラグをリセット
        UserActions.ResetCanCall();

        if (deck.IsExhaustionDeck())
        {
            //牌山が無くなった場合は流局
            return AI.DRAWN_GAME;
        }

        // ポン、カン //

        //プレイヤーが遊んでいるならIDが0の競技者を行動決定対象から除外
        int playing = 0;
        if (UserActions.Playing())
        {
            playing = 1;
        }
        //ポン、カンの有無を決定
        for (int i = 0 + playing; i < hands.Length; i++)
        {
            //捨て牌をした競技者、立直中の競技者が鳴くことは不可
            if (turn != i && riichi[i] == false)
            {
                //鳴きの有無を取得
                int actionId = ai[i].DecideCallKanOrPon(hands[i], discard);
                if (actionId != AI.NOT_CALL)
                {
                    //鳴きを行う場合は競技者IDを保存
                    GameManagerScript.callPlayer = i;
                    //ポン、カンは衝突せず、チーより優先されるので他の競技者についての処理は不要
                    return actionId;
                }
            }
        }
        //プレイヤーが遊んでいるならプレイヤーによる鳴きが可能かを決定する
        if (UserActions.Playing() && riichi[0] == false)
        {
            //手牌内の捨て牌と同一の牌の枚数
            int sameCount = AI.CanCallKanOrPon(hands[0], discard).Count;

            if (sameCount >= 3 - 1)
            {
                //ポン可能
                UserActions.canPon = true;

                if (sameCount >= 4 - 1)
                {
                    //カンも可能
                    UserActions.canOpenKan = true;
                }
            }
        }

        // チー //

        //チーを出来る競技者のID
        int tiPlayer = (turn + 1) % 4;
        if (tiPlayer == 0 && UserActions.Playing())
        {
            if (riichi[tiPlayer] == false)
            {
                /* チーを出来る競技者がプレイヤーだった場合
                 * 
                 * チーが可能な手牌ならフラグを立てる
                 */
                if (AI.CanCallTi(hands[0], discard).Count >= 1)
                {
                    UserActions.canTi = true;
                }
            }
            return AI.WAIT_INPUT;
        }
        else
        {
            /* チーを出来る競技者がAIだった場合
             * 
             * 立直している場合の選択肢は牌を引くのみ
             * 立直していない場合は牌山から牌を引くかチーをするかを決定
             */
            if (riichi[tiPlayer])
            {
                return AI.DRAW_CARD;
            }
            else
            {
                return ai[tiPlayer].DecideDrawCardOrTi(hands[tiPlayer], discard);
            }
        }
    }

    //チー
    public bool Ti(int[] indexes_raw)
    {
        int turn = GameManagerScript.phases.GetTurn();
        
        //直前の捨て牌、チーをする競技者のID、公開しようとしている牌を取得
        int discard = tables[turn][tables[turn].Count - 1];
        int tiPlayer = (turn + 1) % 4;
        int[] indexes = ArrayBase.CopyForEdit(indexes_raw);

        Array.Sort(indexes);

        if (AI.IsThisSetCanTi(hands[tiPlayer], discard, indexes)==false)
        {
            //公開しようとしている牌ではチーが出来ない場合は処理を行わずに終了
            return false;
        }

        //格納(表示)用に並び順を反転
        Array.Reverse(indexes);

        /* 鳴き牌のデータを生成
         * 
         * 手牌から公開する牌を保存
         * 公開する牌を手牌から削除
         * 直前の捨て牌を河から削除
         * 鳴き牌を生成し、チーとして格納
         */
        int[] cards = new int[2];   
        for (int i = 0; i < indexes.Length; i++)
        {
            cards[i] = hands[tiPlayer][indexes[i]];     
            hands[tiPlayer].RemoveAt(indexes[i]);       
        }
        tables[turn].RemoveAt(tables[turn].Count - 1);
        calls[tiPlayer].Add(new CallCardsSet());    
        calls[tiPlayer][calls[tiPlayer].Count - 1].Ti(cards,discard, tiPlayer, turn); 

        //手牌、河、鳴き牌を再表示
        ShowOrHideHand_Only(tiPlayer);
        ShowTableCard_Only(turn,false);     
        ShowCallCard_Only(tiPlayer,false);

        //メッセージを表示
        Messages.ShowMessage(Messages.TI,tiPlayer);

        //処理の完了を通知
        return true;
    }

    //ポン
    public bool Pon(bool bonus)
    {
        int turn = GameManagerScript.phases.GetTurn();
        int callPlayer = GameManagerScript.callPlayer;

        //捨て牌を取得
        int discard = tables[turn][tables[turn].Count - 1];

        if (callPlayer == 0 && UserActions.Playing())
        {
            if (AI.Same(discard, hands[0][UserActions.GetIndexOnly()]) == false)
            {
                //プレイヤーが公開しようとしている牌と捨て牌の種類が一致しない場合は処理を行わずに終了
                return false;
            }
        }

        //選択肢が複数ある場合に手牌から1つを公開用として扱わないようにするためのフラグ
        bool ignore = false;
        if (AI.CanCallKanOrPon(hands[callPlayer], discard).Count >= 4 - 1)
        {
            ignore = true;
        }

        //公開する牌とそのカウンタ
        int[] cards = new int[2];   
        int count = 0;      

        for (int i = hands[callPlayer].Count - 1; 0 <= i; i--)
        {
            if (AI.Same(hands[callPlayer][i], discard))   //捨て牌と同じ牌の場合
            {
                if ((ignore && bonus == false && AI.Bonus5(hands[callPlayer][i])) ||
                    ignore && bonus && AI.Bonus5(hands[callPlayer][i]) == false)
                {
                    //選択肢が複数ある場合で公開対象から除外する牌の場合

                    ignore = false;
                }
                else
                {
                    /* 選択肢が単一または除外対象の牌ではない場合
                     * 
                     * 公開する牌を格納して手牌から取り除く
                     * 探索カウンタを増加、格納用配列が埋まった場合は格納処理を終了
                     */
                    cards[count] = hands[callPlayer][i];     
                    hands[callPlayer].RemoveAt(i);           
                    count++;                        
                    if (count >= 2)
                    {
                        break;
                    }
                }
            }
        }

        //捨て牌を河から削除
        tables[turn].RemoveAt(tables[turn].Count - 1);

        //ポンとして格納
        calls[callPlayer].Add(new CallCardsSet());   
        calls[callPlayer][calls[callPlayer].Count - 1].Pon(cards,discard, callPlayer, turn);

        //手牌、河、鳴き牌を再表示
        ShowOrHideHand_Only(callPlayer);    
        ShowTableCard_Only(turn,false);     
        ShowCallCard_Only(callPlayer,false);

        //メッセージを表示
        Messages.ShowMessage(Messages.PON,callPlayer);

        return true;
    }

    public bool ClosedKan(int kanIndex)
    {
        int turn = GameManagerScript.phases.GetTurn();

        if ((AI.CanClosedKan(hands[turn]).FindIndex
            (index => AI.Same(hands[turn][index], hands[turn][kanIndex]))>= 0)==false)
        {
            return false;
        }

        int[] callCard = new int[4];   //カンに用いる牌の格納用配列
        int count = 0;  //格納カウンタ

        for (int i = hands[turn].Count - 1; 0 <= i; i--)
        {
            if (AI.Same(hands[turn][i], hands[turn][kanIndex]))
            {
                //指定の牌と同じの場合は格納用配列に格納
                callCard[count] = hands[turn][i];
                count++;
                if (count >= 4)
                {
                    break;
                }
            }
        }

        hands[turn].RemoveAll((card) => card == callCard[0]);

        calls[turn].Add(new CallCardsSet());  //鳴き牌を追加
        calls[turn][calls[turn].Count - 1].ClosedKan(callCard, turn); //暗カンとして格納

        ShowOrHideHand_Only(turn);        //手牌を表示
        ShowCallCard_Only(turn,false);          //鳴き牌を表示

        deck.AddPendingShowBonusCount();

        Messages.ShowMessage(Messages.KAN,turn);

        return true;
    }

    //加カン
    public bool AddKan(int kanIndex)
    {
        int turn = GameManagerScript.phases.GetTurn();

        if ((AI.CanAddKan(hands[turn], calls[turn]).FindIndex
            (index => AI.Same(hands[turn][index], hands[turn][kanIndex]))>= 0)==false)
        {
            return false;
        }

        OpenAddBonusCard();

        int callCard = hands[turn][kanIndex];
        hands[turn].RemoveAt(kanIndex);

        int addIndexOfCalls=GameManagerScript.NULL_ID;
        for (int i = 0; i < calls[turn].Count; i++)
        {
            if (calls[turn][i].CanAddKan(callCard))
            {
                addIndexOfCalls = i;
            }
        }
        calls[turn][addIndexOfCalls].AddKan(callCard, turn); //加カンとして格納

        ShowOrHideHand_Only(turn);        //手牌を表示
        ShowCallCard_Only(turn,false);          //鳴き牌を表示

        deck.AddPendingShowBonusCount();

        Messages.ShowMessage(Messages.KAN,turn);

        return true;
    }

    //明カン
    public void OpenKan()
    {
        int turn = GameManagerScript.phases.GetTurn();
        int callPlayer = GameManagerScript.callPlayer;

        int discard = tables[turn][tables[turn].Count - 1];

        OpenAddBonusCard();

        int[] cards = new int[3];       //鳴き牌格納用
        int count = 0;      //鳴き牌探索カウンタ

        for (int i = hands[callPlayer].Count - 1; 0 <= i; i--)
        {
            if (AI.Same(hands[callPlayer][i], discard))   //捨て牌と同じ牌の場合
            {
                cards[count] = hands[callPlayer][i];     //鳴き牌として格納
                hands[callPlayer].RemoveAt(i);           //手牌から取り除く
                count++;                        //探索カウンタを増加
            }
        }

        tables[turn].RemoveAt(tables[turn].Count - 1);          //捨て牌から取り除く
        calls[callPlayer].Add(new CallCardsSet());       //鳴き牌を追加
        //カンとして格納
        calls[callPlayer][calls[callPlayer].Count - 1].OpenKan(cards,discard, callPlayer, turn);

        ShowOrHideHand_Only(callPlayer);    //手牌を表示
        ShowTableCard_Only(turn,false);     //捨て牌を表示
        ShowCallCard_Only(callPlayer,false);      //鳴き牌を表示

        deck.AddPendingShowBonusCount();

        Messages.ShowMessage(Messages.KAN,callPlayer);
    }

    public void OpenAddBonusCard()
    {
        deck.AddBonusCard();
    }
    
    public void DrawnGame_Confirm()
    {
        for (int i = 0; i < 4; i++)
        {
            Messages.ShowMessage(Messages.DRAWN_GAME,i);
        }
        ShowAndHideHands_DrawnGame();
        ShowTableCards_All(true);
        ShowCallCard_All(true);
        deck.EndRound_ShowBonusAlpha();
    }

    public void DrawnGame_ReadyOrNot()
    {
        Scores scoresClass = GameManagerScript.scores;
        Phases phases = GameManagerScript.phases;

        for (int i = 0; i < ready.Length; i++)
        {
            if (ready[i])
            {
                Messages.ShowMessage(Messages.READY,i);
            }
            else
            {
                Messages.ShowMessage(Messages.NOT_READY,i);
            }
        }

        scoresClass.AddOrRemoveScore(ScoreCalculator.NoReadyPenalty(ready));

        int betCount = 0;
        for(int i = 0; i < riichi.Length; i++)
        {
            if (riichi[i])
            {
                betCount++;
            }
        }
        phases.EndRound(ready[phases.HouseIdToPlayerId(0)],true,betCount);
    }

    public void WinSelfDraw()
    {
        int turn = GameManagerScript.phases.GetTurn();
        ShowHand_Only(turn, true, false,true);
        winner[turn] = true;
        Messages.ShowMessage(Messages.WIN_SELF, turn);
    }

    public void ShowRiichiBonus()
    {
        for(int i = 0; i < winner.Length; i++)
        {
            if (winner[i])
            {
                if (winner[i] == riichi[i])
                {
                    deck.ShowRiichiBonus();
                    break;
                }
            }
        }
    }

    //プレイヤー1名分の手牌を表示(基底関数)
    private void ShowHand_Only(int player, bool show,bool drawnGame,bool stop)
    {
        GameManagerScript.DestroyGameObjects(ref handObjects[player]);    //表示している手牌のゲームオブジェクトを削除
        for (int i = 0; i < hands[player].Count; i++)
        {
            GameObject card = new GameObject();
            Sprite sprite;
            int handCard = hands[player][i];
            if (show)
            {
                sprite =
                    CardImages.Image_Front(handCard);    //表向きの牌画像
            }
            else
            {
                sprite = CardImages.Image_Back();    //裏向きの牌画像
            }

            card.AddComponent<SpriteRenderer>().sprite = sprite;        //牌画像を格納
            card.transform.localScale = Layouts.handScales[player];     //大きさを決定
            card.transform.position = Layouts.handOffsets[player] + Layouts.handLineupDirections[player] * i;
            card.transform.rotation =
                Quaternion.Euler(Layouts.handRotations[player]);    //角度を決定

            Color newColor = new Color(1f,1f,1f,1f);
            
            if (drawnGame)
            {
                newColor.a = 0.3f;
            }

            if (player == 0 && UserActions.Playing()&&stop==false)
            {
                card.AddComponent<BoxCollider2D>();

                int index = i;
                for (int j = 0; j < UserActions.handIndexes_forCall.Length; j++)
                {
                    if (index == UserActions.handIndexes_forCall[j])
                    {
                        newColor.a = 0.5f;
                    }
                }

                EventTrigger.Entry down = new EventTrigger.Entry();
                down.eventID = EventTriggerType.PointerDown;
                down.callback.AddListener((f) => UserActions.MouseDown(handCard, index, this));
                card.AddComponent<EventTrigger>().triggers.Add(down);

                EventTrigger.Entry drag = new EventTrigger.Entry();
                drag.eventID = EventTriggerType.PointerEnter;
                drag.callback.AddListener((f) => UserActions.MouseMove(index, this, ref hands[0]));
                card.GetComponent<EventTrigger>().triggers.Add(drag);

                EventTrigger.Entry click = new EventTrigger.Entry();
                click.eventID = EventTriggerType.PointerClick;
                click.callback.AddListener((f) => UserActions.MouseClick(index,this));
                card.GetComponent<EventTrigger>().triggers.Add(click);
            }

            card.GetComponent<SpriteRenderer>().color = newColor;

            handObjects[player].Add(card);      //メモリ解放用のリストに格納
        }
    }

    //プレイヤー情報(1名分)に合わせて牌の表裏を考慮して手牌を表示(派生関数)
    public void ShowOrHideHand_Only(int player)
    {
        bool[] shows = { true, true, true, true };      //人間以外の手牌は裏向き

        ShowHand_Only(player, shows[player],false,false);           //真偽値に応じた牌の向きで表示
    }

    //全員分の手牌を表裏を考慮して表示(派生関数)
    private void ShowAndHideHands(bool[] shows)
    {
        for (int i = 0; i < hands.Length; i++)
        {
            ShowHand_Only(i, shows[i],false,false);
        }
    }

    //全員分の手牌を表向きで表示
    private void ShowHands_All()
    {
        bool[] shows = { true, true, true, true };

        ShowAndHideHands(shows);    //真偽値リストに基づいて表示
    }

    //プレイヤー情報に合わせて牌の表裏を考慮して全員分の手牌を表示(派生関数)
    private void ShowAndHideHands_Default()
    {
        bool[] shows = { true, false, false, false };

        ShowAndHideHands(shows);    //真偽値リストに基づいて表示
    }

    //流局時の手牌表示(派生関数)
    private void ShowAndHideHands_DrawnGame()
    {
        for (int i = 0; i < hands.Length; i++)
        {
            ShowHand_Only(i,ready[i] , true,true);
        }
    }

    //プレイヤー1名分の捨て牌を表示(基底関数)
    private void ShowTableCard_Only(int player,bool alpha)
    {
        GameManagerScript.DestroyGameObjects(ref tableObjects[player]);       //捨て牌のゲームオブジェクトを削除

        for (int i = 0; i < tables[player].Count; i++)
        {
            GameObject card = new GameObject();
            Sprite sprite = CardImages.Image_Front(tables[player][i]);      //画像を取得
            card.AddComponent<SpriteRenderer>().sprite = sprite;            //画像を格納
            card.transform.localScale = Layouts.tableScales[player];        //大きさを決定
            card.transform.position =
                Layouts.tableOffsets[player] +
                Layouts.tableLineupNextDirections[player] * (i % 6) +
                Layouts.tableLineupNewLineDirections[player] * (i / 6);     //表示場所を決定
            card.transform.rotation =
                Quaternion.Euler(Layouts.tableRotations[player]);           //角度を決定

            if (alpha)
            {
                Color newColor= new Color(1f,1f,1f,1f);
                newColor.a = 0.1f;
                card.GetComponent<SpriteRenderer>().color = newColor;
            }
            tableObjects[player].Add(card);     //メモリ解放用のリストに格納
        }
    }

    //鳴き牌表示
    private void ShowCallCard_Only(int player,bool alpha)
    {
        GameManagerScript.DestroyGameObjects(ref callObjects[player]);    //表示している手牌のゲームオブジェクトを削除

        Vector2 position =
            new Vector2(Layouts.callOffsets[player].x, Layouts.callOffsets[player].y);  //表示位置の初期地点

        for (int s = 0; s < calls[player].Count; s++)
        {
            for (int i = 0; i < calls[player][s].callCards.Count; i++)
            {
                CallCard callCard = calls[player][s].callCards[i];  //牌の情報を取得
                GameObject card = new GameObject();
                Sprite sprite;
                if (callCard.closedKan_hide)
                {
                    sprite = CardImages.Image_Back();   //裏向き
                }
                else
                {
                    sprite = CardImages.Image_Front(callCard.cardId);   //表向き
                }
                card.AddComponent<SpriteRenderer>().sprite = sprite;        //画像を格納
                card.transform.localScale = Layouts.callScales[player];     //大きさを決定

                float addDirection;     //回転した牌のための余白を用意するかどうかのフラグ
                Vector3 addRotation;   //回転した牌の場合に追加する回転量
                Vector2 addY = new Vector2(0f, 0f);          //加カン牌用のずらしフラグ
                if (callCard.discardPlayer == player)
                {
                    //自分の鳴き牌の場合は回転はなし
                    addDirection = 0f;
                    addRotation = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    //他家の捨て牌の場合は回転させる
                    position += Layouts.callLineupRotatedAddDirections[player]; //回転分の座標補正
                    addDirection = 1.0f;    //余白用意フラグを立てる
                    addRotation = new Vector3(0f, 0f, 90f);     //回転させる
                    addY = Layouts.callLineupRotatedAddYPositions[player];      //高さ補正
                }
                if (callCard.addKan)
                {
                    //加カン牌の場合は基本位置xを戻す。yはずらすフラグを立てる
                    position -=
                    Layouts.callLineupDirections[player] +
                    Layouts.callLineupRotatedAddDirections[player];
                    addY =
                        Layouts.callLineupRotatedAddYPositions[player] +
                        Layouts.callLineupAddDoubleYPositions[player];

                    addDirection = 1.0f;    //余白用意フラグを立てる
                    addRotation = new Vector3(0f, 0f, 90f);     //回転させる
                }
                card.transform.position = position + addY; //牌の表示位置を決定
                //回転の有無を考慮して次の牌の基本位置を決定
                position +=
                    Layouts.callLineupDirections[player] +
                    Layouts.callLineupRotatedAddDirections[player] * addDirection;
                card.transform.rotation =
                    Quaternion.Euler(Layouts.callRotations[player] + addRotation);    //牌の角度を決定

                if (alpha)
                {
                    Color newColor= new Color(1f,1f,1f,1f);
                    newColor.a = 0.5f;
                    card.GetComponent<SpriteRenderer>().color = newColor;
                }

                callObjects[player].Add(card);  //メモリ解放用のリストに格納
            }
        }
    }

    private void ShowCallCard_All(bool alpha)
    {
        for(int i = 0; i < calls.Length; i++)
        {
            ShowCallCard_Only(i,alpha);
        }
    }

    //全員分の捨て牌を表示(派生関数)
    public void ShowTableCards_All(bool alpha)
    {
        for (int i = 0; i < tables.Length; i++)
        {
            ShowTableCard_Only(i,alpha);
        }
    }
}

internal class CallCard
{
    //鳴き牌の情報
    public int cardId;
    public int discardPlayer;
    public bool closedKan_hide;
    public bool addKan;

    public CallCard()
    {
        closedKan_hide = false;
        addKan = false;
    }

    public CallCard(int card, int discardPlayer, bool closedKan_hide, bool addKan)
    {
        this.cardId = card;
        this.discardPlayer = discardPlayer;
        this.closedKan_hide = closedKan_hide;
        this.addKan = addKan;
    }
}

public class CallCardsSet
{
    readonly public static int ERROR = 0, TI = 1, PON = 2, KAN = 3;

    internal List<CallCard> callCards = new List<CallCard>();     //鳴き牌による1面子
    private int callKind=ERROR;

    public bool IsThisTi()
    {
        return callKind == TI;
    }
    public bool IsThisPon()
    {
        return callKind == PON;
    }
    public bool IsThisKan()
    {
        return callKind == KAN;
    }
    public bool IsThisClosedKan()
    {
        if (IsThisKan()==false)
        {
            return false;
        }
        for (int i = 1; i < callCards.Count; i++)
        {
            if (callCards[i - 1].discardPlayer != callCards[i].discardPlayer)
            {
                return false;
            }
        }
        return true;
    }


    //明カン
    public void OpenKan(int[] cardsId,int discard, int callPlayer, int discardPlayer)
    {
        int count = 0;  //手牌からの牌のカウンタ
        for (int i = 0; i < 4; i++)
        {
            CallCard callCard = new CallCard();

            if ((i + callPlayer + 1) % 4 == discardPlayer)
            {
                //捨て牌
                callCard.cardId = discard;
                callCard.discardPlayer = discardPlayer;
            }
            else
            {
                //手牌からの牌の場合はカウンタに従って情報を取得
                callCard.cardId = cardsId[count];
                count++;
                callCard.discardPlayer = callPlayer;
            }

            if (i >= 3)
            {
                //位置調整のために最後の牌のみ間に入れる
                callCards.Insert(1, callCard);
            }
            else
            {
                //牌情報を格納
                callCards.Add(callCard);
            }
        }

        callKind = KAN;
    }

    //暗カン
    public void ClosedKan(int[] cards, int callPlayer)
    {
        bool[] hides = { true, false, false, true };

        for (int i = 0; i < cards.Length; i++)
        {
            callCards.Add(new CallCard(cards[i], callPlayer, hides[i], false));
        }

        callKind = KAN;
    }

    //加カン
    public void AddKan(int card, int callPlayer)
    {
        for (int i = 0; i < callCards.Count; i++)
        {
            if (callCards[i].discardPlayer != callPlayer)
            {
                callCards.Insert(i + 1, new CallCard(card, callPlayer, false, true));
            }
        }

        callKind = KAN;
    }

    //チー
    public void Ti(int[] cardsId,int discard, int callPlayer, int discardPlayer)
    {
        Call3(cardsId, discard, callPlayer, discardPlayer);

        callKind = TI;
    }

    //ポン
    public void Pon(int[] cardsId,int discard, int callPlayer, int discardPlayer)
    {
        Call3(cardsId, discard, callPlayer, discardPlayer);

        callKind = PON;
    }

    private void Call3(int[] cardsId, int discard, int callPlayer, int discardPlayer)
    {
        int count = 0;  //手牌からの牌のカウンタ
        for (int i = 0; i < 3; i++)
        {
            CallCard callCard = new CallCard();

            if ((i + callPlayer + 1) % 4 == discardPlayer)
            {
                //捨て牌情報
                callCard.cardId = discard;
                callCard.discardPlayer = discardPlayer;
            }
            else
            {
                //手牌からの牌の場合はカウンタに従って情報を取得
                callCard.cardId = cardsId[count];
                count++;
                callCard.discardPlayer = callPlayer;
            }

            callCards.Add(callCard);    //牌情報を格納
        }
    }

    public bool CanAddKan(int card)
    {
        return AI.Same(callCards[0].cardId, card) && callKind == PON;
    }
}

internal class Deck
{   
    // 変数 //
    private int deckTopIndex;           //次に取られる牌の位置
    private int replacementTopIndex;    //次に取られる嶺上牌の位置
    private int showBonusIndex;         //ドラ表示牌の範囲
    private int pendingShowBonusCount;

    // Cards //
    private int[] deck = new int[136];  //牌山
    private int[] replacements = new int[4];    //嶺上牌
    private int[] bonusCards = new int[10];     //ドラ表示牌

    // オブジェクト //
    private List<GameObject> bonusObjects = new List<GameObject>();

    //ゲーム開始時の初期化
    public void Initialize_NewGame()
    {
        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = (i / 4) * 2;
        }

        for (int i = 0; i < 3; i++)
        {
            deck[i * 4 * 9 + 4 * 5 - 1]++;
        }
    }

    //新しい局の開始時の初期化
    public void Initialize_NextRound()
    {
        deckTopIndex = 0;     
        replacementTopIndex = 0;    
        showBonusIndex = 0;
        pendingShowBonusCount = 0;

        Shuffle();  //シャッフル

        for (int i = 0; i < replacements.Length; i++)
        {
            replacements[i] = DrawCard();
        }

        for (int i = 0; i < bonusCards.Length; i++)
        {
            bonusCards[i] = DrawCard();
        }

        ShowBonusCards(false,false);
    }

    //シャッフル
    private void Shuffle()
    {
        for (int i = 0; i < deck.Length; i++)
        {
            int keep = deck[i];
            int r = Random.Xor128_next(deck.Length);
            deck[i] = deck[r];
            deck[r] = keep;
        }
    }

    //牌を引く
    public int DrawCard()
    {
        int card = deck[deckTopIndex];
        deckTopIndex++;    
        return card;
    }

    //嶺上牌を引く
    public int DrawKanCard()
    {
        int card = replacements[replacementTopIndex];
        replacementTopIndex++;
        return card;
    }

    //
    public void AddPendingShowBonusCount()
    {
        pendingShowBonusCount++;
    }

    //ドラ表示牌を追加
    public void AddBonusCard()
    {
        for (int i = 0; i < pendingShowBonusCount; i++)
        {
            showBonusIndex++;

            ShowBonusCards(false,false);
        }

        pendingShowBonusCount = 0;
    }

    //ドラ表示牌を表示
    private void ShowBonusCards(bool alpha,bool riichi)
    {
        GameManagerScript.DestroyGameObjects(ref bonusObjects);    //表示しているドラ表示牌のゲームオブジェクトを削除

        int[] cards = GetBonusCards();      //ドラ表示牌を取得

        int n= 2;
        if (riichi)
        {
            n = 1;
        }

        for (int i = 0; i < cards.Length / n; i++)
        {
            GameObject cardObject = new GameObject();
            Sprite sprite;
            if (i%5 <= showBonusIndex)
            {
                sprite =
                    CardImages.Image_Front(cards[i]);    //表向きの牌画像
            }
            else
            {
                sprite = CardImages.Image_Back();    //裏向きの牌画像
            }
            cardObject.AddComponent<SpriteRenderer>().sprite = sprite;        //牌画像を格納
            cardObject.transform.localScale = Layouts.bonusScale;     //大きさを決定
            cardObject.transform.position = 
                Layouts.bonusOffset + Layouts.bonusLineupDirection * (i%5)+Layouts.bonusDirection_riichi*(i/5);
            cardObject.transform.rotation =
                Quaternion.Euler(Layouts.bonusRotation);    //角度を決定

            if (alpha)
            {
                Color newColor= new Color(1f,1f,1f,1f);
                newColor.a = 0.5f;
                cardObject.GetComponent<SpriteRenderer>().color = newColor;
            }

            bonusObjects.Add(cardObject);      //メモリ解放用のリストに格納
        }
    }

    public void EndRound_ShowBonusAlpha()
    {
        ShowBonusCards(true,false);
    }

    public void ShowRiichiBonus()
    {
        ShowBonusCards(false, true);
    }

    public int[] GetBonusCards()
    {
        int[] array = new int[bonusCards.Length];
        for (int i = 0; i < bonusCards.Length; i++)
        {
            array[i] = bonusCards[i];
        }

        return array;
    }

    public int GetShowBonusIndex()
    {
        return showBonusIndex;
    }

    public bool IsExhaustionDeck()
    {
        return (deck.Length - 1) - replacementTopIndex < deckTopIndex;
    }

    public bool CanKan()
    {

        return true;
    }
}