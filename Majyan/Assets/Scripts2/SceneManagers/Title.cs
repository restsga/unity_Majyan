using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //ゲーム開始ボタンをクリック時に実行
    public void OnClick_StartButton()
    {
        //乱数の初期値(seed)を決定
        int time= DateTime.Now.Second*1000+ DateTime.Now.Millisecond;
        //XOR128.DecideInitial(time);

        //メインのシーンを呼び出す
        SceneManager.LoadSceneAsync("Main");
    }
}
