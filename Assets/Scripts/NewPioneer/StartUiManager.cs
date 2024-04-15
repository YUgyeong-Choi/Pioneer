using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    public TMP_InputField inputRoomName;
    public TextMeshProUGUI reconfirmTitle;
    public GameObject keyboardPage;
    public GameObject reconfirmPage;
    public GameObject roomPanel;
    public TextMeshProUGUI addRoomText;
    public TextMeshProUGUI changeRoomNameText;
    public GameObject player;
    public GameObject roomSelectPage;
    public TextMeshProUGUI roomSizeText;
    public TextMeshProUGUI roomTypeText;

    private string _path;
    private List<RoomData> _roomDataList;
    private RoomDataClass[] _cubeInfo;
    private RoomDataClass[] _cylinderInfo;
    private int _roomTypeIndex = 1;

    // 버튼의 text에 roomName 할당
    void Start()
    {
        //Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
        _path = Path.Combine(Application.persistentDataPath, "RoomData.json");
        _roomDataList = ReadRoomDataFromFile();

        for (int i = 0; i < _roomDataList.Count; i++)
        {
            string roomName = "Room" + (i + 1);
            Transform roomButtonTransform = roomPanel.transform.Find(roomName);

            if (roomButtonTransform != null)
            {
                TextMeshProUGUI buttonText = roomButtonTransform.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = _roomDataList[i].roomName;
                }
                else
                {
                    Debug.LogError("Text component not found on " + roomName);
                }
            }
            else
            {
                Debug.LogError(roomName + " not found in RoomPanel");
            }
        }
        
        _cubeInfo = new RoomDataClass[]
        {
            new RoomDataClass("Cube", "small", new Vector3(20f, 1f, 0f)),
            new RoomDataClass("Cube", "middle", new Vector3(40f, 1f, 0f)),
            new RoomDataClass("Cube", "large", new Vector3(60f, 1f, 0f))
        };

        _cylinderInfo = new RoomDataClass[]
        {
            new RoomDataClass("Cylinder", "small", new Vector3(20f, 1f, 20f)),
            new RoomDataClass("Cylinder", "middle", new Vector3(40f, 1f, 20f)),
            new RoomDataClass("Cylinder", "large", new Vector3(60f, 1f, 20f))
        };
    }

    // json 불러와서 List 형태로
    List<RoomData> ReadRoomDataFromFile()
    {
        List<RoomData> roomDataList = new List<RoomData>();

        if (File.Exists(_path))
        {
            string[] lines = File.ReadAllLines(_path);

            foreach (string line in lines)
            {
                RoomData roomData = JsonUtility.FromJson<RoomData>(line);
                roomDataList.Add(roomData);
            }
        }
        else
        {
            Debug.LogError("File not found: " + _path);
        }

        return roomDataList;
    }
    
    // New Room 버튼 클릭 시 keyboard 초기화
    public void PreviousWordClear()
    {
        inputRoomName.text = "";
    }
    

    // 키보드로 방 이름 설정 후 finish 버튼
    public void WritingRoomName()
    {
        foreach (RoomData room in _roomDataList)
        {
            // 저장된 방 이름이면 Error 후 초기화
            if (room.roomName == inputRoomName.text)
            {
                StartCoroutine(SameRoomError());
                inputRoomName.text = "";
                return;
            }
        }

        // 이름이 적혀있으면 Page 이동
        if (inputRoomName.text.Length != 0)
        {
            keyboardPage.SetActive(false);
            reconfirmPage.SetActive(true);
            reconfirmTitle.text = inputRoomName.text;
        }  
    }
    
    IEnumerator SameRoomError()
    {
        changeRoomNameText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        changeRoomNameText.gameObject.SetActive(false);
    }
    
    // 방 선택
    public void SelectRoom(Button clickedButton)
    {
        string currentTime= DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
        int roomIndex = 0;
        TextMeshProUGUI buttonText = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
        
        foreach (RoomData room in _roomDataList)
        {
            if (room.roomName == buttonText.text)
            {
                currentTime = room.currentTime;
                break;
            }
            roomIndex++;
        }
        
        // currentTime이 초기화 상태면 방이 없어 Error
        if (currentTime == DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss"))
        {
            StartCoroutine(NoRoomError());
            return; 
        }

        // roomName과 currentTime을 다음 씸으로 넘겨주는 부분
        GameObject transfer = GameObject.Find("Information Transfer");

        if (transfer != null)
        {
            DontDestroyOnLoad dontDestroyScript = transfer.GetComponent<DontDestroyOnLoad>();

            if (dontDestroyScript != null)
            {
                dontDestroyScript.roomName = buttonText.text;
                dontDestroyScript.currentTime = currentTime;
                dontDestroyScript.roomIndex = roomIndex;
            }
            else
            {
                Debug.LogError("DontDestroyOnLoad ��ũ��Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("'Player' �±װ� ������ GameObject�� ã�� �� �����ϴ�.");
        }

        SceneManager.LoadScene(1);
    }

    IEnumerator NoRoomError()
    {
        addRoomText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        addRoomText.gameObject.SetActive(false);
    }

    // 방 모양 선택
    public void RoomTypeButton(Button clickedButton)
    {
        _roomTypeIndex = 1;
        TextMeshProUGUI buttonText = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
        
        if (buttonText.text == "Cube")
        {
            roomSelectPage.SetActive(true);
            roomTypeText.text = "Cube";
            roomSizeText.text = _cubeInfo[_roomTypeIndex].roomSize;
            player.transform.position = _cubeInfo[_roomTypeIndex].position;
        }else if(buttonText.text == "Cylinder")
        {
            roomSelectPage.SetActive(true);
            roomTypeText.text = "Cylinder";
            roomSizeText.text = _cylinderInfo[_roomTypeIndex].roomSize;
            player.transform.position = _cylinderInfo[_roomTypeIndex].position;
        }
    }

    // 방 사이즈 이동
    public void MoveRoomType(Button clickedButton)
    {
        RoomDataClass[] roomInfoArray = null;

        if (roomTypeText.text == "Cube")
        {
            roomInfoArray = _cubeInfo;
        }
        else if (roomTypeText.text == "Cylinder")
        {
            roomInfoArray = _cylinderInfo;
        }

        if (roomInfoArray != null)
        {
            if (clickedButton.gameObject.name == "Icon Prev")
            {
                _roomTypeIndex--;
                if (_roomTypeIndex < 0)
                {
                    _roomTypeIndex = roomInfoArray.Length - 1;
                }
            }
            else if (clickedButton.gameObject.name == "Icon Next")
            {
                _roomTypeIndex++;
                if (_roomTypeIndex >= roomInfoArray.Length)
                {
                    _roomTypeIndex = 0;
                }
            }

            roomSizeText.text = roomInfoArray[_roomTypeIndex].roomSize;
            player.transform.position = roomInfoArray[_roomTypeIndex].position;
        }
    }

    public void BackSelectPage()
    {
        roomSelectPage.SetActive(false);
        player.transform.position = new Vector3(0, 1, -1.5f);
    }
}

