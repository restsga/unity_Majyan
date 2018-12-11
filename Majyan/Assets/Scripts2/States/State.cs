abstract public class State
{
    //タイマー
    protected float timer = 0f;

    //主機能
    protected abstract State MainFunction();
    //タイマーを設定
    public abstract State SetTimer();

    //フレーム毎に呼び出す
    public State Update(float deltaTime)
    {
        //待ち時間が終わったら主機能の関数を呼び、遷移後の状態を返す
        timer -= deltaTime * Main.gameSpeed;
        if (timer <= 0)
        {
            return MainFunction();
        }

        //待ち時間が終わっていない場合は遷移は行わない
        return this;
    }
}

static public class StateObjects
{
    public delegate State StateSetTimer();
    static public WaitInput waitInput_object;
    static public StateSetTimer waitInput;
    static private NewGameInit newGameInit_object;
    static public StateSetTimer newGameInit;
    static private NewRoundInit newRoundInit_object;
    static public StateSetTimer newRoundInit;
    static private Deal deal_object;
    static public StateSetTimer deal;
    static private Draw draw_object;
    static public StateSetTimer draw;
    static private AIDiscard aiDiscard_object;
    static public StateSetTimer aiDiscard;
    static private Call call_object;
    static public StateSetTimer call;
    static private AIDraw aiDraw_object;
    static public StateSetTimer aiDraw;

    static public void Initialize()
    {
        waitInput_object = new WaitInput();
        waitInput = waitInput_object.SetTimer;
        newGameInit_object = new NewGameInit();
        newGameInit = newGameInit_object.SetTimer;
        newRoundInit_object = new NewRoundInit();
        newRoundInit = newRoundInit_object.SetTimer;
        deal_object = new Deal();
        deal = deal_object.SetTimer;
        draw_object = new Draw();
        draw = draw_object.SetTimer;
        aiDiscard_object = new AIDiscard();
        aiDiscard = aiDiscard_object.SetTimer;
        call_object = new Call();
        call = call_object.SetTimer;
        aiDraw_object = new AIDraw();
        aiDraw = aiDraw_object.SetTimer;
    }
}