using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phases {

    // 画像 //
    private Sprite[] font_ja;       //日本語
    private Sprite[] font_num;      //数字
    private Sprite[] scoreSticks;   //点棒
    
    // オブジェクト //
    private List<GameObject> textObjects = new List<GameObject>();     //局数表示用テキスト

    // 変数 //
    private int startPlayer;            //起家
    private int round;                  //局数
    private int parentCount;            //n本場
    private int betCount;               //リーチ棒の数

        private int turnPlayer;     //手番プレイヤー

    public int GetTurn()
    {
        return turnPlayer;
    }

    public void Initialize_NewGame()
    {
        font_ja = LoadSprites.SortedNumbers_Multiple("fonts/japanese");
        font_num = LoadSprites.SortedNumbers_Multiple("fonts/numbers");
        scoreSticks = LoadSprites.SortedNumbers_Multiple("scoreSticks");
        
        startPlayer = Random.Xor128_next(4);    //起家を決定
        round = 0;          //局数
        parentCount = 0;    //n本場
        betCount = 0;       //リーチ棒

        Initialize_NextRound();
    }

    public void Initialize_NextRound()
    {
        turnPlayer = HouseIdToPlayerId(0);    //東家のプレイヤーの手番で開始

        ShowRound();
    }

    public void EndRound(bool parentStay,bool increaseParentCount,int addBetCount)
    {
        //親が流れた場合は局数のカウントを追加
        if (parentStay == false)
        {
            round++;
        }

        if (increaseParentCount)
        {
            parentCount++;      //n本場を増加
        }
        else
        {
            parentCount = 0;    //n本場をリセット
        }

        betCount += addBetCount;
    }

    public void ShowRound()
    {
        GameManagerScript.DestroyGameObjects(ref textObjects);      //古いオブジェクトを削除

        //表示する文字を格納
        Sprite[] texts = new Sprite[9];
        texts[0] = font_ja[round / 4];          //方角
        texts[1] = font_num[round % 4 + 1];     //局数
        texts[2] = font_ja[4];                  //「局」
        texts[3] = scoreSticks[0];              //100点棒
        texts[4] = font_ja[5];                  //積算記号
        texts[5] = font_num[parentCount % 10];  //n本場
        texts[6] = scoreSticks[1];              //1000点棒
        texts[7] = font_ja[5];                  //積算記号
        texts[8] = font_num[betCount % 10];     //リーチ棒の数

        //ゲームオブジェクトとしてパラメータを設定＆文字を表示
        for (int i = 0; i < texts.Length; i++)
        {
            GameObject text = new GameObject();
            text.AddComponent<SpriteRenderer>().sprite = texts[i];      //フォント画像を格納
            text.transform.localScale = Layouts.roundTextScales[i];     //大きさを決定
            text.transform.position = Layouts.roundTextPositons[i];     //表示場所を決定
            text.transform.rotation = Quaternion.Euler(Layouts.roundTextRotations[i]);      //角度を指定

            textObjects.Add(text);     //メモリ解放用のリストに格納
        }
    }

    //場風
    public int FieldWind()
    {
        return round / 4;
    }

    //自風
    public int PlayerIdToSeatWind(int player)
    {
        //return (Math.Abs(startPlayer - (player + 4)) + round) % 4;
        return Math.Abs((startPlayer + round) % 4 - (player + 4)) % 4;
    }

    //方角idをプレイヤーidに変換
    public int HouseIdToPlayerId(int house)
    {
        return (startPlayer + round + house) % 4;
    }

    //全員分の自風
    public int[] SeatWinds()
    {
        int[] seatWinds = new int[4];

        for(int i = 0; i < seatWinds.Length; i++)
        {
            seatWinds[i] = PlayerIdToSeatWind(i);
        }

        return seatWinds;
    }

    //通常の手番移動
    public void ChangeTurn_Default()
    {
        turnPlayer = (turnPlayer + 1) % 4;  //手番移動
    }

    //鳴きによる手番移動
    public void ChangeTurn_Call(int callPlayer)
    {
        turnPlayer = callPlayer;
    }

}
