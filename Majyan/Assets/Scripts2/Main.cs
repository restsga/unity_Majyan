using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour {

    //現在の状態(状態遷移用)
    private State state;

    //牌山情報
    static public Deck deck = new Deck();
    //各競技者が保持する情報群
    static public Player[] players = Enumerable.Range(0, 4).Select((i) => new Player(i)).ToArray();

    //null用の値
    static public readonly int NULL = -9999;

    // Use this for initialization
    void Start()
    {
        state = new AppInit();
    }

    // Update is called once per frame
    void Update()
    {
        state= state.Update(Time.deltaTime);
    }
}
