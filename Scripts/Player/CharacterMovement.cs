using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float dashDistance = 4f;
    [Space(5)]

    [Header("Attacking")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] public float maxAttackTime = 0.2f;
    [SerializeField] public float minAttackTime = 0.025f;
    [SerializeField] public float attackTimeStep = 0.025f;
    [HideInInspector] public float currentAttackTime;
    [Space(5)]

    [Header("References")]
    [SerializeField] private PlayerActions playerActions;

    //privates========================================================================================================================================
    //References-------------------------------------
    private Rigidbody2D rb;

    //Movement---------------------------------------
    private Vector2 moveDirection;
    private Vector2 lastAttackPosition;

    //Hit--------------------------------------------
    private bool isHit = false;

    

    public enum states
    {
        Idle,
        Run,
        Attack,
        Dash,
        Parry,
        Hit
    }
    [HideInInspector] public states currentState = states.Idle;



    private void Awake()
    {
        playerActions = new PlayerActions();
        rb = GetComponent<Rigidbody2D>();

        currentAttackTime = maxAttackTime;
    }

    private void OnEnable()
    {
        playerActions.PlayerMap.Enable();
        playerActions.PlayerMap.Attack.performed += ctx => OnAttack();
        playerActions.PlayerMap.Dash.performed += ctx => OnDash();
        playerActions.PlayerMap.Parry.performed += ctx => OnParry();
    }

    private void OnDisable()
    {
        playerActions.PlayerMap.Disable();
        playerActions.PlayerMap.Attack.performed -= ctx => OnAttack();
        playerActions.PlayerMap.Dash.performed -= ctx => OnDash();
        playerActions.PlayerMap.Parry.performed -= ctx => OnParry();
    }

    private void Update()
    {
        StateMachine();
    }

    private void StateMachine()
    {
        switch (currentState)
        {
            case states.Idle:
                moveDirection = playerActions.PlayerMap.Movement.ReadValue<Vector2>();
                if (moveDirection != Vector2.zero)
                {
                    currentState = states.Run;
                }
                break;
            case states.Run:
                moveDirection = playerActions.PlayerMap.Movement.ReadValue<Vector2>();
                if (moveDirection == Vector2.zero)
                {
                    currentState = states.Idle;
                }
                break;
            case states.Attack:

                break;

            case states.Dash:

                break;
            case states.Parry:

                break;
            case states.Hit:

                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" && isHit == false)
        {
            isHit = true;
            currentState = states.Hit;
            Hit(transform.position - other.gameObject.transform.position);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = moveDirection * movementSpeed;
    }

//Attack-----------------------------------------
    private void MidAttackMiss()
    {
        Vector2 direction = new Vector2(Random.Range(-attackRange/1.5f, attackRange/1.5f), Random.Range(-attackRange/1.5f, attackRange/1.5f));
        LeanTween.move(gameObject, lastAttackPosition + direction, currentAttackTime/2).setEaseOutSine().setOnComplete(EndAttack);   
    }

    private void MidAttackHit()
    {
        GameObject target = GetComponent<EnemyRadar>().target;

        //float angle = Random.Range(0f, 2f * Mathf.PI);
        float deltaY = lastAttackPosition.y - target.transform.position.y;
        float deltaX = lastAttackPosition.x - target.transform.position.x;

        // Calculate the angle in radians
        float angleInRadians = Mathf.Atan2(deltaY, deltaX);

        // Convert the angle to degrees (optional)
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

        float rand = Random.Range(-50, 50) * Mathf.Deg2Rad;

        float newX = target.transform.position.x + (attackRange - 1f) * Mathf.Cos(angleInRadians + rand);
        float newY = target.transform.position.y + (attackRange - 1f) * Mathf.Sin(angleInRadians + rand);

        //Vector2 direction = new Vector2(Random.Range(-attackRange/1.5f, attackRange/1.5f), Random.Range(-attackRange/1.5f, attackRange/1.5f));


        LeanTween.move(gameObject, new Vector2(newX, newY), currentAttackTime/2).setEaseOutSine().setOnComplete(EndAttack);


        target.GetComponent<BasicEnemy>().Knockback(new Vector2(target.transform.position.x - lastAttackPosition.x, target.transform.position.y - lastAttackPosition.y));
        
        if (currentAttackTime > minAttackTime)
        {
            currentAttackTime -= attackTimeStep;
        }
    
    }

    private void EndAttack()
    {
        currentState = states.Idle;
    }

//Hit------------------------------------------
    private void Hit(Vector3 _direction)
    {
        Debug.Log("hit!!!");
        float knockbackDistance = 2.4f;
        LeanTween.move(gameObject, transform.position + _direction.normalized * knockbackDistance, 0.1f).setEaseOutSine().setOnComplete(EndHit);
    }

    private void EndHit()
    {
        isHit = false;
        currentState = states.Idle;
    }

//Binds-------------------------------------------
    private void OnAttack()
    {
        if (currentState == states.Idle || currentState == states.Run)
        {
            currentState = states.Attack;
            lastAttackPosition = transform.position;
            Debug.Log("Attack");
            if (GetComponent<EnemyRadar>().target != null)
            {
                GameObject target = GetComponent<EnemyRadar>().target;
                Vector2 direction = target.transform.position - transform.position;
                if (Vector2.Distance(transform.position, target.transform.position) < attackRange)
                {
                    LeanTween.move(gameObject, new Vector2(transform.position.x, transform.position.y) + direction, currentAttackTime/2).setEaseInSine().setOnComplete(MidAttackHit);
                }
                else
                {
                    LeanTween.move(gameObject, new Vector2(transform.position.x, transform.position.y) + direction.normalized * attackRange, currentAttackTime/2).setEaseInSine().setOnComplete(MidAttackMiss);
                }
            }
            else
            {
                Vector2 direction = new Vector2(Random.Range(-attackRange, attackRange), Random.Range(-attackRange, attackRange));
                LeanTween.move(gameObject, new Vector2(transform.position.x, transform.position.y) + direction, currentAttackTime/2).setEaseInSine().setOnComplete(MidAttackMiss);
            }
        }
    }

    private void OnParry()
    {
        if (currentState == states.Idle || currentState == states.Run || currentState == states.Attack)
        {
            Debug.Log("Parry");
        }
    }

    private void OnDash()
    {
        if (currentState == states.Idle || currentState == states.Run)
        {
            currentState = states.Dash;
            Debug.Log("Dash");
            LeanTween.move(gameObject, new Vector2(transform.position.x, transform.position.y) + moveDirection.normalized * dashDistance, 0.1f).setEaseInOutSine().setOnComplete(EndDash);

        }
    }

    private void EndDash()
    {
        currentState = states.Idle;
    }

}
