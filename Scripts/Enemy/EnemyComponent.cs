using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float knockbackDistance = 0.5f;
    [SerializeField] private int maxHealth = 100;


    public bool isHighlighted = false;

    public void ActivateHighlight()
    {
        isHighlighted = true;
        GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);
    }

    public void DeactivateHighight()
    {
        isHighlighted = false;
        GetComponent<SpriteRenderer>().color = new Color(0.2f, 0f, 0f);
    }


}
