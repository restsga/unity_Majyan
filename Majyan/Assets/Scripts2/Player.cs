using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    // 共通の情報 //
    //起家
    static private int firstPlayer;
    //親
    static private int parent;
    //ラウンド数
    static private int round;
    //残っているリーチ棒の数
    static private int betCount;
    //n本場
    static private int parentCount;
    //手番
    static public int turn;
    //プレイヤーの参加の有無
    static private bool playing = true;
    //最終捨て牌(鳴き可能な牌)
    static private int lastDiscard;

    //場風、ラウンド数、リーチ棒の数、n本場、手番の表示用オブジェクト
    static private SpriteRenderer fieldWindText;
    static private SpriteRenderer roundText;
    static private SpriteRenderer betCountText;
    static private SpriteRenderer parentCountText;
    static private SpriteRenderer[] turnIcon;
    
    // 個別の情報 //
    //競技者ID
    private int id;
    //手牌、副露牌、捨て牌
    private List<int> hand = new List<int>();
    private List<int[]> call = new List<int[]>();
    private List<int> table = new List<int>();
    //自風
    private int direction;
    //得点
    private int score=25000;
    //AI
    private AI_Base ai;

    //自風、得点、メッセージの表示用のオブジェクト
    private SpriteRenderer seatWindText;
    private SpriteRenderer[] scoreTexts;
    private SpriteRenderer messageText;
    //手牌、捨て牌の表示用オブジェクト
    private SpriteRenderer[] handSpriteRenderers = new SpriteRenderer[14];
    private SpriteRenderer[] tableSpriteRenderers = new SpriteRenderer[6 * 5];


    public Player(int id,AI_Base ai)
    {
        this.id = id;
        this.ai = ai;
    }

    //プレイヤーは参加しているか
    static public bool IsPlaying()
    {
        return playing;
    }

    //アプリケーション起動時の初期化
    public void Initialize_Application()
    {
        //手牌、捨て牌の表示用のSpriteRendererを取得
        handSpriteRenderers = 
            Enumerable.Range(0, 14).
            Select(i => GameObject.Find("UIObjects/Hands/PL (" + id + ")/Card (" + i + ")").
            GetComponent<SpriteRenderer>()).ToArray();
        tableSpriteRenderers=
            Enumerable.Range(0,30).
            Select(i => GameObject.Find("UIObjects/Tables/PL (" + id + ")/Card (" + i + ")").
            GetComponent<SpriteRenderer>()).ToArray();

        //場風、ラウンド数、リーチ棒の数、n本場、手番の表示用のSpriteRendererを取得
        fieldWindText = GameObject.Find("UIObjects/RoundText/Wind").GetComponent<SpriteRenderer>();
        roundText = GameObject.Find("UIObjects/RoundText/Round").GetComponent<SpriteRenderer>();
        betCountText = GameObject.Find("UIObjects/RoundText/Count1000").GetComponent<SpriteRenderer>();
        parentCountText = GameObject.Find("UIObjects/RoundText/Count100").GetComponent<SpriteRenderer>();
        turnIcon =
            Enumerable.Range(0, 4).
            Select(i => GameObject.Find("UIObjects/Directions/PL (" + i + ")/TurnIcon").
            GetComponent<SpriteRenderer>()).ToArray();

        //自風、得点、メッセージの表示用のSpriteRendererを取得
        seatWindText = GameObject.Find("UIObjects/Directions/PL (" + id + ")").GetComponent<SpriteRenderer>();
        scoreTexts= Enumerable.Range(0, 6).Select
            (i => GameObject.Find("UIObjects/Scores/PL (" + id + ")/" + i).GetComponent<SpriteRenderer>()).ToArray();
        messageText= GameObject.Find("UIObjects/Messages/PL (" + id + ")").GetComponent<SpriteRenderer>();
    }

    //ゲーム開始時の初期化
    public void Initialize_NewGame()
    {
        firstPlayer = XOR128.Next(4);
        parent = firstPlayer;
        round = 0;
        betCount = 0;
        parentCount = 0;
        score = 25000;
    }

    //ラウンド開始時の初期化
    public void Initialize_NewRound()
    {
        hand.Clear();
        call.Clear();
        table.Clear();
        turn = parent;
        direction = (id + 4 - parent) % 4;
        lastDiscard = Main.NULL;
        ShowRound();
        ShowDirection();
        ShowScore();
        ShowTurnIcon();
    }

    //ラウンド数表示
    private void ShowRound()
    {
        //場風表示
        fieldWindText.sprite =ImageObjects.japaneseImages.GetSprite(round / 4);
        //局数表示
        roundText.sprite =ImageObjects.normalNumberImages.GetSprite(round % 4 + 1);
        //リーチ棒の数を表示
        betCountText.sprite =ImageObjects.normalNumberImages.GetSprite(betCount%10);
        //n本場を表示
        parentCountText.sprite =ImageObjects.normalNumberImages.GetSprite(parentCount%10);
    }

    //自風表示
    private void ShowDirection()
    {
        seatWindText.sprite =ImageObjects.japaneseImages.GetSprite(direction);
    }

    //スコア表示
    private void ShowScore()
    {
        //得点を文字列に変換
        String score_str = score.ToString();
        Char[] score_chars = Enumerable.Range(0, score_str.Length).Select(i => score_str[i]).Reverse().ToArray();

        //得点を再表示
        Array.ForEach(scoreTexts, sr => sr.sprite = null);
        Enumerable.Range(0, score_chars.Length).ToList().
            ForEach(i => scoreTexts[i].sprite = 
            ImageObjects.normalNumberImages.GetSprite(int.Parse(score_chars[i].ToString())));
    }

    //手番表示
    static private void ShowTurnIcon()
    {
        //全て透明化
        Array.ForEach(turnIcon, sr => sr.color = new Color(1f, 0.5f, 1f, 0f));

        turnIcon[turn].color = new Color(1f, 0.5f, 1f, 1f);
    }

    //配牌
    public void Deal(List<int> deal)
    {
        //配牌を手牌に追加
        hand.AddRange(deal);

        //AIの手牌ならソート
        if (IsThisPlayingUser() == false)
        {
            hand.Sort();
        }

        //手牌表示
        ShowHand();
    }

    //ユーザー用のデータか(AIではないか)
    private bool IsThisPlayingUser()
    {
        return id == 0 && playing;
    }

    //手牌表示
    private void ShowHand()
    {
        //表示中の画像は削除
        Array.ForEach(handSpriteRenderers, sr => sr.sprite = null);

        //手札として存在する分だけ画像を表示
        Enumerable.Range(0,hand.Count).ToList().ForEach
            (i => handSpriteRenderers[i].sprite = ImageObjects.card.GetSprite(hand[i]));
    }

    //ドラッグ中の牌以外を薄く表示
    public void ShowHandAlpha(int index)
    {
        if (index <= hand.Count - 1)
        {
            Array.ForEach(handSpriteRenderers.Where((sr, i) => i != index).ToArray(),
                sr => sr.color = new Color(1f, 1f, 1f, 0.3f));
        }
    }

    //透明度を戻す
    public void ShowHandResetAlpha()
    {
        Array.ForEach(handSpriteRenderers, sr => sr.color = new Color(1f, 1f, 1f, 1f));
    }

    //手動のソート
    public void SelfSort(int drag,int drop)
    {
        //ツモ牌のソートは禁止
        int maxIndex = hand.Count - 1;
        if (hand.Count % 3 != 1)
        {
            maxIndex--;
        }

        if (drag <= maxIndex && drop <= maxIndex)
        {
            int card = hand[drag];
            hand.RemoveAt(drag);
            hand.Insert(drop,card);

            ShowHand();
        }
    }


    //牌を引く
    public void DrawCard(int card)
    {
        //引いた牌を手牌に追加
        hand.Add(card);

        //手牌表示
        ShowHand();
    }


    //プレイヤーのターンかつプレイヤーは参加しているか(入力待ちが必要か)
    static public bool IsPlayingUserTurn()
    {
        return turn == 0 && playing;
    }

    //AIの捨て牌決定
    public int AIDiscard()
    {
        return ai.ActionDecision_Discard(hand, call);
    }

    //プレイヤーが捨て牌可能か
    public bool CanPLDiscard()
    {
        return IsPlayingUserTurn() && hand.Count % 3 == 2;
    }

    //捨て牌をする
    public void Discard(int index)
    {
        //手牌から河に牌を移動
        lastDiscard= hand[index];
        table.Add(lastDiscard);
        hand.RemoveAt(index);

        //手牌と河を再表示
        ShowHand();
        ShowTableCard();
    }

    //捨て牌表示
    private void ShowTableCard()
    {
        //表示中の画像は削除
        Array.ForEach(tableSpriteRenderers, sr => sr.sprite = null);

        //捨て牌として存在する分だけ画像を表示
        Enumerable.Range(0, table.Count).ToList().ForEach
            (i => tableSpriteRenderers[i].sprite = ImageObjects.card.GetSprite(table[i]));
    }

    //手番であるか
    public bool IsThisPlayerTurn()
    {
        return turn == id;
    }

    //AIの副露決定
    public int[] AICall()
    {
        if (IsThisPlayingUser())
        {
            return new int[1];
        }
        else
        {
            return ai.ActionDecision_Call(hand, call, lastDiscard);
        }
    }

    //捨て牌で副露される
    public void Called()
    {
        //最終捨て牌を河から削除
        table.RemoveAt(table.Count - 1);

        //河を再表示
        ShowTableCard();
    }

    //ポン、カンをする
    public void Call(int[] indexes)
    {
        //手牌から副露牌に牌を移動

        //自身の手番にする
        turn = id;
        ShowTurnIcon();

        //手牌と副露牌を再表示
        ShowHand();

        //メッセージを表示
        ShowMessage("ポン");
    }

    //メッセージ表示
    private void ShowMessage(string key)
    {
        messageText.sprite = ImageObjects.messages.GetSprite(MessageImages.messageAndIndex[key]);
        messageText.color = new Color(1f, 1f, 1f, MessageImages.messageAndTime[key]);
    }

    //手番移動
    static public void NextTurn()
    {
        turn = (turn + 1) % 4;
        ShowTurnIcon();
    }

    //AIのツモorチー決定
    public int[] AIDraw()
    {
        return ai.ActionDecision_Draw(hand, call);
    }
}
