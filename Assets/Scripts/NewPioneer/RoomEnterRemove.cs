using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomEnterRemove : MonoBehaviour
{
    private RoomNameImage roomsUiManager;

    private void Start()
    {
        roomsUiManager = GetComponent<RoomNameImage>();
    }

    public void GoRoomBtn()
    {
        const string sceneName = "RoomScene";
        SceneManager.LoadScene(sceneName);
    }

    public void MultiBtn()
    {
        
    }

    public void DeleteRoomBtn()
    {
        //Managers.Data.DeleteRoomData();
        roomsUiManager.ControlRooms();
    }
}
