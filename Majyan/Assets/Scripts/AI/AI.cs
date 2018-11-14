using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AI {

    protected int discard;

    public abstract void DecideDiscard(List<int> list);

    public int GetDiscord()
    {
        return discard;
    }
}
