using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRadar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float distanceToNotice = 5f;
    [Space(5)]

    [Header("References")]
    [SerializeField] private PlayerActions playerActions;



    [HideInInspector] public GameObject target = null;
    private GameObject lastTarget = null;

    //privates----------------------------------------------------------
    private List<GameObject> currentPool = new List<GameObject>();
    private int targetIndex = 0;
    private float checkInterval = 0.25f;
    private float lastCheck = 0f;




    private void Awake()
    {
        playerActions = new PlayerActions();
    }


    private void OnEnable()
    {
        playerActions.PlayerMap.Enable();
        playerActions.PlayerMap.NextTarget.performed += ctx => OnNextTarget();
    }


    private void OnDisable()
    {
        playerActions.PlayerMap.Disable();
        playerActions.PlayerMap.NextTarget.performed -= ctx => OnNextTarget();
    }


    private void Update()
    {
        if (Time.time > lastCheck + checkInterval)
        {
            lastCheck = Time.time;
            currentPool = PerformScan();
            currentPool = SortByClosest(currentPool);

            if (currentPool.Count == 0)
            {
                target = null;
                if (lastTarget != null) { lastTarget.GetComponent<EnemyComponent>().DeactivateHighight(); }
                GetComponent<CharacterMovement>().currentAttackTime = GetComponent<CharacterMovement>().maxAttackTime;
            }
            else
            {
                /*
                target = currentPool[0];
                if 
                if (lastTarget != target)
                {
                    if (lastTarget != null) { lastTarget.GetComponent<EnemyComponent>().DeactivateHighight(); }
                    target.GetComponent<EnemyComponent>().ActivateHighlight();
                }
                */
                List<GameObject> newPool = new List<GameObject>();
                if (currentPool.Count > 3)
                    newPool = new List<GameObject> { currentPool[0], currentPool[1], currentPool[2] };
                else
                    newPool = currentPool;

                currentPool = newPool;
                if (!currentPool.Contains(target))
                {
                    if (target != null) { target.GetComponent<EnemyComponent>().DeactivateHighight(); }
                    GetComponent<CharacterMovement>().currentAttackTime = GetComponent<CharacterMovement>().maxAttackTime;
                    targetIndex = 0;
                    target = currentPool[0];
                    target.GetComponent<EnemyComponent>().ActivateHighlight();
                }
                else
                {
                    targetIndex = currentPool.IndexOf(target);
                }
            }

            lastTarget = target;
        }
    }

/*
target out of range
switch target
*/

    private List<GameObject> PerformScan()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> closeEnemies = new List<GameObject>();

        foreach (GameObject enemy in allEnemies)
        {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= distanceToNotice)
            {
                closeEnemies.Add(enemy);
            }
        }

        return closeEnemies;
    }

    private List<GameObject> SortByClosest(List<GameObject> _list)
    {
        List<GameObject> result = new List<GameObject>();

        while (_list.Count > 0)
        {
            int closestIndex = 0;
            float closestDistance = distanceToNotice;
            int i = 0;
            foreach (GameObject current in _list)
            {
                float currentDistance = Vector2.Distance(transform.position, current.transform.position);
                if (currentDistance <= closestDistance)
                {
                    closestDistance = currentDistance;
                    closestIndex = i;
                }
                i++;
            }

            result.Add(_list[closestIndex]);
            _list.RemoveAt(closestIndex);

        }

        return result;
    }

    private void OnNextTarget()
    {
        Debug.Log("NextTarget");
        if (currentPool.Count > 1)
        {
            if (targetIndex + 1 < currentPool.Count)
            {
                targetIndex += 1;
            }
            else
            {
                targetIndex = 0;
            }
            target.GetComponent<EnemyComponent>().DeactivateHighight();
            GetComponent<CharacterMovement>().currentAttackTime = GetComponent<CharacterMovement>().maxAttackTime;
            target = currentPool[targetIndex];
            target.GetComponent<EnemyComponent>().ActivateHighlight();
        }
    }
}
