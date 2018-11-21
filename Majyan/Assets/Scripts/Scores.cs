using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scores{

    // 画像 //
    private Sprite[] font_ja;       //日本語
    private Sprite[] font_num;     //数字

    // オブジェクト //
    private List<GameObject>[] textObjects = new List<GameObject>[4];  //得点表示用テキスト

    // 変数 //
    private int[] scores = new int[4];      //得点

	public void Initialize()
    {
        font_ja = LoadSprites.SortedNumbers_Multiple("fonts/japanese");
        font_num = LoadSprites.SortedNumbers_Multiple("fonts/numbers");

        for (int i = 0; i < textObjects.Length; i++)
        {
            textObjects[i] = new List<GameObject>();
        }

        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = 25000;
        }
    }
    
    //得点表示
    public void ShowScore(int player,int seatWind)
    {
        GameManagerScript.DestroyGameObjects(ref textObjects[player]);      //古いオブジェクトを削除

        //方角表示
        GameObject compass = new GameObject();
        compass.AddComponent<SpriteRenderer>().sprite =font_ja[seatWind];       //フォント画像を格納
        compass.transform.localScale = Layouts.scoreTextScales[player];             //大きさを決定
        compass.transform.position = Layouts.scoreTextOffsets[player];              //表示場所を決定
        compass.transform.rotation = Quaternion.Euler(Layouts.scoreTextRotations[player]);      //角度を指定

        textObjects[player].Add(compass);      //メモリ解放用のリストに格納


        //得点表示
        bool zero = false;              //頭の0を表示しないためのフラグ
        int score = scores[player];     //得点を取得
        for (int i = 0, n = 100000; n >= 1; i++, n /= 10)
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
                text.AddComponent<SpriteRenderer>().sprite = font_num[score / n];     //表示する数字を決定
                text.transform.localScale = Layouts.scoreTextScales[player];    //大きさを決定
                text.transform.position =
                    Layouts.scoreTextOffsets[player] +
                    Layouts.scoreTextCompassSpaces[player] +
                    Layouts.scoreTextLineupDirections[player] * i;            //表示場所を決定
                text.transform.rotation =
                    Quaternion.Euler(Layouts.scoreTextRotations[player]);   //角度を決定

                score -= (score / n) * n;               //表示した桁を0にする

                textObjects[player].Add(text);     //メモリ解放用のリストに格納
            }
        }
    }

    //全員分の得点を表示(派生関数)
    public void ShowScores_All(int[] seatWinds)
    {
        for (int i = 0; i < scores.Length; i++)
        {
            ShowScore(i,seatWinds[i]);
        }
    }
}
