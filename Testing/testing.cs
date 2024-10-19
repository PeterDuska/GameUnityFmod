using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class testing : MonoBehaviour, IPointerDownHandler
{
    public string name;


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(name);
    }
}
