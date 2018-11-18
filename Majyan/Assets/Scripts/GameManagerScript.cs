using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public Sprite[] fontImages_ja = new Sprite[6];      //フォント画像(日本語)
    public Sprite[] fontImages_num = new Sprite[10];    //フォント画像(数字)
    public Sprite[] scoreIconImages = new Sprite[2];    //点棒画像
    // クラス
    private DeckManager deckManager = new DeckManager();    //デッキ管理クラス
    //ゲームオブジェクト(メモリ解放用)
    private List<GameObject>[] handObjects = new List<GameObject>[4];       //手牌
    private List<GameObject>[] tableObjects = new List<GameObject>[4];      //捨て牌
    private List<GameObject>[] callObjects = new List<GameObject>[4];       //鳴き牌
    private List<GameObject> roundTextObjects = new List<GameObject>();     //局数表示用テキスト
    private List<GameObject>[] scoreTextObjects = new List<GameObject>[4];  //得点表示用テキスト
    private List<GameObject> bonusObjects = new List<GameObject>();
    
    //AI
    private AI[] ai = { new Draw_Discard(), new Draw_Discard(), new Draw_Discard(), new Draw_Discard() };

    //UI
    private int movingCard=NULL_ID;

    // 定数 //
    private const int NULL_ID = -1000;  //nullとして扱う数値

    // 変数 //
    private int[] scores = new int[4];  //得点
    private int startPlayer;            //起家
    private int round;                  //局数
    private int parentCount;            //n本場
    private int betCount;               //リーチ棒の数
    private bool parentStay;            //連荘フラグ
    private bool increaseParentCount;   //n本場増加フラグ

    private int turnPlayer;     //手番プレイヤー

    private bool playing = true;

    private delegate void NextMethod();     
    private NextMethod nextMethod;          //一定時間経過後に実行する関数
    private float timer;                    //タイマー

    private int seed;       //乱数のseed値(表示用)

    // Cards //
    private List<int>[] hands = new List<int>[4];       //手牌
    private List<int>[] tables = new List<int>[4];      //捨て牌
    private List<CallCardsSet>[] callCards = new List<CallCardsSet>[4];     //鳴き牌

	// Use this for initialization
	void Start () {
        //Random.SetSeed(DateTime.Now.Millisecond);

        //235:暗カン,277:加カン、明カン,908:明カン
        seed = 235;
        seed = DateTime.Now.Millisecond;    //seed値決定
        Random.SetSeed(10000+seed);         //seed値を入力

        GameObject.Find("Canvas/SeedText").GetComponent<Text>().text = ""+seed;  //seed値を表示

        //リストの初期化
        for (int i = 0; i < 4; i++)
        {
            hands[i] = new List<int>();
            tables[i] = new List<int>();
            callCards[i] = new List<CallCardsSet>();

            handObjects[i] = new List<GameObject>();
            tableObjects[i] = new List<GameObject>();
            callObjects[i] = new List<GameObject>();
            scoreTextObjects[i] = new List<GameObject>();
        }

        Initialize_NewGame();       //ゲーム開始時用の初期化処理
	}

    // Update is called once per frame
    void Update()
    {

        if (nextMethod != null)         //一定時間経過後に実行する関数が存在する場合
        {
            timer -= Time.deltaTime;    //タイマーを進める

            if (timer <= 0)             //待ち時間が終了している場合
            {
                nextMethod();           //格納されていた関数を実行
            }
        }

        if (Input.touchSupported)
        {
            if (Input.touchCount <= 0)
            {
                MouseUp();
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                MouseUp();
            }
        }
    }

    //ゲーム開始時の初期化
    private void Initialize_NewGame()
    {
        //デッキ管理クラスにゲーム開始時用の初期化処理を実行させる
        deckManager.Initialize_NewGame();

        //得点をリセット
        for(int i = 0; i < scores.Length; i++)
        {
            scores[i] = 25000;
        }

        startPlayer = Random.Xor128_next(4);    //起家を決定
        round = 0;          //局数
        parentCount = 0;    //n本場
        betCount = 0;       //リーチ棒
        parentStay = true;  //親を変更しない(Initialize_NextRound()用)
        increaseParentCount = false;    //n本場を増加しない(Initialize_NextRound()用)

        timer = 0f;         //タイマー初期化

        Initialize_NextRound();     //局単位の初期化処理
    }

    //局単位での初期化処理
    private void Initialize_NextRound()
    {
        //デッキ管理クラスに局単位での初期化処理を実行させる
        deckManager.Initialize_NextRound();     

        //手牌と捨て牌を初期化
        for (int i = 0; i < 4; i++)
        {
            hands[i].Clear();
            tables[i].Clear();
        }

        //親が流れた場合は局数のカウントを追加
        if (parentStay == false)
        {
            round++;
        }

        if (increaseParentCount)    //フラグ
        {
            parentCount++;      //n本場を増加
        }
        else
        {
            parentCount = 0;    //n本場をリセット
        }

        ShowRound();        //局数表示
        ShowBonusCards();   //ドラ表示

        turnPlayer = Rules.HouseIdToPlayerId(startPlayer, round, 0);    //東家のプレイヤーの手番で開始

        nextMethod= Deal;       //配牌
        timer = 0f;             //待ち時間を設定
    }

    //局数表示
    private void ShowRound()
    {
        //表示する文字を格納
        Sprite[] texts = new Sprite[9];
        texts[0]= fontImages_ja[round / 4];
        texts[1]= fontImages_num[round % 4 + 1];
        texts[2]= fontImages_ja[4];
        texts[3] = scoreIconImages[0];
        texts[4] = fontImages_ja[5];
        texts[5] = fontImages_num[parentCount%10];
        texts[6] = scoreIconImages[1];
        texts[7] = fontImages_ja[5];
        texts[8] = fontImages_num[betCount%10];

        //ゲームオブジェクトとしてパラメータを設定＆文字を表示
        for(int i = 0; i < texts.Length; i++)
        {
            GameObject text = new GameObject();
            text.AddComponent<SpriteRenderer>().sprite = texts[i];      //フォント画像を格納
            text.transform.localScale = Layouts.roundTextScales[i];     //大きさを決定
            text.transform.position = Layouts.roundTextPositons[i];     //表示場所を決定
            text.transform.rotation = Quaternion.Euler(Layouts.roundTextRotations[i]);      //角度を指定

            roundTextObjects.Add(text);     //メモリ解放用のリストに格納
        }

        ShowScores_All();       //全員分の得点を表示
    }

    //得点表示(基底関数)
    private void ShowScore(int player)
    {
        //方角表示
        GameObject compass = new GameObject();
        compass.AddComponent<SpriteRenderer>().sprite =
            fontImages_ja[Rules.PlayerIdToHouseId(startPlayer,round,player)];       //フォント画像を格納
        compass.transform.localScale = Layouts.scoreTextScales[player];             //大きさを決定
        compass.transform.position = Layouts.scoreTextOffsets[player];              //表示場所を決定
        compass.transform.rotation = Quaternion.Euler(Layouts.scoreTextRotations[player]);      //角度を指定

        scoreTextObjects[player].Add(compass);      //メモリ解放用のリストに格納


        //得点表示
        bool zero = false;              //頭の0を表示しないためのフラグ
        int score = scores[player];     //得点を取得
        for (int i=0, n = 100000; n >= 1;i++, n /= 10)
        //i:表示場所決定用のカウンタ
        //n:指定の桁の数値を1桁に変換するための数値
        {
            if (zero || score / n > 0 || n == 1)
                //頭の0以外なら表示
                //1の位なら無条件に表示
            {
                zero = true;    //頭の0が終わったことを示すフラグ

                //数値として表示
                GameObject text = new GameObject();
                text.AddComponent<SpriteRenderer>().sprite = fontImages_num[score / n];     //表示する数字を決定
                text.transform.localScale = Layouts.scoreTextScales[player];    //大きさを決定
                text.transform.position = 
                    Layouts.scoreTextOffsets[player]+
                    Layouts.scoreTextCompassSpaces[player]+
                    Layouts.scoreTextLineupDirections[player]*i;            //表示場所を決定
                text.transform.rotation = 
                    Quaternion.Euler(Layouts.scoreTextRotations[player]);   //角度を決定

                score -= (score / n) * n;               //表示した桁を0にする

                scoreTextObjects[player].Add(text);     //メモリ解放用のリストに格納
            }
        }
    }    

    //全員分の得点を表示(派生関数)
    private void ShowScores_All()
    {
        for(int i = 0; i < scores.Length; i++)
        {
            ShowScore(i);
        }
    }
    
    //配牌
    private void Deal()
    {
        //各プレイヤーに13枚ずつ牌を配る
        for (int p = 0; p < hands.Length; p++) {
            for (int i = 0; i < 13;i++)
            {
                hands[p].Add(deckManager.DrawCard());   //山の一番上の牌を手牌として取得
            }

            hands[p].Sort();        //手牌をソートする

            ShowOrHideHand_Only(p); //手牌を表示
        }

        nextMethod = DrawCard;              //一定時間経過後に牌を引く
        timer = Times.Wait_DealToDraw();    //タイマーを設定
    }

    //プレイヤー1名分の手牌を表示(基底関数)
    private void ShowHand_Only(int player,bool show)
    {
        DestroyCardObjects(ref handObjects[player]);    //表示している手牌のゲームオブジェクトを削除
        for (int i = 0; i < hands[player].Count; i++)   
        {
            GameObject card = new GameObject();
            Sprite sprite;
            int handCard = hands[player][i];
            if (show)   
            {
                sprite = 
                    cardImages[Rules.IdChangeSerialToCardImageId(handCard)];    //表向きの牌画像
            }
            else
            {
                sprite = cardImages[cardImages.Length-1];    //裏向きの牌画像
            }
            
            card.AddComponent<SpriteRenderer>().sprite = sprite;        //牌画像を格納
            card.transform.localScale = Layouts.handScales[player];     //大きさを決定
            card.transform.position = Layouts.handOffsets[player]+Layouts.handLineupDirections[player]*i;
            card.transform.rotation = 
                Quaternion.Euler(Layouts.handRotations[player]);    //角度を決定

            if (player == 0&&playing)
            {
                card.AddComponent<BoxCollider2D>();

                EventTrigger.Entry down = new EventTrigger.Entry();
                down.eventID = EventTriggerType.PointerDown;
                down.callback.AddListener((f) => MouseDown(handCard));
                card.AddComponent<EventTrigger>().triggers.Add(down);

                EventTrigger.Entry drag = new EventTrigger.Entry();
                drag.eventID = EventTriggerType.PointerEnter;
                int index = i;
                drag.callback.AddListener((f) => MouseMove(index));
                card.GetComponent<EventTrigger>().triggers.Add(drag);
            }

            handObjects[player].Add(card);      //メモリ解放用のリストに格納
        }
    }

    //
    public void MouseDown(int card)
    {
        movingCard = card;
        ShowOrHideHand_Only(0);
    }

    //
    public void MouseMove(int index)
    {
        if (movingCard >= 0)
        {
            for (int i = 0; i < hands[0].Count; i++)
            {
                if (hands[0][i] == movingCard)
                {
                    hands[0].RemoveAt(i);
                    break;
                }
            }

            hands[0].Insert(index, movingCard);

            ShowOrHideHand_Only(0);
        }
    }

    //
    public void MouseUp()
    {
        movingCard = NULL_ID;
        ShowOrHideHand_Only(0);
    }

    //プレイヤー情報(1名分)に合わせて牌の表裏を考慮して手牌を表示(派生関数)
    private void ShowOrHideHand_Only(int player)
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
        bool[] shows = { true, true, true, true};

        ShowAndHideHands(shows);    //真偽値リストに基づいて表示
    }

    //プレイヤー情報に合わせて牌の表裏を考慮して全員分の手牌を表示(派生関数)
    private void ShowAndHideHands_Default()
    {
        bool[] shows = { true, false, false, false };

        ShowAndHideHands(shows);    //真偽値リストに基づいて表示
    }

    //牌のゲームオブジェクトを削除
    private void DestroyCardObjects(ref List<GameObject> cardObjects)
    {
        for (int i = cardObjects.Count-1; 0 <= i; i--)
        {
            Destroy(cardObjects[i]);
        }

        cardObjects.Clear();    //ゲームオブジェクトの格納変数を初期化
    }

    //ドラ表示牌を表示
    private void ShowBonusCards()
    {
        DestroyCardObjects(ref bonusObjects);    //表示しているドラ表示牌のゲームオブジェクトを削除

        int[] cards = deckManager.GetBonusCards();      //ドラ表示牌を取得
        int index = deckManager.GetShowBonusIndex();    //表示範囲を取得

        for (int i = 0; i < cards.Length/2; i++)
        {
            GameObject card = new GameObject();
            Sprite sprite;
            if (i<=index)
            {
                sprite =
                    cardImages[Rules.IdChangeSerialToCardImageId(cards[i])];    //表向きの牌画像
            }
            else
            {
                sprite = cardImages[cardImages.Length - 1];    //裏向きの牌画像
            }
            card.AddComponent<SpriteRenderer>().sprite = sprite;        //牌画像を格納
            card.transform.localScale = Layouts.bonusScale;     //大きさを決定
            card.transform.position = Layouts.bonusOffset+ Layouts.bonusLineupDirection* i;
            card.transform.rotation =
                Quaternion.Euler(Layouts.bonusRotation);    //角度を決定

            bonusObjects.Add(card);      //メモリ解放用のリストに格納
        }
    }

    //牌を引く
    private void DrawCard()
    {
        hands[turnPlayer].Add(deckManager.DrawCard());      //手番のプレイヤーの手牌に追加

        DrawCommon();       //牌を引くときの共通処理
    }

    //牌を引くときの共通処理
    private void DrawCommon()
    {
        ShowOrHideHand_Only(turnPlayer);        //手番プレイヤーの手牌を表示

        if (Rules.CanClosedKan(hands[turnPlayer]))  //暗カン可能な場合
        {
            if (ai[turnPlayer].DecideClosedKan(hands[turnPlayer]))  //AIが暗カンを行うと判断した場合
            {
                nextMethod = ClosedKan;     //暗カン
                timer = Times.Wait_HandKan();     //タイマーを設定

                return;
            }
        }

        if (Rules.CanAddKan(hands[turnPlayer], callCards[turnPlayer]))  //加カン可能な場合
        {
            if (ai[turnPlayer].DecideAddKan(hands[turnPlayer], callCards[turnPlayer]))  //AIが加カンを行うと判断した場合
            {
                nextMethod = AddKan;     //加カン
                timer = Times.Wait_HandKan();     //タイマーを設定

                return;
            }
        }

        ai[turnPlayer].DecideDiscard(hands[turnPlayer]);    //AIに捨て牌を決定させる

        nextMethod = Discard;       //捨て牌をする
        timer = Times.Wait_DrawToDiscard();     //タイマーを設定
    }

    //暗カン
    private void ClosedKan()
    {
        int kan_card = ai[turnPlayer].GetKanCardId();   //カンをする牌のid
        int[] callCard = new int[4];   //カンに用いる牌の格納用配列
        int count = 0;  //格納カウンタ

        for (int i = hands[turnPlayer].Count - 1; 0 <= i; i--)
        {
            if (Rules.Same_BonusEquate(hands[turnPlayer][i],kan_card))
            {
                //指定の牌と同じの場合は手牌から格納用配列に移動
                callCard[count] = hands[turnPlayer][i];
                hands[turnPlayer].RemoveAt(i);
                count++;
                if (count >= 4)
                {
                    break;
                }
            }
        }

        callCards[turnPlayer].Add(new CallCardsSet());  //鳴き牌を追加
        callCards[turnPlayer][callCards[turnPlayer].Count - 1].ClosedKan(callCard, turnPlayer); //暗カンとして格納

        ShowOrHideHand_Only(turnPlayer);        //手牌を表示
        ShowCallCard_Only(turnPlayer);          //鳴き牌を表示

        nextMethod = OpenAddBonus_Closed;  //ドラ表示牌をめくる
        timer = Times.Wait_OpenAddBonus();      //タイマーを設定
    }

    //加カン
    private void AddKan()
    {
        int kan_index = ai[turnPlayer].GetKanCardId();   //カンをする牌の鳴き牌内での位置情報
        int callCard = NULL_ID;

        for (int i = hands[turnPlayer].Count - 1; 0 <= i; i--)
        {
            if (Rules.Same_BonusEquate(hands[turnPlayer][i], callCards[turnPlayer][kan_index].callCards[0].card))
            {
                //指定の牌と同じの場合は手牌から鳴き牌に移動
                callCard = hands[turnPlayer][i];
                hands[turnPlayer].RemoveAt(i);
                    break;
            }
        }

        callCards[turnPlayer][kan_index].AddKan(callCard, turnPlayer); //加カンとして格納

        ShowOrHideHand_Only(turnPlayer);        //手牌を表示
        ShowCallCard_Only(turnPlayer);          //鳴き牌を表示

        nextMethod = DrawKanCard_Open;       //嶺上牌を引く
        timer = Times.Wait_DrawKanCard();     //タイマーを設定
    }

    //ドラ表示牌をめくる(暗カン)
    private void OpenAddBonus_Closed()
    {
        deckManager.AddBonusCard();     //ドラ表示牌を追加
        ShowBonusCards();   //ドラ表示

        nextMethod = DrawKanCard_Closed;       //嶺上牌を引く
        timer = Times.Wait_DrawKanCard();     //タイマーを設定

    }

    //ドラ表示牌をめくる(明カン、加カン)
    private void OpenAddBonus_Open()
    {
        deckManager.AddBonusCard();     //ドラ表示牌を追加
        ShowBonusCards();   //ドラ表示

        nextMethod = CallRob;      //鳴き
        timer = Times.Wait_Call();  //タイマーを設定
    }

    //嶺上牌を引く(暗カン)
    private void DrawKanCard_Closed()
    {
        hands[turnPlayer].Add(deckManager.DrawKanCard());      //手番のプレイヤーの手牌に追加

        DrawCommon();       //牌を引くときの共通処理

        nextMethod = Discard;       //捨て牌をする
        timer = Times.Wait_DrawToDiscard();     //タイマーを設定
    }

    //嶺上牌を引く(明カン、加カン)
    private void DrawKanCard_Open()
    {
        hands[turnPlayer].Add(deckManager.DrawKanCard());      //手番のプレイヤーの手牌に追加

        DrawCommon();       //牌を引くときの共通処理

        nextMethod = Discard_OpenKan;       //捨て牌をする
        timer = Times.Wait_DrawToDiscard();     //タイマーを設定
    }

    //捨て牌
    private void Discard()
    {
        int discardIndex = ai[turnPlayer].GetDiscardIndex();     //捨て牌の手牌内での位置情報を取得
        tables[turnPlayer].Add(hands[turnPlayer][discardIndex]);    //捨て牌として追加
        hands[turnPlayer].RemoveAt(discardIndex);       //手牌から取り除く

        ShowOrHideHand_Only(turnPlayer);    //手牌を表示
        ShowTableCard_Only(turnPlayer);     //捨て牌を表示

        nextMethod = CallRob;      //鳴き
        timer = Times.Wait_Call();  //タイマーを設定
    }

    //ドラ表示牌をめくる直前の捨て牌
    private void Discard_OpenKan()
    {
        Discard();      //捨て牌をする

        nextMethod = OpenAddBonus_Open;     //ドラ表示牌をめくる
        timer = Times.Wait_OpenAddBonus();     //タイマーを設定
    }

    //プレイヤー1名分の捨て牌を表示(基底関数)
    private void ShowTableCard_Only(int player)
    {
        DestroyCardObjects(ref tableObjects[player]);       //捨て牌のゲームオブジェクトを削除

        for (int i = 0; i < tables[player].Count; i++)
        {
            GameObject card = new GameObject();
            Sprite sprite = 
                cardImages[Rules.IdChangeSerialToCardImageId(tables[player][i])];   //画像を取得
            card.AddComponent<SpriteRenderer>().sprite = sprite;            //画像を格納
            card.transform.localScale = Layouts.tableScales[player];        //大きさを決定
            card.transform.position =
                Layouts.tableOffsets[player] +
                Layouts.tableLineupNextDirections[player] * (i%6)+
                Layouts.tableLineupNewLineDirections[player] * (i / 6);     //表示場所を決定
            card.transform.rotation = 
                Quaternion.Euler(Layouts.tableRotations[player]);           //角度を決定

            tableObjects[player].Add(card);     //メモリ解放用のリストに格納
        }
    }

    //全員分の捨て牌を表示(派生関数)
    private void ShowTableCards_All()
    {
        for(int i = 0; i < tables.Length; i++)
        {
            ShowTableCard_Only(i);
        }
    }

    //鳴き
    private void CallRob()
    {
        int discard = tables[turnPlayer][tables[turnPlayer].Count - 1];     //捨て牌

        for (int p = 0; p < hands.Length; p++)
        {
            if (p != turnPlayer)    //自分の捨て牌は除く
            {
                bool[] pon_kan = Rules.CanPonOrOpenKan(hands[p], discard);  //ポン、カンが可能かを取得

                if (pon_kan[1])     //カン可能
                {
                    if (ai[p].DecideOpenKan(hands[p]))      //AIがカンを行うと判断した場合
                    {
                        OpenKan(p,discard);     //明カン

                        return;
                    }
                }
                if (pon_kan[0])         //ポン可能
                {
                    bool bonus = true;    //鳴き牌に赤ドラを含めるかのフラグ

                    if (ai[p].DecidePon(hands[p], ref bonus))    //AIがポンを行うと判断した場合
                    {
                        Pon(p, discard, bonus);     //ポン

                        return;
                    }
                }
            }
        }

        int tiPlayer = (turnPlayer + 1) % 4;    //チーが可能なプレイヤー
        if (Rules.CanTi(hands[tiPlayer], discard))  //チーが可能な場合
        {
            int[] indexes = new int[2];     //鳴き牌とする牌の手牌内での位置情報の格納用
            if (ai[tiPlayer].DecideTi(hands[tiPlayer], ref indexes, discard))    //AIがチーをすると判断した場合
            {
                Ti(tiPlayer, indexes, discard);     //チー

                return;
            }
        }

        //鳴きが無ければ実行される
        nextMethod = NextTurn;          //手番移動処理
        timer = Times.Wait_NextTurn();  //タイマーを設定
    }

    //明カン
    private void OpenKan(int callPlayer,int discard)
    {
        int[] cards = new int[4];       //鳴き牌格納用
        int count = 0;      //鳴き牌探索カウンタ

        for (int i = hands[callPlayer].Count - 1; 0 <= i; i--)
        {
            if (Rules.Same_BonusEquate(hands[callPlayer][i], discard))   //捨て牌と同じ牌の場合
            {
                cards[count] = hands[callPlayer][i];     //鳴き牌として格納
                hands[callPlayer].RemoveAt(i);           //手牌から取り除く
                count++;                        //探索カウンタを増加
            }
        }

        cards[3] = discard;     //鳴き牌格納用の配列の最後(=未定義の部分)に捨て牌を格納
        tables[turnPlayer].RemoveAt(tables[turnPlayer].Count - 1);          //捨て牌から取り除く
        callCards[callPlayer].Add(new CallCardsSet());       //鳴き牌を追加
        callCards[callPlayer][callCards[callPlayer].Count - 1].OpenKan(cards, callPlayer, turnPlayer);     //カンとして格納

        CallRob_End(callPlayer);     //鳴き終了処理

        nextMethod = DrawKanCard_Open;       //嶺上牌を引く
        timer = Times.Wait_DrawKanCard();     //タイマーを設定
    }

    //ポン
    private void Pon(int callPlayer,int discard,bool bonus)
    {
        int[] cards = new int[3];   //鳴き牌格納用
        int count = 0;      //鳴き牌探索カウンタ

        for (int i = hands[callPlayer].Count - 1; 0 <= i; i--)
        {
            if (Rules.Same_BonusEquate(hands[callPlayer][i], discard))   //捨て牌と同じ牌の場合
            {
                cards[count] = hands[callPlayer][i];     //鳴き牌として格納
                hands[callPlayer].RemoveAt(i);           //手牌から取り除く
                count++;                        //探索カウンタを増加
            }
        }

        if (count >= 3&&Rules.Bonus5(discard))     //鳴き牌の選択肢が複数の場合
        {
            int bonusIndex;     //赤ドラの扱い

            if (bonus == false) //赤ドラを鳴き牌に含めない場合
            {
                bonusIndex = 2; //鳴き牌格納用配列の末尾へ格納
            }
            else
            {
                bonusIndex = 0; //鳴き牌格納用配列の先頭へ格納
            }

            for (int i = 0; i < cards.Length; i++)
            {
                if (Rules.Bonus5(cards[i]))     //赤ドラの場合
                {
                    //赤ドラとその他の牌を入れ替える
                    int keep = cards[bonusIndex];
                    cards[bonusIndex] = cards[i];
                    cards[i] = keep;
                    break;
                }
            }
        }

        cards[2] = discard;     //鳴き牌格納用配列の末尾を捨て牌で上書き
        tables[turnPlayer].RemoveAt(tables[turnPlayer].Count - 1);  //捨て牌を取り除く
        callCards[callPlayer].Add(new CallCardsSet());   //鳴き牌を追加
        callCards[callPlayer][callCards[callPlayer].Count - 1].Pon(cards, callPlayer, turnPlayer);     //ポンとして格納

        CallRob_End(callPlayer);     //鳴き終了処理
    }

    //チー
    private void Ti(int tiPlayer,int[] indexes,int discard)
    {
        //牌の並び順を降順にする
        Array.Sort(indexes);
        Array.Reverse(indexes);

        int[] cards = new int[3];   //鳴き牌格納用

        for (int i = 0; i < indexes.Length; i++)
        {
            cards[i] = hands[tiPlayer][indexes[i]];     //鳴き牌として格納
            hands[tiPlayer].RemoveAt(indexes[i]);       //手牌から取り除く
        }

        cards[2] = discard;     //鳴き牌格納用の配列の最後(=未定義の部分)に捨て牌を格納
        tables[turnPlayer].RemoveAt(tables[turnPlayer].Count - 1);  //捨て牌から取り除く
        callCards[tiPlayer].Add(new CallCardsSet());    //鳴き牌を追加
        callCards[tiPlayer][callCards[tiPlayer].Count - 1].Ti(cards, tiPlayer, turnPlayer); //チーとして格納

        CallRob_End(tiPlayer);      //鳴き終了処理
    }

    //鳴き終了処理
    private void CallRob_End(int callPlayer)
    {
        ShowOrHideHand_Only(callPlayer);    //手牌を表示
        ShowTableCard_Only(turnPlayer);     //捨て牌を表示
        ShowCallCard_Only(callPlayer);      //鳴き牌を表示

        ai[callPlayer].DecideDiscard(hands[callPlayer]);    //AIに捨て牌を決定させる
        turnPlayer = callPlayer;                            //手番を移動

        nextMethod = Discard;               //捨て牌をする
        timer = Times.Wait_DrawToDiscard(); //タイマーを設定
    }

    //鳴き牌表示
    private void ShowCallCard_Only(int player)
    {
        DestroyCardObjects(ref callObjects[player]);    //表示している手牌のゲームオブジェクトを削除

        Vector2 position = 
            new Vector2(Layouts.callOffsets[player].x, Layouts.callOffsets[player].y);  //表示位置の初期地点
        
        for (int s = 0; s < callCards[player].Count; s++)
        {
            for (int i = 0; i< callCards[player][s].callCards.Count; i++)
            {
                CallCard callCard = callCards[player][s].callCards[i];  //牌の情報を取得
                GameObject card = new GameObject();
                Sprite sprite;
                if (callCard.closedKan_hide)
                {
                    sprite = cardImages[cardImages.Length - 1];   //裏向き
                }
                else
                {
                    sprite =cardImages[Rules.IdChangeSerialToCardImageId(callCard.card)];   //表向き
                }
                card.AddComponent<SpriteRenderer>().sprite = sprite;        //画像を格納
                card.transform.localScale = Layouts.callScales[player];     //大きさを決定

                float addDirection;     //回転した牌のための余白を用意するかどうかのフラグ
                Vector3 addRotation ;   //回転した牌の場合に追加する回転量
                Vector2 addY=new Vector2( 0f,0f);          //加カン牌用のずらしフラグ
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
                        Layouts.callLineupRotatedAddYPositions[player]+
                        Layouts.callLineupAddDoubleYPositions[player];

                    addDirection = 1.0f;    //余白用意フラグを立てる
                    addRotation = new Vector3(0f, 0f, 90f);     //回転させる
                }
                card.transform.position = position+addY; //牌の表示位置を決定
                //回転の有無を考慮して次の牌の基本位置を決定
                position += 
                    Layouts.callLineupDirections[player]+ 
                    Layouts.callLineupRotatedAddDirections[player]*addDirection;
                card.transform.rotation =
                    Quaternion.Euler(Layouts.callRotations[player]+addRotation);    //牌の角度を決定

                callObjects[player].Add(card);  //メモリ解放用のリストに格納
            }
        }
    }

    //手番移動処理
    private void NextTurn()
    {
        turnPlayer = (turnPlayer + 1) % 4;  //手番移動
        DrawCard();     //牌を引く
    }
}

