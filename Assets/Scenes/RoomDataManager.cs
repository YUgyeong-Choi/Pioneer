using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class RoomData
{
    public string roomName;
    public string roomType;
    public string roomSize;
    public string currentTime;
}

public class RoomDataManager : MonoBehaviour
{
    public TextMeshProUGUI reconfirmTitle;
    public TextMeshProUGUI roomType;
    public TextMeshProUGUI roomSize;

    public RoomData roomData = new RoomData();
    
    //Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    private string _path;
    private int _roomIndex=99;

    void Start()
    {
        _path = Path.Combine(Application.persistentDataPath, "RoomData.json");
        _roomIndex = ReadRoomDataFromFile();
    }
    
    int ReadRoomDataFromFile()
    {
        List<RoomData> roomDataList = new List<RoomData>();

        if (File.Exists(_path))
        {
            string[] lines = File.ReadAllLines(_path);

            foreach (string line in lines)
            {
                RoomData fileRoomData = JsonUtility.FromJson<RoomData>(line);
                roomDataList.Add(fileRoomData);
            }
        }
        else
        {
            Debug.LogError("File not found: " + _path);
        }

        return roomDataList.Count;
    }


    public void PlusRoom()
    {
        //파일 다음 줄에 추가
        if (File.Exists(_path))
        {
            roomData.roomName = reconfirmTitle.text;
            roomData.roomType = roomType.text;
            roomData.roomSize = roomSize.text;
            roomData.currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            using (StreamWriter writer = File.AppendText(_path))
            {
                string jsonData = JsonUtility.ToJson(roomData);
                writer.WriteLine(jsonData);
            }
        }
        else // 파일 생성 및 추가
        {
            roomData.roomName = reconfirmTitle.text;
            roomData.roomType = roomType.text;
            roomData.roomSize = roomSize.text;
            roomData.currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string data = JsonUtility.ToJson(roomData);
            File.WriteAllText(_path, data + "\n");
            
        }
        
        // RoomsScene에 정보 전달
        GameObject transfer = GameObject.Find("Information Transfer");

        if (transfer != null)
        {
            DontDestroyOnLoad dontDestroyScript = transfer.GetComponent<DontDestroyOnLoad>();

            if (dontDestroyScript != null)
            {
                dontDestroyScript.roomName = roomData.roomName;
                dontDestroyScript.currentTime = roomData.currentTime;
                dontDestroyScript.roomIndex = _roomIndex;
            }
            else
            {
                Debug.LogError("DontDestroyOnLoad 스크립트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("'Player' 태그가 지정된 GameObject를 찾을 수 없습니다.");
        }
        
        SceneManager.LoadScene(1);

    }
}
