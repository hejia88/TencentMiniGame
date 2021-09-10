using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions : MonoBehaviour
{
    public bool isSelfDestroy = false;
    public float DestroyDelay = 2.0f;
    void Start()
    {
        if (isSelfDestroy)
        {
            Destroy(gameObject, DestroyDelay);
        }
    }
}