public class DeckManager
{
    // 変数 //
    private int deckUsedIndex;  //使用済みの牌の位置
    private int replacementUsedIndex;   //使用済みの嶺上牌の位置
    private int showBonusIndex;     //ドラ表示牌の範囲

    // Cards //
    private int[] deck = new int[136];  //牌山
    /*private int[] deck = {0,1,2,4,5,6,8,9,10,12,13,14,16,
        20,21,22,24,25,26,28,29,30,32,33,34,36,
    40,41,42,44,45,46,48,49,50,52,53,54,56,
    60,61,62,64,65,66,68,69,70,72,73,74,76,
    3,7,11,15,17,18,19,23,27,31,35,37,38,39,43,47,51,55,57,58,59,63,67,71,75,77,78,79,80};*/
    private int[] replacements = new int[4];    //嶺上牌
    private int[] bonusCards = new int[10];     //ドラ表示牌

        //ゲーム開始時の初期化
    public void Initialize_NewGame()
    {
        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = i;
        }
    }

    //新しい局の開始時の初期化
    public void Initialize_NextRound()
    {
        deckUsedIndex = -1;     //使用済みの牌は存在しないので-1
        replacementUsedIndex = -1;       //カンの回数
        showBonusIndex = 0;     //ドラ表示牌の範囲

        Shuffle();  //シャッフル

        for(int i = 0; i < replacements.Length; i++)
        {
            replacements[i] = DrawCard();
        }

        for(int i = 0; i < bonusCards.Length; i++)
        {
            bonusCards[i] = DrawCard();
        }
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
        deckUsedIndex++;    //未使用の牌の先頭のものを使用済みの牌とする
        return deck[deckUsedIndex];
    }

    //嶺上牌を引く
    public int DrawKanCard()
    {
        replacementUsedIndex++;
        return replacements[replacementUsedIndex];
    }

    //ドラ表示牌を追加
    public void AddBonusCard()
    {
        showBonusIndex++;
    }

    public int[] GetBonusCards()
    {
        int[] array = new int[bonusCards.Length];
        for(int i = 0; i < bonusCards.Length; i++)
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

public class CallCard
{
    //鳴き牌の情報
    public int card;
    public int discardPlayer;
    public bool closedKan_hide;
    public bool addKan;

    public CallCard()
    {
        closedKan_hide = false;
        addKan = false;
    }

    public CallCard(int card,int discardPlayer, bool closedKan_hide,bool addKan)
    {
        this.card = card;
        this.discardPlayer = discardPlayer;
        this.closedKan_hide = closedKan_hide;
        this.addKan = addKan;
    }
}

public class CallCardsSet
{
    public List<CallCard> callCards = new List<CallCard>();     //鳴き牌による1面子

    //ポン
    public void Pon(int[] cardsId,int callPlayer, int discardPlayer)
    {
        int count = 0;  //手牌からの牌のカウンタ
        for (int i = 0; i < 3; i++)
        {
            CallCard callCard= new CallCard();

            if ((i + callPlayer + 1) % 4 == discardPlayer)  
            {
                //捨て牌の場合は末尾の情報を用いる
                callCard.card = cardsId[2];
                callCard.discardPlayer = discardPlayer;
            }
            else
            {
                //手牌からの牌の場合はカウンタに従って情報を取得
                callCard.card = cardsId[count];
                count++;
                callCard.discardPlayer = callPlayer;
            }

            callCards.Add(callCard);    //牌情報を格納
        }
    }

    //明カン
    public void OpenKan(int[] cardsId, int callPlayer, int discardPlayer)
    {
        int count = 0;  //手牌からの牌のカウンタ
        for (int i = 0; i < 4; i++)
        {
            CallCard callCard = new CallCard();

            if ((i + callPlayer + 1) % 4 == discardPlayer)
            {
                //捨て牌の場合は末尾の情報を用いる
                callCard.card = cardsId[3];
                callCard.discardPlayer = discardPlayer;
            }
            else
            {
                //手牌からの牌の場合はカウンタに従って情報を取得
                callCard.card = cardsId[count];
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
    }

    //暗カン
    public void ClosedKan(int[] cards,int callPlayer)
    {
        bool[] hides = { true, false, false, true};

        for(int i = 0; i < cards.Length; i++)
        {
            callCards.Add(new CallCard(cards[i],callPlayer,hides[i],false));
        }
    }

    //加カン
    public void AddKan(int card,int callPlayer)
    {
        for(int i = 0; i < callCards.Count; i++)
        {
            if (callCards[i].discardPlayer != callPlayer)
            {
                callCards.Insert(i + 1, new CallCard(card, callPlayer, false, true));
            }
        }
    }

    //チー
    public void Ti(int[] cardsId, int callPlayer, int discardPlayer)
    {
        int count = 0;  //手牌からの牌のカウンタ
        for (int i = 0; i < 3; i++)
        {
            CallCard callCard = new CallCard();

            if ((i + callPlayer + 1) % 4 == discardPlayer)
            {
                //捨て牌の場合は末尾の情報を用いる
                callCard.card = cardsId[2];
                callCard.discardPlayer = discardPlayer;
            }
            else
            {
                //手牌からの牌の場合はカウンタに従って情報を取得
                callCard.card = cardsId[count];
                count++;
                callCard.discardPlayer = callPlayer;
            }

            callCards.Add(callCard);    //牌情報を格納
        }
    }
}