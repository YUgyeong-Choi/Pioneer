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

    private string _path;
    private List<RoomData> _roomDataList;

    // 버튼의 text에 roomName 할당
    void Start()
    {
        //Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
        _path = Path.Combine(Application.persistentDataPath, "RoomData.json");
        _roomDataList = ReadRoomDataFromFile();

        //Debug.Log(roomDataList.Count);

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
}
