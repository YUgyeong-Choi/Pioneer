using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;

public class SyncPosition : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
