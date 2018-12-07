abstract public class State{

    //待ち時間
    protected float timer=0f;

    //主機能
    protected abstract State MainFunction();

    //フレーム毎に呼び出す
    public State Update(float deltaTime)
    {
        //待ち時間が終わったら主機能の関数を呼び、遷移後の状態を返す
        timer -= deltaTime;
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
    static public WaitInput waitInput;
    static public NewGameInit newGameInit;
    static public NewRoundInit newRoundInit;
    static public Deal deal;

    static public void Initialize()
    {
        waitInput = new WaitInput();
        newGameInit = new NewGameInit();
        newRoundInit = new NewRoundInit();
        deal = new Deal();
    }
}