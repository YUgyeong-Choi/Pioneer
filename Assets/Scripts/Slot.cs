using System;
using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject itemInSlot;
    public Image slotImage;
    private Color originalColor;

    private void Start()
    {
        slotImage = GetComponentInChildren<Image>();
        originalColor = slotImage.color;
    }

    private void OnTriggerStay(Collider other)
    {
        if (itemInSlot != null)
            return;

        Item item = FindItemInGameObjectOrParent(other.gameObject);

        if (item == null) return;
        
        if (!item.GetComponent<Grabbable>().IsHeld())
        {
            InsertItem(item.gameObject);
        }
    }

    void InsertItem(GameObject go)
    {
        go.GetComponent<Rigidbody>().isKinematic = true;
        go.transform.SetParent(gameObject.transform, true);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = go.GetComponent<Item>().slotRotation;
        go.GetComponent<Item>().inSlot = true;
        go.GetComponent<Item>().currentSlot = this;
        itemInSlot = go;
        slotImage.color = Color.gray;
    }

    public void ResetColor()
    {
        slotImage.color = originalColor;
    }
    
    private Item FindItemInGameObjectOrParent(GameObject obj)
    {
        // 현재 게임 오브젝트에서 Item 컴포넌트를 시도하여 찾습니다.
        Item item = obj.GetComponent<Item>();
        
        // Item 컴포넌트가 발견되면 반환합니다.
        if (item != null)
        {
            return item;
        }
        
        // 부모 게임 오브젝트가 있을 경우, 그 부모에서 재귀적으로 Item 컴포넌트를 찾습니다.
        if (obj.transform.parent != null)
        {
            return FindItemInGameObjectOrParent(obj.transform.parent.gameObject);
        }
        
        // Item 컴포넌트를 찾지 못하면 null을 반환합니다.
        return null;
    }
}
