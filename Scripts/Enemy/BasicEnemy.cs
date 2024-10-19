using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float scanDistance = 10f;
    [Space(5)]
    [Header("Orbiting")]
    [SerializeField] private float minOrbitingSpeed = 2f;
    [SerializeField] private float maxOrbitingSpeed = 6f;
    [Space(2)]
    [SerializeField] private float minOrbitingDistance = 4f;
    [SerializeField] private float maxOrbitingDistance = 8f;
    [Space(5)]

    [Header("References")]
    [SerializeField] private Collider2D hurtBox;

    //privates========================================================================================================================================
    //Orbiting--------------------------------------------------------
    private float orbitInterval = 0f;
    private float orbitStamp = 0f;
    private float orbitingSpeed = 0f;
    private float orbitingDistance = 0f;

    //Movement--------------------------------------------------------
    private Vector2 moveDirection = Vector2.zero;

    //Scanning--------------------------------------------------------
    private float scanInterval = 0.3f;
    private float timeStamp = 0f;
    private GameObject player;

    //Attack--------------------------------------------------------
    private Vector2 attackDirection = Vector2.zero;
    private Vector3 showAttackUITarget = Vector3.zero;
    private bool inAttack = false;
    private LeanTween attackTween;


    public enum states
    {
        Idle,
        Wander,
        Approaching,
        Orbit,
        Attack,
        PreAttack,
        Defend,
        Hit,
        Die,
        Stun
    }
    //[HideInInspector] 
    public states currentState = states.Idle;
    [HideInInspector] public bool isAlerted = false;

    private Rigidbody2D rb;

//Body--------------------------------------------------------
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        orbitingSpeed = Random.Range(minOrbitingSpeed, maxOrbitingSpeed);
        orbitingDistance = Random.Range(minOrbitingDistance, maxOrbitingDistance);

        GetComponent<LineRenderer>().enabled = false;
        hurtBox.enabled = false;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (currentState == states.Orbit)
        {
            rb.velocity = moveDirection.normalized * orbitingSpeed;
        }
        else
        {
            rb.velocity = moveDirection.normalized * movementSpeed;
        }
    }

    private void StateMachine()
    {
        //ScanForPlayer();
        //moveDirection = Vector2.zero;
        switch (currentState)
        {
            case states.Idle:
                //ScanForPlayer();
                if (isAlerted)
                    IdleAlerted();
                else
                    Idle();
                break;
            case states.Wander:

                break;
            case states.Approaching:
                //ScanForPlayer();
                Approaching();
                break;
            case states.Orbit:
                Orbiting();
                break;
            case states.Attack:
                Vector3 directionVector = attackDirection;
                Vector3 target = Vector3.zero;
                if (inAttack == false)
                {
                    target = transform.position + directionVector.normalized * (Vector2.Distance(transform.position, player.transform.position) + 1.5f);
                }
                else
                {
                    target = showAttackUITarget;
                }
                Vector3[] positions = new Vector3[] {transform.position, target};
                GetComponent<LineRenderer>().SetPositions(positions);
                break;
            case states.Defend:

                break;
            case states.Hit:

                break;
            case states.Die:

                break;
            case states.Stun:

                break;
        }
    }

    private void Idle()
    {
        moveDirection = Vector2.zero;

         if (Time.time > timeStamp)
        {
            timeStamp = Time.time + scanInterval;
            if (Vector2.Distance(transform.position, player.transform.position) < scanDistance)
            {
                if (isAlerted == false)
                    currentState = states.Approaching;
                isAlerted = true;
            }
        }
    }

    private void IdleAlerted()
    {
        moveDirection = Vector2.zero;
    }

    private void Approaching()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > orbitingDistance)
        {
            Vector2 direction = player.transform.position - transform.position;
            moveDirection = direction;
        }
        else
        {
            currentState = states.Orbit;
        }

        if (Time.time > timeStamp)
        {
            timeStamp = Time.time + scanInterval;
            if (Vector2.Distance(transform.position, player.transform.position) > scanDistance * 2)
            {
                if (isAlerted == true)
                    currentState = states.Idle;
                isAlerted = false;
                //currentState = states.Idle;
            }
        }
    }
    
    private void Orbiting()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > orbitingDistance + 2f)
        {
            currentState = states.Approaching;
            return;
        }

        if (Time.time > orbitStamp)
        {
            float orbitInterval = Random.Range(0.3f, 1.2f);
            orbitStamp = Time.time + orbitInterval;

            int nextMove = Random.Range(0, 100);
            int direction;
            if (nextMove > 72)
            {
                //right
                direction = 1;
            }
            else if (nextMove > 44)
            {
                //left
                direction = -1;
            }
            else if (nextMove > 26)
            {
                //stand
                direction = 0;
            }
            else
            {
                orbitStamp = 0f;
                currentState = states.Attack;
                PreAttack();
                moveDirection = Vector2.zero;
                return;
            }

            Vector2 orbitCenter = player.transform.position;

            Vector2 directionVector = orbitCenter - new Vector2(transform.position.x, transform.position.y);
            Vector2 normalVector = new Vector2(- directionVector.y, directionVector.x);

            moveDirection = normalVector * direction;
            
        }
    }

