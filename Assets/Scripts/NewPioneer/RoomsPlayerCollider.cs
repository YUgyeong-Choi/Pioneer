using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RoomsPlayerCollider : MonoBehaviour
{
    [SerializeField] private int idx;
    [SerializeField] private GameObject moveUi;
    
    // 타이머에 사용될 변수
    private Coroutine timerCoroutine;
    private float timerDuration = 15f;
    private bool isTimerRunning = false;
    
    private void OnTriggerEnter(Collider other)
    {
        Log.Debug("들어감");
        if (idx < Managers.Data.roomDataList.Count && !isTimerRunning)
        {
            Managers.Data.roomIndex = idx;
            RoomUiPositionDataClass postionAndRotation = Managers.Data.roomUiDataList[idx];

            moveUi.SetActive(true);
            moveUi.transform.position = postionAndRotation.position;
            moveUi.transform.rotation = Quaternion.Euler(postionAndRotation.rotation);
            
            timerCoroutine = StartCoroutine(StartTimer());
            isTimerRunning = true;
        }
        
    }
    
    // 타이머를 시작하는 코루틴
    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timerDuration);

        // 일정 시간이 지나면 UI를 비활성화
        if (moveUi != null)
        {
            moveUi.SetActive(false);
            isTimerRunning = false;
        }
    }
    
}
