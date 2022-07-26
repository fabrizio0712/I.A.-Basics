using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : INode
{
    public delegate void myDelegate();
    myDelegate action;
    public ActionNode(myDelegate Action) 
    {
        action = Action;
    }
    public void SubAction(myDelegate newAction) 
    {
        action += newAction;
    }
    public void Execute()
    {
        action();
    }
}
