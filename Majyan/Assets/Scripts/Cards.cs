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

    // オブジェクト //
    private List<GameObject>[] handObjects = new List<GameObject>[4];       //手牌
    private List<GameObject>[] tableObjects = new List<GameObject>[4];      //捨て牌
    private List<GameObject>[] callObjects = new List<GameObject>[4];       //鳴き牌
    
    public Cards()
    {
        for(int i = 0; i < hands.Length; i++)
        {
            hands[i] = new List<int>();
            tables[i] = new List<int>();
            calls[i] = new List<CallCardsSet>();

            handObjects[i] = new List<GameObject>();
            tableObjects[i] = new List<GameObject>();
            callObjects[i] = new List<GameObject>();
        }
    }

    public void Initialize_NewGame()
    {
        deck.Initialize_NewGame();

        Initialize_NextRound();
    }

    public void Initialize_NextRound()
    {
        deck.Initialize_NextRound();

        for (int i = 0; i < hands.Length; i++) {
            hands[i].Clear();
            tables[i].Clear();
            calls[i].Clear();
        }
    }

    //配牌
    public void Deal()
    {
        //各プレイヤーに13枚ずつ牌を配る
        for (int p = 0; p < hands.Length; p++)
        {
            for (int i = 0; i < 13; i++)
            {
                hands[p].Add(deck.DrawCard());   //山の一番上の牌を手牌として取得
            }

            hands[p].Sort();        //手牌をソートする

            ShowOrHideHand_Only(p); //手牌を表示
        }
    }

    public int DrawCard(int turn, AI[] ai)
    {
        return DrawCard(turn, ai, true);
    }

    //牌を引く
    private int DrawCard(int turn, AI[] ai,bool usual)
    {
        if (usual)
        {
            hands[turn].Add(deck.DrawCard());      //手番のプレイヤーの手牌に追加
        }
        else
        {
            hands[turn].Add(deck.DrawKanCard());      //手番のプレイヤーの手牌に追加
        }
        ShowOrHideHand_Only(turn);        //手番プレイヤーの手牌を表示

        if (turn == 0 && UserActions.Playing())
        {
            UserActions.SelectingDiscard();

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
            return ai[turn].DecideDiscardOrKan(hands[turn], calls[turn]);
        }
    }

    //捨て牌
    public int Discard(int discardIndex,int turn,AI[] ai,ref int callPlayer)
    {
        int discard = hands[turn][discardIndex];
        tables[turn].Add(discard);    //捨て牌として追加
        hands[turn].RemoveAt(discardIndex);       //手牌から取り除く

        ShowOrHideHand_Only(turn);    //手牌を表示
        ShowTableCard_Only(turn);     //捨て牌を表示

        int playing = 0;
        if (UserActions.Playing())
        {
            playing = 1;
        }
        for(int i = 0+playing; i < hands.Length; i++)
        {
            if (turn != i)
            {
                int actionId= ai[i].DecideCallKanOrPon(hands[i], discard);

                if (actionId != AI.NOT_CALL)
                {
                    callPlayer = i;
                    return actionId;
                }
            }
        }

        if (UserActions.Playing())
        {
            int sameCount = AI.CanCallKanOrPon(hands[0], discard).Count;
            if (sameCount >= 3 - 1)
            {
                UserActions.canPon = true;

                if (sameCount >= 4 - 1)
                {
                    UserActions.canOpenKan = true;
                }
            }
        }

        int tiPlayer = (turn + 1) % 4;
        if (tiPlayer== 0 && UserActions.Playing())
        {
            if (AI.CanCallTi(hands[0], discard).Count >= 1)
            {
                UserActions.canTi = true;
            }
            return AI.WAIT_INPUT;
        }
        else
        {
            return ai[tiPlayer].DecideDrawCardOrTi(hands[tiPlayer], discard);
        }
    }

    public bool Ti(int turn,int[] indexes_raw)
    {
        int discard = tables[turn][tables[turn].Count - 1];

        int tiPlayer = (turn + 1) % 4;

        int[] indexes = new int[indexes_raw.Length];

        for(int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = indexes_raw[i];
        }

        Array.Sort(indexes);

        if (AI.IsThisSetCanTi(hands[tiPlayer], discard, indexes)==false)
        {
            return false;
        }

        Array.Reverse(indexes);

        int[] cards = new int[2];   //鳴き牌格納用

        for (int i = 0; i < indexes.Length; i++)
        {
            cards[i] = hands[tiPlayer][indexes[i]];     //鳴き牌として格納
            hands[tiPlayer].RemoveAt(indexes[i]);       //手牌から取り除く
        }

        tables[turn].RemoveAt(tables[turn].Count - 1);  //捨て牌から取り除く

        calls[tiPlayer].Add(new CallCardsSet());    //鳴き牌を追加
        calls[tiPlayer][calls[tiPlayer].Count - 1].Ti(cards,discard, tiPlayer, turn); //チーとして格納

        ShowOrHideHand_Only(tiPlayer);    //手牌を表示
        ShowTableCard_Only(turn);     //捨て牌を表示
        ShowCallCard_Only(tiPlayer);      //鳴き牌を表示

        return true;
    }

    public bool Pon(int turn, int callPlayer,bool bonus)
    {
        int discard = tables[turn][tables[turn].Count - 1];

        if (callPlayer == 0 && UserActions.Playing())
        {
            if (AI.Same(discard, hands[0][UserActions.GetIndexOnly()])==false)
            {
                return false;
            }
        }

        bool ignore = false;
        if (AI.CanCallKanOrPon(hands[callPlayer], discard).Count >= 4 - 1)
        {
            ignore = true;
        }

        int[] cards = new int[2];   //鳴き牌格納用
        int count = 0;      //鳴き牌探索カウンタ

        for (int i = hands[callPlayer].Count - 1; 0 <= i; i--)
        {
            if (AI.Same(hands[callPlayer][i], discard))   //捨て牌と同じ牌の場合
            {
                if ((ignore && bonus == false && AI.Bonus5(hands[callPlayer][i])) ||
                    ignore && bonus && AI.Bonus5(hands[callPlayer][i]) == false)
                {
                    ignore = false;
                }
                else
                {
                    cards[count] = hands[callPlayer][i];     //鳴き牌として格納
                    hands[callPlayer].RemoveAt(i);           //手牌から取り除く
                    count++;                        //探索カウンタを増加

                    if (count >= 2)
                    {
                        break;
                    }
                }
            }
        }

        tables[turn].RemoveAt(tables[turn].Count - 1);  //捨て牌を取り除く
        calls[callPlayer].Add(new CallCardsSet());   //鳴き牌を追加
        //ポンとして格納
        calls[callPlayer][calls[callPlayer].Count - 1].Pon(cards,discard, callPlayer, turn);

        ShowOrHideHand_Only(callPlayer);    //手牌を表示
        ShowTableCard_Only(turn);     //捨て牌を表示
        ShowCallCard_Only(callPlayer);      //鳴き牌を表示

        return true;
    }

    public bool ClosedKan(int turn, int kanIndex)
    {
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
        ShowCallCard_Only(turn);          //鳴き牌を表示

        deck.AddPendingShowBonusCount();

        return true;
    }

    //加カン
    public bool AddKan(int turn, int kanIndex)
    {
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
        ShowCallCard_Only(turn);          //鳴き牌を表示

        deck.AddPendingShowBonusCount();

        return true;
    }

    //明カン
    public void OpenKan(int turn, int callPlayer)
    {
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
        ShowTableCard_Only(turn);     //捨て牌を表示
        ShowCallCard_Only(callPlayer);      //鳴き牌を表示

        deck.AddPendingShowBonusCount();
    }

    public void OpenAddBonusCard()
    {
        deck.AddBonusCard();
    }

    public void DrawKanCard(int turn,AI[] ai)
    {
        DrawCard(turn, ai, false);
    }
    //プレイヤー1名分の手牌を表示(基底関数)
    private void ShowHand_Only(int player, bool show)
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

            if (player == 0 && UserActions.Playing())
            {
                card.AddComponent<BoxCollider2D>();

                int index = i;
                for (int j = 0; j < UserActions.handIndexes_forCall.Length; j++)
                {
                    if (index == UserActions.handIndexes_forCall[j])
                    {
                        Color color = card.GetComponent<SpriteRenderer>().color;
                        color.a = 0.5f;
                        card.GetComponent<SpriteRenderer>().color = color;
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

            handObjects[player].Add(card);      //メモリ解放用のリストに格納
        }
    }

    //プレイヤー情報(1名分)に合わせて牌の表裏を考慮して手牌を表示(派生関数)
    public void ShowOrHideHand_Only(int player)
    {
        bool[] shows = { true, true, true, true };      //人間以外の手牌は裏向き

        ShowHand_Only(player, shows[player]);           //真偽値に応じた牌の向きで表示
    }

    //全員分の手牌を表裏を考慮して表示(派生関数)
    private void ShowAndHideHands(bool[] shows)
    {
        for (int i = 0; i < hands.Length; i++)
        {
            ShowHand_Only(i, shows[i]);
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

    //プレイヤー1名分の捨て牌を表示(基底関数)
    private void ShowTableCard_Only(int player)
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

            tableObjects[player].Add(card);     //メモリ解放用のリストに格納
        }
    }

    //鳴き牌表示
    private void ShowCallCard_Only(int player)
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

                callObjects[player].Add(card);  //メモリ解放用のリストに格納
            }
        }
    }

    //全員分の捨て牌を表示(派生関数)
    private void ShowTableCards_All()
    {
        for (int i = 0; i < tables.Length; i++)
        {
            ShowTableCard_Only(i);
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

        ShowBonusCards();
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

            ShowBonusCards();
        }

        pendingShowBonusCount = 0;
    }

    //ドラ表示牌を表示
    private void ShowBonusCards()
    {
        GameManagerScript.DestroyGameObjects(ref bonusObjects);    //表示しているドラ表示牌のゲームオブジェクトを削除

        int[] cards = GetBonusCards();      //ドラ表示牌を取得
        
        for (int i = 0; i < cards.Length / 2; i++)
        {
            GameObject cardObject = new GameObject();
            Sprite sprite;
            if (i <= showBonusIndex)
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
            cardObject.transform.position = Layouts.bonusOffset + Layouts.bonusLineupDirection * i;
            cardObject.transform.rotation =
                Quaternion.Euler(Layouts.bonusRotation);    //角度を決定

            bonusObjects.Add(cardObject);      //メモリ解放用のリストに格納
        }
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
}