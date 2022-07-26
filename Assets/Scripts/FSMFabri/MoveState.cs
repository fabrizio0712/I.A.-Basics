using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState<T> : FSMState<T>
{
    IMove entity;
    public MoveState(IMove Entity) 
    {
        entity = Entity;
    }
    public override void Execute()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(horizontal, 0, vertical).normalized;
        entity.Move(dir);
    }
    public override void Sleep()
    {
        entity.Move(Vector3.zero);
    }
}
