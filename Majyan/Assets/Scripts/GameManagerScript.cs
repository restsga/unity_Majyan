using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

    // Objects //
    public Sprite[] cardImages = new Sprite[37];
    public Sprite[] fontImages_ja = new Sprite[6];
    public Sprite[] fontImages_num = new Sprite[10];
    public Sprite[] scoreIconImages = new Sprite[2];
    private DeckManager deckManager = new DeckManager();
    private List<GameObject> cardObjects = new List<GameObject>();
    private List<GameObject> roundTextObjects = new List<GameObject>();
    private List<GameObject>[] scoreTextObjects = new List<GameObject>[4];
         
    // Constants //
    private const int NULL_ID = -1000;

    // Variable //
    private int[] scores = new int[4];
    private int startPlayer;
    private int round;
    private int parentCount;
    private int betCount;
    private bool parentStay;
    private bool increaseParentCount;
    
    // Cards //
    private List<int>[] hands = new List<int>[4];
    private List<int>[] tables = new List<int>[4];

	// Use this for initialization
	void Start () {

        //Random.SetSeed(DateTime.Now.Millisecond);

        for (int i = 0; i < 4; i++)
        {
            hands[i] = new List<int>();
            tables[i] = new List<int>();
            scoreTextObjects[i] = new List<GameObject>();
        }

        Initialize_NewGame();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Initialize_NewGame()
    {
        deckManager.Initialize_NewGame();

        for(int i = 0; i < scores.Length; i++)
        {
            scores[i] = 25000;
        }

        startPlayer = Random.Xor128_next(4);
        round = 0;
        parentCount = 0;
        betCount = 0;
        parentStay = true;
        increaseParentCount = false;

        Initialize_NextRound();
    }

    private void Initialize_NextRound()
    {
        deckManager.Initialize_NextRound();

        for (int i = 0; i < 4; i++)
        {
            hands[i].Clear();
            tables[i].Clear();
        }

        if (parentStay == false)
        {
            round++;
        }

        if (increaseParentCount)
        {
            parentCount++;
        }
        else
        {
            parentCount = 0;
        }

        ShowRound();

        Deal();
    }

    private void ShowRound()
    {
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

        for(int i = 0; i < texts.Length; i++)
        {
            GameObject text = new GameObject();
            text.AddComponent<SpriteRenderer>().sprite = texts[i];
            text.transform.localScale = Layouts.roundTextScales[i];
            text.transform.position = Layouts.roundTextPositons[i];
            text.transform.rotation = Quaternion.Euler(Layouts.roundTextRotations[i]);

            roundTextObjects.Add(text);
        }


        ShowScores_All();
    }

    private void ShowScore(int player)
    {
        //scoreTexts[player].GetComponent<Text>().text = 
        //Rules.PlayerIdToHouseString(startPlayer,round,player)+" "+ scores[player];

        GameObject compass = new GameObject();
        compass.AddComponent<SpriteRenderer>().sprite = fontImages_ja[Rules.PlayerIdToHouseId(startPlayer,round,player)];
        compass.transform.localScale = Layouts.scoreTextScales[player];
        compass.transform.position = Layouts.scoreTextOffsets[player];
        compass.transform.rotation = Quaternion.Euler(Layouts.scoreTextRotations[player]);

        scoreTextObjects[player].Add(compass);


        bool zero = false;
        int score = scores[player];

        for (int i=0, n = 100000; n >= 1;i++, n /= 10)
        {
            if (zero || score / n > 0 || n == 1)
            {
                zero = true;

                GameObject text = new GameObject();
                text.AddComponent<SpriteRenderer>().sprite = fontImages_num[score / n];
                text.transform.localScale = Layouts.scoreTextScales[player];
                text.transform.position = 
                    Layouts.scoreTextOffsets[player]+
                    Layouts.scoreTextCompassSpaces[player]+Layouts.scoreTextLineupDirections[player]*i;
                text.transform.rotation = Quaternion.Euler(Layouts.scoreTextRotations[player]);

                score -= (score / n) * n;
                scoreTextObjects[player].Add(text);
            }
        }
    }    

    private void ShowScores_All()
    {
        for(int i = 0; i < scores.Length; i++)
        {
            ShowScore(i);
        }
    }
    
    private void Deal()
    {
        for (int p = 0; p < hands.Length; p++) {
            for (int i = 0; i < 13;i++)
            {
                hands[p].Add(deckManager.DrawCard());
            }

            ShowHands(p);
        }
    }

    private void ShowHands(int player)
    {
        for (int i = 0; i < hands[player].Count; i++)
        {
            GameObject card = new GameObject();
            card.AddComponent<SpriteRenderer>().sprite = cardImages[Rules.IdChangeSerialToCard(hands[player][i])];
            card.transform.localScale = Layouts.handScales[player];
            card.transform.position = Layouts.handOffsets[player]+Layouts.handLineupDirections[player]*i;
            card.transform.rotation = Quaternion.Euler(Layouts.handRotations[player]);
            cardObjects.Add(card);
        }
    }

    private void ShowHands_All()
    {
        for(int i = 0; i < hands.Length; i++)
        {
            ShowHands(i);
        }
    }
}

public class DeckManager
{
    // Variable //
    private int deckUsedIndex;

    // Cards //
    private int[] deck = new int[136];

    public void Initialize_NewGame()
    {
        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = i;
        }
    }

    public void Initialize_NextRound()
    {
        deckUsedIndex = -1;
        Shuffle();
    }

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

    public int DrawCard()
    {
        deckUsedIndex++;
        return deck[deckUsedIndex];
    }
}


