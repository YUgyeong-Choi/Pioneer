using System;
using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;

[RequireComponent(typeof(Grabbable))]
[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    public bool inSlot;
    public Vector3 slotRotation = Vector3.zero;
    public Slot currentSlot;

    private Grabbable grabbable;

    private void Awake()
    {
        grabbable = GetComponent<Grabbable>();

        grabbable.onGrab.AddListener(OnGrabBegin);
    }

    public void OnGrabBegin(Hand hand, Grabbable grabbable)
    {
        if (inSlot)
        {
            //gameObject.GetComponentInParent<Slot>().itemInSlot = null;
            //gameObject.transform.parent = null;
            inSlot = false;
            currentSlot.itemInSlot = null;
            currentSlot.ResetColor();
            currentSlot = null;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

}
