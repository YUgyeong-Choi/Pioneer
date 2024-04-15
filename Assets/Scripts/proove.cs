using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proove : MonoBehaviour
{
    private void Awake()
    {
        proove[] prooves = FindObjectsOfType<proove>();
        
        if(prooves.Length > 1)
            Destroy(gameObject);
    }
}
