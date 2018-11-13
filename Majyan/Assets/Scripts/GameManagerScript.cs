using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

    // Objects //
    public Sprite[] cardImages = new Sprite[37];
    private DeckManager deckManager = new DeckManager();
    private GameObject roundText;
    private GameObject scoreIcons;
    private GameObject[] scoreTexts=new GameObject[4];

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

        roundText = GameObject.Find("Canvas/RoundText");
        scoreIcons = GameObject.Find("Canvas/ScoreIcons");

        for(int i = 0; i < scoreTexts.Length; i++)
        {
            scoreTexts[i] = GameObject.Find("Canvas/ScoreText" + i);
        }

        //Random.SetSeed(DateTime.Now.Millisecond);

        for (int i = 0; i < 4; i++)
        {
            hands[i] = new List<int>();
            tables[i] = new List<int>();
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
        roundText.GetComponent<Text>().text = Rules.CountsToString(round, parentCount, betCount);

        ShowScores_All();
    }

    private void ShowScore(int player)
    {
        scoreTexts[player].GetComponent<Text>().text = 
            Rules.PlayerIdToHouseString(startPlayer,round,player)+" "+ scores[player];
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
            card.transform.localScale = Layouts.GetHandScale(player);
            card.transform.position = Layouts.GetHandOffset(player)+Layouts.GetHandLineupDirection(player)*i;
            card.transform.rotation = Quaternion.Euler(Layouts.GetHandRotation(player));
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


