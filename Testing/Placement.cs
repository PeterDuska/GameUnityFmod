using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Placement : MonoBehaviour, IPointerDownHandler
{

    private Vector3 normalScale;

    private Color normalColor = new Color(0, 0.5f, 0f, 0.4f);
    private Color seeColor = new Color(0f, 0.8f, 0f, 0.7f);

    void Start()
    {
        normalScale = transform.localScale;
        GetComponent<SpriteRenderer>().color = normalColor;
    }

    public void ActivateHighlight()
    {
        transform.localScale = normalScale * 1.5f;
        GetComponent<SpriteRenderer>().color = seeColor;
    }

    public void DeactivateHighlight()
    {
        transform.localScale = normalScale;
        GetComponent<SpriteRenderer>().color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
