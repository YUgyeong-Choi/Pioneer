using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RoomsPlayerCollider : MonoBehaviour
{
    [SerializeField] private int idx;
    [SerializeField] private GameObject moveUiPrefab;

    private GameObject moveUi;
    private void OnTriggerEnter(Collider other)
    {
        Log.Debug("들어감");
        if (idx < Managers.Data.roomDataList.Count)
        {
            Managers.Data.roomIndex = idx;
            RoomUiPositionDataClass postionAndRotation = Managers.Data.roomUiDataList[idx];
            moveUi = Instantiate(moveUiPrefab, postionAndRotation.position, Quaternion.Euler(postionAndRotation.rotation));
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        Log.Debug("나감");
        Destroy(moveUi);
    }
    
}
