using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator _animator;
    private int isAttackingHash;
    private int isDeadHash;
    private int isWalkingHash;
    public Transform targetPosition;
    public ArcherController archer;
    public float pLerp = .01f;
    public float rLerp = .02f;

    float detectRange = 5;
    float attackRange = 1;
    float speed = 0.01f;

    private State currentState;

    enum State
    {
        Idle,
        Walking,
        Attacking,
        Dead
    };

    void Start()
    {
        isDeadHash = Animator.StringToHash("dead");
        isAttackingHash = Animator.StringToHash("attacking");
        isWalkingHash = Animator.StringToHash("walking");
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToArcher();
        Attack();
    }

    private void MoveToArcher()
    {
        // transform.position = Vector3.Lerp(transform.position, archer.transform.position, pLerp);
        // transform.rotation = Quaternion.Lerp(transform.rotation, archer.transform.rotation, rLerp);



        float dist = Vector3.Distance(archer.transform.position, transform.position);
        //check if it is within the range you set
        if (dist <= detectRange)
        {
            // Debug.Log("in detectRange dist=" + dist);
            if (currentState == State.Idle)
            {
                ChangeState(State.Walking);
            }

            if (currentState == State.Walking)
            {
                // transform.LookAt(archer.transform);
                Quaternion targetRotation =
                    Quaternion.LookRotation((archer.transform.position - transform.position).normalized);
                targetRotation.z = 0;
                float sharpness = 0.1f;
                float blend = 1f - Mathf.Pow(1f - sharpness, 30f * Time.deltaTime);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, blend);
                transform.position = Vector3.MoveTowards(transform.position, archer.transform.position, speed);
            }
        }
        else if (currentState == State.Walking)
        {
            ChangeState(State.Idle);
        }
    }

    void Attack()
    {
        if (currentState == State.Attacking)
        {
            if (archer.isDead)
            {
                ChangeState(State.Idle);
                return;
            }

            float dist = Vector3.Distance(archer.transform.position, transform.position);
            if (dist >= attackRange)
            {
                if (dist <= detectRange)
                {
                    ChangeState(State.Walking);
                }
                else
                {
                    ChangeState(State.Idle);
                }
            }
        }
    }

    void DamageArcher()
    {
        archer.GetDamage(25);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ZombieController => OnTriggerEnter");
        ArrowController arrow = other.gameObject.GetComponent<ArrowController>();
        ArcherController archer = other.gameObject.GetComponent<ArcherController>();
        if (arrow)
        {
            Debug.Log("ZombieController => OnTriggerEnter ArrowController");
            ChangeState(State.Dead);
        }
        else if (archer)
        {
            ChangeState(State.Attacking);
        }

    }

    void ChangeState(State state)
    {
        switch (state)
        {
            case State.Idle:
                _animator.SetBool(isWalkingHash, false);
                _animator.SetBool(isDeadHash, false);
                _animator.SetBool(isAttackingHash, false);
                break;
            case State.Walking:
                _animator.SetBool(isWalkingHash, true);
                _animator.SetBool(isDeadHash, false);
                _animator.SetBool(isAttackingHash, false);
                break;
            case State.Attacking:
                _animator.SetBool(isWalkingHash, false);
                _animator.SetBool(isDeadHash, false);
                _animator.SetBool(isAttackingHash, true);
                break;
            case State.Dead:
                _animator.SetBool(isWalkingHash, false);
                _animator.SetBool(isDeadHash, true);
                _animator.SetBool(isAttackingHash, false);
                break;
        }

        currentState = state;
    }

    void Destroy()
    {
        
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("OnControllerColliderHit");
        if (hit.gameObject.GetComponent<ArrowController>())
        {
            _animator.SetBool(isDeadHash, true);
        }
    }
}