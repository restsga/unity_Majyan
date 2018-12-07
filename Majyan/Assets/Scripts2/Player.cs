using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    // 共通の情報 //
    //起家
    static private int firstPlayer;
    //手番
    static private int turn;

    // 個別の情報 //
    //競技者ID
    private int id;
    //手牌、副露牌、捨て牌
    private List<int> hand = new List<int>();
    private List<int[]> call = new List<int[]>();
    private List<int> table = new List<int>();
    //得点
    private int score=25000;

    //手牌表示用オブジェクト
    private SpriteRenderer[] handSpriteRenderers = new SpriteRenderer[14];

    public Player(int id)
    {
        this.id = id;
    }

    //アプリケーション起動時の初期化
    public void Initialize_Application()
    {
        //手牌表示用のSpriteRendererを取得
        handSpriteRenderers = 
            Enumerable.Range(0, 14).
            Select(i => GameObject.Find("UIObjects/Hands/PL (" + id + ")/Card (" + i + ")").
            GetComponent<SpriteRenderer>()).ToArray();
    }

    //ゲーム開始時の初期化
    public void Initialize_NewGame()
    {
        score = 25000;
    }

    //ラウンド開始時の初期化
    public void Initialize_NewRound()
    {
        hand.Clear();
        call.Clear();
        table.Clear();
    }

    //配牌
    public void Deal(List<int> deal)
    {
        //配牌を手牌に追加
        hand.AddRange(deal);

        //手牌表示
        ShowHand();
    }

    //手牌表示
    private void ShowHand()
    {
        Array.ForEach(handSpriteRenderers, sr => sr.sprite = null);
        Enumerable.Range(0,hand.Count).ToList().ForEach
            (i => handSpriteRenderers[i].sprite = ImageObjects.card.GetSprite(hand[i]));
    }
}
