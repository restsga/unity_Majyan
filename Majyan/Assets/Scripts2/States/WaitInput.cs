using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitInput : State
{
    public WaitInput()
    {
        timer = 0f;
    }

    protected override State MainFunction()
    {
        return this;
    }
}
