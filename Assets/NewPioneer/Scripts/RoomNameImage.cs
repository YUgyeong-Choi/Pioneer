using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomNameImage : MonoBehaviour
{
    [SerializeField] private GameObject rooms;
    private void Start()
    {
        ControlRooms(); // Room 버튼 세팅
    }

    public void ControlRooms()
    {
        for (int i = 0; i < 9; i++)
        {
            string roomName = "Room" + (i + 1);
            Transform roomTransform = rooms.transform.Find(roomName);

            if (roomTransform != null)
            {
                if (i < Managers.Data.roomDataList.Count)
                {
                    Transform roomNameTransform = roomTransform.Find("Canvas/RoomName");
                    TextMeshProUGUI roomNameText = roomNameTransform.GetComponent<TextMeshProUGUI>();
                    roomNameText.text = Managers.Data.roomDataList[i].roomName;
                }
            }
        }
    }
}
