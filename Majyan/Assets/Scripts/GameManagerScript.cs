using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

    // Objects //
    public Sprite[] cardImages = new Sprite[38];
    public Sprite[] fontImages_ja = new Sprite[6];
    public Sprite[] fontImages_num = new Sprite[10];
    public Sprite[] scoreIconImages = new Sprite[2];
    private DeckManager deckManager = new DeckManager();
    private List<GameObject>[] handObjects = new List<GameObject>[4];
    private List<GameObject>[] tableObjects = new List<GameObject>[4];
    private List<GameObject>[] callObjects = new List<GameObject>[4];
    private List<GameObject> roundTextObjects = new List<GameObject>();
    private List<GameObject>[] scoreTextObjects = new List<GameObject>[4];
    private AI[] ai = { new Draw_Discard(), new Draw_Discard(), new Draw_Discard(), new Draw_Discard() };     

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

    private int turnPlayer;

    private delegate void NextMethod();
    private NextMethod nextMethod;
    private float timer;
    
    // Cards //
    private List<int>[] hands = new List<int>[4];
    private List<int>[] tables = new List<int>[4];
    private List<CallCardsSet>[] callCards = new List<CallCardsSet>[4];

	// Use this for initialization
	void Start () {

        //Random.SetSeed(DateTime.Now.Millisecond);
        Random.SetSeed(10048);
        //kan:10006,10008,10012,10025
        //ankan:10029,10033
        //kakan:10048

        for (int i = 0; i < 4; i++)
        {
            hands[i] = new List<int>();
            tables[i] = new List<int>();
            callCards[i] = new List<CallCardsSet>();

            handObjects[i] = new List<GameObject>();
            tableObjects[i] = new List<GameObject>();
            callObjects[i] = new List<GameObject>();
            scoreTextObjects[i] = new List<GameObject>();
        }

        Initialize_NewGame();
	}
	
	// Update is called once per frame
	void Update () {

        if (nextMethod != null)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                nextMethod();
            }
        }
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

        nextMethod = null;
        timer = 0f;

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

        turnPlayer = Rules.HouseIdToPlayerId(startPlayer, round, 0);

        nextMethod= Deal;
        timer = 0f;
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

            hands[p].Sort();

            ShowOrHideHand_Only(p);
        }

        nextMethod = DrawCard;
        timer = Times.Wait_DealToDraw();
    }

    private void ShowHand_Only(int player,bool show)
    {
        DestroyCardObjects(ref handObjects[player]);
        for (int i = 0; i < hands[player].Count; i++)
        {
            GameObject card = new GameObject();
            Sprite sprite;
            if (show)
            {
                sprite = cardImages[Rules.IdChangeSerialToCardImageId(hands[player][i])];
            }
            else
            {
                sprite = cardImages[37];
            }
            card.AddComponent<SpriteRenderer>().sprite = sprite;
            card.transform.localScale = Layouts.handScales[player];
            card.transform.position = Layouts.handOffsets[player]+Layouts.handLineupDirections[player]*i;
            card.transform.rotation = Quaternion.Euler(Layouts.handRotations[player]);
            handObjects[player].Add(card);
        }
    }

    private void ShowOrHideHand_Only(int player)
    {
        bool[] shows = { true, true, true, true };
        ShowHand_Only(player, shows[player]);
    }

    private void ShowAndHideHands(bool[] shows)
    {
        for (int i = 0; i < hands.Length; i++)
        {
            ShowHand_Only(i, true);
        }
    }

    private void ShowHands_All()
    {
        bool[] shows = { true, true, true, true};
        ShowAndHideHands(shows);
    }

    private void ShowAndHideHands_Default()
    {
        bool[] shows = { true, false, false, false };
        ShowAndHideHands(shows);
    }

    private void DestroyCardObjects(ref List<GameObject> cardObjects)
    {
        for (int i = cardObjects.Count-1; 0 <= i; i--)
        {
            Destroy(cardObjects[i]);
        }

        cardObjects.Clear();
    }

    private void DrawCard()
    {
        hands[turnPlayer].Add(deckManager.DrawCard());

        ShowHand_Only(turnPlayer, true);

        ai[turnPlayer].DecideDiscard(hands[turnPlayer]);

        nextMethod = Discard;
        timer = Times.Wait_DrawToDiscard();
    }

    private void Discard()
    {
        int discardIndex = ai[turnPlayer].GetDiscord();
        tables[turnPlayer].Add(hands[turnPlayer][discardIndex]);
        hands[turnPlayer].RemoveAt(discardIndex);

        ShowOrHideHand_Only(turnPlayer);
        ShowTableCard_Only(turnPlayer);

        nextMethod = Call_Rob;
        timer = Times.Wait_Call();
    }

    private void ShowTableCard_Only(int player)
    {
        DestroyCardObjects(ref tableObjects[player]);

        for (int i = 0; i < tables[player].Count; i++)
        {
            GameObject card = new GameObject();
            Sprite sprite = cardImages[Rules.IdChangeSerialToCardImageId(tables[player][i])];
            card.AddComponent<SpriteRenderer>().sprite = sprite;
            card.transform.localScale = Layouts.tableScales[player];
            card.transform.position =
                Layouts.tableOffsets[player] +
                Layouts.tableLineupNextDirections[player] * (i%6)+
                Layouts.tableLineupNewLineDirections[player] * (i / 6);
            card.transform.rotation = Quaternion.Euler(Layouts.tableRotations[player]);
            tableObjects[player].Add(card);
        }
    }

    private void ShowTableCards_All()
    {
        for(int i = 0; i < tables.Length; i++)
        {
            ShowTableCard_Only(i);
        }
    }

    private void Call_Rob()
    {
        int discard = tables[turnPlayer][tables[turnPlayer].Count - 1];

        for (int p = 0; p < hands.Length; p++)
        {
            if (p != turnPlayer)
            {
                bool[] pon_kan = Rules.CanPonOrKan(hands[p], discard);

                if (pon_kan[1])
                {
                    if (ai[p].DecideKan(hands[p]))
                    {
                        int[] cards = new int[4];
                        int count = 0;

                        for (int i = hands[p].Count - 1; 0 <= i; i--)
                        {
                            if (Rules.Same_BonusEquate(hands[p][i], discard))
                            {
                                cards[count] = hands[p][i];
                                hands[p].RemoveAt(i);
                                count++;
                            }
                        }

                        cards[3] = discard;
                        tables[turnPlayer].RemoveAt(tables[turnPlayer].Count - 1);
                        callCards[p].Add(new CallCardsSet());
                        callCards[p][callCards[p].Count - 1].Kan(cards, p, turnPlayer);

                        ShowOrHideHand_Only(p);
                        ShowTableCard_Only(turnPlayer);
                        ShowCallCard_Only(p);

                        ai[p].DecideDiscard(hands[p]);
                        turnPlayer = p;

                        nextMethod = Discard;
                        timer = Times.Wait_DrawToDiscard();
                        return;
                    }
                }
                if (pon_kan[0])
                {
                    bool bonus=true;

                    if (ai[p].DecidePon(hands[p],ref bonus))
                    {
                        int[] cards = new int[3];
                        int count = 0;

                        for (int i = hands[p].Count - 1; 0 <= i; i--)
                        {
                            if (Rules.Same_BonusEquate(hands[p][i], discard))
                            {
                                cards[count] = hands[p][i];
                                hands[p].RemoveAt(i);
                                count++;
                            }
                        }
                        if (count >= 3)
                        {
                            int indexAdd = 0;
                            if (bonus == false)
                            {
                                indexAdd = 2;
                            }
                            for (int i = 0; i < cards.Length; i++)
                            {
                                if (Rules.Bonus5(cards[i]))
                                {
                                    int keep = cards[0 + indexAdd];
                                    cards[0 + indexAdd] = cards[i];
                                    cards[i] = keep;
                                    break;
                                }
                            }
                        }

                        cards[2] = discard;
                        tables[turnPlayer].RemoveAt(tables[turnPlayer].Count - 1);
                        callCards[p].Add(new CallCardsSet());
                        callCards[p][callCards[p].Count - 1].Pon(cards, p, turnPlayer);

                        ShowOrHideHand_Only(p);
                        ShowTableCard_Only(turnPlayer);
                        ShowCallCard_Only(p);

                        ai[p].DecideDiscard(hands[p]);
                        turnPlayer = p;

                        nextMethod = Discard;
                        timer = Times.Wait_DrawToDiscard();
                        return;
                    }
                }
            }
        }

        int tiPlayer = (turnPlayer + 1) % 4;
        if (Rules.CanTi(hands[tiPlayer], discard))
        {
            int[] indexes = new int[2];
            if (ai[tiPlayer].DecideTi(hands[tiPlayer], ref indexes,discard))
            {
                Array.Sort(indexes);
                Array.Reverse(indexes);
                    int[] cards = new int[3];

                    for (int i = 0; i<indexes.Length; i++)
                    {
                            cards[i] = hands[tiPlayer][indexes[i]];
                            hands[tiPlayer].RemoveAt(indexes[i]);
                    }

                    cards[2] = discard;
                    tables[turnPlayer].RemoveAt(tables[turnPlayer].Count - 1);
                    callCards[tiPlayer].Add(new CallCardsSet());
                    callCards[tiPlayer][callCards[tiPlayer].Count - 1].Ti(cards, tiPlayer, turnPlayer);

                    ShowOrHideHand_Only(tiPlayer);
                    ShowTableCard_Only(turnPlayer);
                    ShowCallCard_Only(tiPlayer);

                    ai[tiPlayer].DecideDiscard(hands[tiPlayer]);
                    turnPlayer = tiPlayer;

                    nextMethod = Discard;
                    timer = Times.Wait_DrawToDiscard();
                    return;
                }
        }

        nextMethod = NextTurn;
        timer = Times.Wait_NextTurn();
    }

    private void ShowCallCard_Only(int player)
    {
        DestroyCardObjects(ref callObjects[player]);

        Vector2 position = new Vector2(Layouts.callOffsets[player].x, Layouts.callOffsets[player].y);
        
        for (int s = 0; s < callCards[player].Count; s++)
        {
            for (int i = 0; i< callCards[player][s].callCards.Count; i++)
            {
                CallCard callCard = callCards[player][s].callCards[i];
                GameObject card = new GameObject();
                Sprite sprite = cardImages[Rules.IdChangeSerialToCardImageId(callCard.card)];
                card.AddComponent<SpriteRenderer>().sprite = sprite;
                card.transform.localScale = Layouts.callScales[player];
                float addDirection;
                Vector3 addRotation ;
                if (callCard.discardPlayer == player)
                {
                    addDirection = 0f;
                    addRotation = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    position += Layouts.callLineupRotatedAddDirections[player];
                    addDirection = 1.0f;
                    addRotation = new Vector3(0f, 0f, 90f);
                }
                card.transform.position = position;
                position += 
                    Layouts.callLineupDirections[player]+ Layouts.callLineupRotatedAddDirections[player]*addDirection;
                card.transform.rotation =
                    Quaternion.Euler(Layouts.callRotations[player]+addRotation);

                callObjects[player].Add(card);
            }
        }
    }

    private void NextTurn()
    {
        turnPlayer = (turnPlayer + 1) % 4;
        DrawCard();
    }
}

