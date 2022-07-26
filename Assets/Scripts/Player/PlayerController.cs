using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private FSM<string> fsm;
    private Player player;
    private Rigidbody rigidbody;
    public bool moveEnable;

    void Start()
    {
        moveEnable = false;
        rigidbody = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        fsm = new FSM<string>();
        IdleState<string> idle = new IdleState<string>(player);
        MoveState<string> move = new MoveState<string>(player);
        idle.AddTransition("Move", move);
        move.AddTransition("Idle", idle);
        fsm.SetInit(idle);
    }

    void Update()
    {
        if (moveEnable)
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            if (h != 0 || v != 0)
            {
                fsm.Transition("Move");
            }
            if (Mathf.Abs(h) <= 0.1f && Mathf.Abs(v) <= 0.1f && Mathf.Abs(rigidbody.velocity.y) <= 0.1f)
            {
                fsm.Transition("Idle");
            }
            fsm.OnUpdate();
        }
    }
}
