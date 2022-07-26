using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject exclamation;
    [SerializeField] private GameObject dot1;
    [SerializeField] private GameObject dot2;
    [SerializeField] private GameObject dot3;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bullet;
    [SerializeField] private List<Transform> path;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rangeSight;
    [SerializeField] private float angleSight;
    [SerializeField] private float distanceToPathPoint;
    [SerializeField] private float idleTime;
    [SerializeField] private float speed;
    [SerializeField] private float turnSmoothVelocity;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float predictionTime;
    [SerializeField] private bool patrolOpen;
    private Rigidbody rigidbody;
    private int nextPoint;
    private int indexModifier = 1;
    private float idleCurrentTime;
    private ActionNode _shoot;
    private ActionNode _pursuit;
    private QuestionNode _rangeToShoot;
    private float bugTimer;
    private float currentBugTimer;
    private bool shootEnable;
    [SerializeField]private float shootTimer;
    private float currentShootTimer;
    private enum State {idle, patrol, exalted }
    private State state;
    void Start()
    {
        bullet.SetActive(false);
        bugTimer = 1;
        rigidbody = GetComponent<Rigidbody>();
        exclamation.SetActive(false);
        state = State.idle;
        transform.position = path[nextPoint].position;
        _shoot = new ActionNode(Shoot);
        _pursuit = new ActionNode(Pursuit);
        _rangeToShoot = new QuestionNode(RangeToShoot, _shoot, _pursuit);
    }
    void Update()
    {
        if (!shootEnable) 
        {
            if (currentShootTimer < shootTimer)
            {
                currentShootTimer += Time.deltaTime;
            }
            else 
            {
                currentShootTimer = 0;
                shootEnable = true;
            }
        }
        if (state == State.idle || state == State.patrol)
        {
            if (IsInSight(player.transform.position))
            {
                state = State.exalted;
                idleCurrentTime = 0;
                dot1.SetActive(false);
                dot2.SetActive(false);
                dot3.SetActive(false);
            }
        }
        else
        {
            if (!IsInSight(player.transform.position))
            {
                state = State.idle;
            }
        }
        StateMachine((int)state);
    }
    private void StateMachine(int s) 
    {
        switch (s) 
        {
            case 0:
                exclamation.SetActive(false);
                rigidbody.velocity = Vector3.zero;
                if (idleCurrentTime < idleTime)
                {
                    idleCurrentTime += Time.deltaTime;
                    if (idleCurrentTime > idleTime / 4) dot1.SetActive(true);
                    if (idleCurrentTime > idleTime / 2) dot2.SetActive(true);
                    if (idleCurrentTime > 3 * idleTime / 4) dot3.SetActive(true);
                }
                else 
                {
                    idleCurrentTime = 0;
                    dot1.SetActive(false);
                    dot2.SetActive(false);
                    dot3.SetActive(false);
                    state = State.patrol;
                }
                break;
            case 1:
                exclamation.SetActive(false);
                PatrolLogic();
                break;
            case 2:
                exclamation.SetActive(true);
                _rangeToShoot.Execute();
                break;
        }
    }
    public void Move(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            float _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, _angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward;
            rigidbody.velocity = ((transform.forward + moveDirection).normalized * speed + new Vector3(0, rigidbody.velocity.y, 0));
        }
        else
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
    public void PatrolLogic() 
    {
        var point = path[nextPoint];
        var posPoint = point.position;
        posPoint.y = transform.position.y;
        Vector3 dir = point.position - transform.position;
        if (dir.magnitude < distanceToPathPoint)
        {
            if (patrolOpen)
            {
                if (nextPoint + indexModifier > path.Count - 1 || nextPoint + indexModifier < 0)
                {
                    indexModifier *= -1;
                    nextPoint += indexModifier;
                    state = State.idle;
                }
                else
                {
                    nextPoint += indexModifier;
                }
            }
            else
            {
                if (nextPoint < path.Count - 1)
                {
                    nextPoint++;
                    state = State.idle;
                }
                else
                {
                    nextPoint = 0;
                    state = State.idle;
                }
            }
        }
        else 
        {
            Move(dir.normalized);
        }
    }
    private bool IsInSight(Vector3 playerPos) 
    {
        Vector3 difference = playerPos - transform.position;
        float distance = difference.magnitude;
        if (distance > rangeSight) return false;
        float angleToTarget = Vector3.Angle(transform.forward, difference);
        if (angleToTarget > angleSight / 2) return false;
        if (Physics.Raycast(transform.position, difference.normalized, distance, layerMask)) return false;
        return true;
    }
    private void Pursuit() 
    {
        Vector3 direction = player.transform.position - transform.position;
        transform.forward = direction;
        Move(direction);
    }
    private void Shoot() 
    {
        rigidbody.velocity = Vector3.zero;
        Vector3 direction = player.transform.position - transform.position;
        transform.forward = direction;
        if (!bullet.active && shootEnable) 
        {
            bullet.transform.forward = direction;
            bullet.transform.position = transform.position + new Vector3(0,1,0);
            bullet.SetActive(true); 
        }
        Debug.Log("Shooting");
    }
    private bool RangeToShoot() 
    {
        Vector3 diff = player.transform.position - transform.position;
        if (diff.magnitude < 6)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rangeSight);
        Gizmos.DrawWireSphere(transform.position, rangeSight);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angleSight / 2, 0) * transform.forward * rangeSight);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angleSight / 2, 0) * transform.forward * rangeSight);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            transform.position = path[nextPoint].position;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall") 
        {
            if (currentBugTimer < bugTimer)
            {
                currentBugTimer += Time.deltaTime;
            }
            else 
            {
                currentBugTimer = 0;
                transform.position = path[nextPoint].position;
            }
        }
    }
}
