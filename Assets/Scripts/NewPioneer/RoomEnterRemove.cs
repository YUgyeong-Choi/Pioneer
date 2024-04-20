using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomEnterRemove : MonoBehaviour
{
    [SerializeField] private GameObject multiVisual;
    [SerializeField] private TextMeshProUGUI multiText;
    private InteractableColorVisual visual;
    private GameObject rooms;

    private void Start()
    {
        rooms = GameObject.Find("Rooms");
    }

    public void GoRoomBtn()
    {
        const string sceneName = "RoomScene";
        SceneManager.LoadScene(sceneName);
    }

    public void MultiBtn()
    {
        if (Managers.Data.multiSetting == false)
        {
            Managers.Data.multiSetting = true;
            visual.InjectOptionalNormalColorState(new InteractableColorVisual.ColorState() { Color = Color.green });
            visual.InjectOptionalHoverColorState(new InteractableColorVisual.ColorState() { Color = Color.green });
            visual.InjectOptionalSelectColorState(new InteractableColorVisual.ColorState() { Color = Color.green });
            multiText.text = "ON MULTI";
        }
        else
        {
            Managers.Data.multiSetting = false;
            visual.InjectOptionalNormalColorState(new InteractableColorVisual.ColorState() { Color = Color.red });
            visual.InjectOptionalHoverColorState(new InteractableColorVisual.ColorState() { Color = Color.red });
            visual.InjectOptionalSelectColorState(new InteractableColorVisual.ColorState() { Color = Color.red });
            multiText.text = "OFF MULTI";
        }
    }

    public void DeleteRoomBtn()
    {
        Managers.Data.DeleteRoom();
        Destroy(gameObject);
        RoomNameImage roomsUiManager = rooms.GetComponent<RoomNameImage>();
        roomsUiManager.ControlRooms();
    }
}