public class DeckManager
{
    // Variable //
    private int deckUsedIndex;

    // Cards //
    private int[] deck = new int[136];
    /*private int[] deck = {0,1,2,4,5,6,8,9,10,12,13,14,16,
        20,21,22,24,25,26,28,29,30,32,33,34,36,
    40,41,42,44,45,46,48,49,50,52,53,54,56,
    60,61,62,64,65,66,68,69,70,72,73,74,76,
    3,7,11,15,17,18,19,23,27,31,35,37,38,39,43,47,51,55,57,58,59,63,67,71,75,77,78,79,80};*/
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

public class CallCard
{
    public int card;
    public int discardPlayer;
}

public class CallCardsSet
{
    public List<CallCard> callCards = new List<CallCard>();

    public void Pon(int[] cardsId,int callPlayer, int discardPlayer)
    {
        int count = 0;
        for(int i = 0; i < 3; i++)
        {
            CallCard callCard= new CallCard();

            if ((i + callPlayer + 1) % 4 == discardPlayer)
            {
                callCard.card = cardsId[2];
                callCard.discardPlayer = discardPlayer;
            }
            else
            {
                callCard.card = cardsId[count];
                count++;
                callCard.discardPlayer = callPlayer;
            }

            callCards.Add(callCard);
        }
    }

    public void Kan(int[] cardsId, int callPlayer, int discardPlayer)
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            CallCard callCard = new CallCard();

            if ((i + callPlayer + 1) % 4 == discardPlayer)
            {
                callCard.card = cardsId[3];
                callCard.discardPlayer = discardPlayer;
            }
            else
            {
                callCard.card = cardsId[count];
                count++;
                callCard.discardPlayer = callPlayer;
            }

            if (i >= 3)
            {
                callCards.Insert(1, callCard);
            }
            else
            {
                callCards.Add(callCard);
            }
        }
    }

    public void Ti(int[] cardsId, int callPlayer, int discardPlayer)
    {
        int count = 0;
        for (int i = 0; i < 3; i++)
        {
            CallCard callCard = new CallCard();

            if ((i + callPlayer + 1) % 4 == discardPlayer)
            {
                callCard.card = cardsId[2];
                callCard.discardPlayer = discardPlayer;
            }
            else
            {
                callCard.card = cardsId[count];
                count++;
                callCard.discardPlayer = callPlayer;
            }

            callCards.Add(callCard);
        }
    }
}