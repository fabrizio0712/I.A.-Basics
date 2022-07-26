using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : FSMState<T>
{
    IMove entity;
    public IdleState(IMove Entity) 
    {
        entity = Entity;
    }
    public override void Awake()
    {
        Debug.Log("IdleState Awake");
    }
    public override void Execute()
    {
        entity.Move(Vector3.zero);
    }
    public override void Sleep()
    {
        Debug.Log("IdleState Sleep");
    }
}
