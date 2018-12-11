using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppInit : State
{
    public override State SetTimer()
    {
        timer = 0f;
        return this;
    }

    protected override State MainFunction()
    {
        //デバッグ用
        ForDebugFunction();

        //画像を読み込む
        ImageObjects.Initialize();

        //状態遷移用のオブジェクト群を初期化
        StateObjects.Initialize();

        //競技者が保持する情報群を初期化
        Array.ForEach(Main.players, player => player.Initialize_Application());

        return StateObjects.newGameInit();
    }

    private void ForDebugFunction()
    {
        //seed値決定と表示
        int seed = 491;//299
        //seed = DateTime.Now.Millisecond;
        XOR128.DecideInitial(seed);
        GameObject.Find("Canvas/SeedText").GetComponent<Text>().text = "" + seed;
    }
}
