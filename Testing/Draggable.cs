using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private Vector3 offsetFromFinger;
    [SerializeField] private float distanceStartDrag = 2f;
    [SerializeField] private float distnaceSee = 3f;
    [Space(5)]

    [Header("References")]
    [SerializeField] private Transform domino;
    [SerializeField] private Camera cam;


    private float _zDistanceToCamera;
    

    private IEnumerator coroutine;


    private Vector2 startPosition;
    private Vector2 touchStartPosition;

    private bool checking = false;
    private bool moving = false;
    private bool test = false;

    private GameObject target = null;
    private GameObject lastTarget = null;
    private GameObject[] targets;
    //private Camera cam;

    void Start()
    {
        //cam = GameObject.Find("Camera").GetComponent<Camera>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (moving)
        {
            transform.position = cam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, _zDistanceToCamera));
            int indexClosest = 0;
            float distanceClosest = 9999f;
            
            int i = 0;
            foreach (GameObject current in targets)
            {
                float currentDistance = Vector2.Distance(transform.position, current.transform.position);
                if (currentDistance < distanceClosest)
                {
                    distanceClosest = currentDistance;
                    indexClosest = i;
                }
                i++;
            }

            if (distanceClosest < distnaceSee)
                target = targets[indexClosest];
            else
                target = null;

            if (lastTarget != target)
            {
                if (lastTarget != null)
                    lastTarget.GetComponent<Placement>().DeactivateHighlight();
                
                if (target != null)
                    target.GetComponent<Placement>().ActivateHighlight();
            }

            lastTarget = target;
        }
        else if (checking == true && test == true)
        {
            if (Vector2.Distance(touchStartPosition, cam.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, _zDistanceToCamera))) > distanceStartDrag)
            {
                moving = true;
                checking = false;
                test = false;
            }            
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer UP");
        if (moving)
        {
            if (target != null)
            {

            }
            else
            {
                Reset();
            }
        }
        else if (checking)
        {
            test = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer DOWN");
        if (checking == false)
        {
            touchStartPosition = transform.position;
            FindTargets();
            coroutine = CreateOffset(0.25f);
            StartCoroutine(coroutine);
            checking = true;
            test = true;

            _zDistanceToCamera = Mathf.Abs (transform.position.z - cam.transform.position.z);

        }
        else
        {
            moving = true;
        }
    }

    public void Reset()
    {
        transform.position = startPosition;
        StopCoroutine(coroutine);
        domino.localPosition = new Vector3(0, 0, 0);
        checking = false;
        moving = false;
        test = false;
    }

    private void FindTargets()
    {
        targets = GameObject.FindGameObjectsWithTag("Placement");
    }

    //Coroutines----------------------------------------------------------------------------------------
    private IEnumerator CreateOffset(float timeToOffset)
    {
        Vector3 startPosition = domino.localPosition;

        float elapsedTime = 0;
        while (elapsedTime < timeToOffset)
        {
            float t = elapsedTime / timeToOffset;
            domino.localPosition = Vector3.Lerp(startPosition, offsetFromFinger, t);
        
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        domino.localPosition = offsetFromFinger;
    }
}