//Stun--------------------------------------------------------
    private void PreStun()
    {
        currentState = states.Stun;
    }

    private void EndStun()
    {
        currentState = states.Approaching;
    }

//Attack--------------------------------------------------------
    private void PreAttack()
    {
        GetComponent<LineRenderer>().enabled = true;
        Vector3 directionVector = transform.position - player.transform.position;
        //showAttackUITarget = directionVector.normalized * (Vector2.Distance(transform.position, player.transform.position) + 1.5f);

        float magnitude = Vector2.Distance(transform.position, player.transform.position);
        showAttackUITarget = transform.position + directionVector.normalized  * (magnitude + 1.5f) * -1;


        attackDirection = directionVector.normalized * -1;

        LeanTween.move(gameObject, transform.position + directionVector.normalized * 2, 0.6f).setEaseInSine().setOnComplete(Attack);
    }
    
    private void Attack()
    {
        inAttack = true;
        hurtBox.enabled = true;
        float magnitude = Vector2.Distance(transform.position, player.transform.position);
        Vector3 directionVector = player.transform.position - transform.position;


        LeanTween.move(gameObject, transform.position + new Vector3(attackDirection.x, attackDirection.y, 0) * (magnitude + 1.5f), 0.2f).setEaseOutSine().setOnComplete(EndAttack);
    }

    private void EndAttack()
    {
        inAttack = false;
        GetComponent<LineRenderer>().enabled = false;
        hurtBox.enabled = false;
        currentState = states.Approaching;
    }

//Hit--------------------------------------------------------
    public void Knockback(Vector2 direction)
    {
        GetComponent<LineRenderer>().enabled = false;
        moveDirection = Vector2.zero;
        currentState = states.Hit;
        LeanTween.move(gameObject, new Vector2(transform.position.x, transform.position.y) + direction.normalized * 0.5f, 0.15f).setEaseOutSine().setOnComplete(EndKnockback);
    }

    private void EndKnockback()
    {   
        currentState = states.Approaching;
    }

    private void ScanForPlayer()
    {
        if (isAlerted)
        {
            if (Time.time > timeStamp)
            {
                timeStamp = Time.time + scanInterval;
                if (Vector2.Distance(transform.position, player.transform.position) > scanDistance * 2)
                {
                    if (isAlerted == true)
                        currentState = states.Idle;
                    isAlerted = false;
                    //currentState = states.Idle;
                }
            }
        }
        else
        {
            if (Time.time > timeStamp)
            {
                timeStamp = Time.time + scanInterval;
                if (Vector2.Distance(transform.position, player.transform.position) < scanDistance)
                {
                    if (isAlerted == false)
                        currentState = states.Approaching;
                    isAlerted = true;
                }
            }
        }
    }
}
