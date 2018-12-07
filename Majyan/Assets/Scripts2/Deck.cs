using System.Linq;
using System.Collections.Generic;

public class Deck
{
    //牌山、嶺上牌、ドラ表示牌
    List<int> deck = new List<int>();
    List<int> replacements = new List<int>();
    int[] bonusCards = new int[10];

    int openedBonusCount;

    public void Initialize()
    {
        //牌山、嶺上牌、ドラ表示牌を初期化
        deck.Clear();
        replacements.Clear();
        bonusCards = Enumerable.Repeat(Main.NULL, 10).ToArray();

        //牌の生成とシャッフル
        List<int> cardId = Enumerable.Range(0, 136).ToList();
        List<uint> randomId = Enumerable.Range(0, 136).Select(i => XOR128.Random()).ToList();
        deck= cardId.OrderBy(i=>randomId[i]).Select(i => CreateCardId(i)).ToList();

        //嶺上牌を確保
        replacements = deck.GetRange(0, 4);
        deck.RemoveRange(0, 4);

        //ドラ表示牌を確保
        bonusCards = deck.GetRange(0, 10).ToArray();
        deck.RemoveRange(0, 10);

        //ドラ表示牌を1枚公開
        openedBonusCount = 1;
    }

    //配牌
    public List<int> Deal()
    {
        //牌山の牌の一部を手牌(のための戻り値用List)に移す
        List<int> hand = new List<int>();
        hand = deck.GetRange(0, 13);
        deck.RemoveRange(0, 13);

        return hand;
    }

    /* 連番を牌IDに変換
     * 
     * 通常の牌は偶数、赤ドラは奇数
     */
    private int CreateCardId(int i)
    {
        //牌の種類
        int kind = i / 4;

        //通常の牌:2n+0(偶数)
        int red = 0;
        if (kind < 3 * 9 && kind % 9 == 5 - 1 && i % 4 == 0)
        {
            //赤ドラ:2n+1(奇数)
            red = 1;
        }

        return kind * 2 + red;
    }
}
