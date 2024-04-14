using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MakeRoom : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] public TextMeshProUGUI roomName; 
    [SerializeField] public TextMeshProUGUI type;
    [SerializeField] public TextMeshProUGUI size;
    private int sizeIndex=1;
    private List<string> sizeText = new List<string>() { "small", "middle", "large" };
    public RoomData roomData = new RoomData();
    
    private List<string> randomNameList = new List<string>() { 
        "Cozy Cabin", 
        "Sunny Villa", 
        "Mystic Manor", 
        "Tranquil Retreat", 
        "Serene Oasis", 
        "Whispering Woods", 
        "Harmony Haven", 
        "Eternal Eden", 
        "Dreamy Dwelling", 
        "Peaceful Paradise"
    };

    public void randomRoomNameBtn()
    {
        int randomIndex = UnityEngine.Random.Range(0, randomNameList.Count);
        roomName.text = randomNameList[randomIndex];
    }

    public void ShowBtn()
    {
        player.transform.position = new Vector3(20, 0, 0);
    }

    public void TypeSelectBtn()
    {
        if (type.text == "Cube")
        {
            type.text = "Cylinder";
        }
        else
        {
            type.text = "Cube";
        }
    }

    public void SizeSelectPrevBtn()
    {
        if (sizeIndex == 0)
        {
            sizeIndex = 2;
        }
        else
        {
            sizeIndex--;
        }

        size.text = sizeText[sizeIndex];
    }
    
    public void SizeSelectNextBtn()
    {
        if (sizeIndex == 2)
        {
            sizeIndex = 0;
        }
        else
        {
            sizeIndex++;
        }
        size.text = sizeText[sizeIndex];
    }

    public void FinishBtn()
    {
        roomData.roomName = roomName.text;
        roomData.roomType = type.text;
        roomData.roomSize = size.text;
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        roomData.currentTime = time;
        Managers.Data.PlusRoom(roomData);
        SceneManager.LoadScene("RoomScene");
    }
}
