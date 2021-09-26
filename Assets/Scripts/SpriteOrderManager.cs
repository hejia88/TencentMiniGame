using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderManager : MonoBehaviour
{
    public bool FixedObject;
    private SpriteRenderer spriteRenderer;
    public bool isCharacter;
    [SerializeField] GameObject thisCharacter;
    public int layerOffset;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -(int)(transform.position.z * 100);
    }

    void Update()
    {
        if (FixedObject)
        {
            if (isCharacter)
            {
                spriteRenderer.sortingOrder = -(int)(thisCharacter.transform.position.z * 100) + layerOffset;
            }
        }
    }


}
