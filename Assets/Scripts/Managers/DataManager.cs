using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Data;

/// <summary>
/// 읽어들이는 데이터의 포맷 클래스는 ILoader인터페이스를 구현해야 함.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface ILoader<TKey, TValue>
{
    /// <summary>
    /// 데이터를 Dictionary 형태로 변환하는 메서드. 
    /// Dictionary의 키와 값은 인터페이스의 제네릭 타입으로 지정.
    /// </summary>
    /// <returns></returns>
    Dictionary<TKey, TValue> MakeDict();
}

/// <summary>
/// 데이터를 관리하는 매니저.
/// </summary>
public class DataManager
{
    public List<RoomData> roomDataList;
    public List<RoomUiPositionDataClass> roomUiDataList = new List<RoomUiPositionDataClass>();
    public List<Vector3> roomSelectUiPosition = new List<Vector3>();
    public int roomIndex;
    public bool multiSetting = false;
    private string _path;
    private string _roomMeshPath;
    
    public List<byte[]> roomMeshList = new List<byte[]>();
    
    /// <summary>
    /// 게임 내 정보를 담는 Dictionary. 
    /// 처음 게임이 시작될 때, Init() 함수를 통해 필요한 정보를 여기에 캐싱하고,
    /// 이후에 필요할 때는 여기를 참조해 정보를 활용한다.
    /// </summary>
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();
    
    /// <summary>
    /// 게임이 시작할 때 데이터를 로드하는 초기화 메서드. 
    /// LoadJson 메서드를 사용해 지정된 경로의 Json 파일로부터 데이터를 로드하고,
    /// ILoader 인터페이스를 구현하는 객체의 MakeDict 함수를 호출해 정보를 캐싱한다.
    /// </summary>
    public void Init()
    {
        _path = Path.Combine(Application.persistentDataPath, "RoomData.json");
        _roomMeshPath = Path.Combine(Application.persistentDataPath, "RoomMesh.json");
        roomDataList = ReadRoomDataFromFile();
        SetRoomUIPositionRotation();
        SetRoomSelectUIPosition();
        // 다음과 같은 형태로 사용 가능
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
    }

    /// <summary>
    /// Json 파일에서 데이터를 로드하고, 해당 데이터를 처리하기 위한 ILoader 인터페이스를 구현하는 타입의 인스턴스를 생성한다.
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="TLoader"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    TLoader LoadJson<TLoader, TKey, TValue>(string path) where TLoader : ILoader<TKey, TValue>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<TLoader>(textAsset.text);
    }
    
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
    
    List<byte[]> ReadRoomMeshDataFromFile()
    {
        List<byte[]> roomMeshDataList = new List<byte[]>();

        if (File.Exists(_roomMeshPath))
        {
            string[] lines = File.ReadAllLines(_roomMeshPath);

            foreach (string line in lines)
            {
                string[] byteValues = line.Split('-');
                byte[] imageData = new byte[byteValues.Length];
                for (int i = 0; i < byteValues.Length; i++)
                {
                    imageData[i] = Convert.ToByte(byteValues[i], 16);
                }

                roomMeshDataList.Add(imageData);
            }
        }
        else
        {
            Debug.Log("File not found: " + _roomMeshPath);
        }

        return roomMeshDataList;
    }
    
    public void PlusRoom(RoomData roomData)
    {
        //파일 다음 줄에 추가
        if (File.Exists(_path))
        {
            using (StreamWriter writer = File.AppendText(_path))
            {
                string jsonData = JsonUtility.ToJson(roomData);
                writer.WriteLine(jsonData);
            }
        }
        else // 파일 생성 및 추가
        {
            string data = JsonUtility.ToJson(roomData);
            File.WriteAllText(_path, data + "\n");
        }

        roomIndex = roomDataList.Count;
    }
    
    public void DeleteRoom()
    {
        roomDataList.RemoveAt(roomIndex);

        string text = string.Join("", roomDataList.Select(roomData => JsonUtility.ToJson(roomData) + "\n").ToArray());
        File.WriteAllText(_path, text);
        
        roomDataList = ReadRoomDataFromFile();
    }

    public void saveRoomData(byte[] imageData)
    {
        if (File.Exists(_roomMeshPath))
        {
            // room 내용 변경
            if (roomMeshList.Count > roomIndex)
            {
                string roomMeshs = null;
                roomMeshList[roomIndex] = imageData;
                foreach (byte[] roomMesh in roomMeshList)
                {
                    string byteString = BitConverter.ToString(roomMesh);
                    roomMeshs += byteString + "\n";
                }

                File.WriteAllText(_roomMeshPath, roomMeshs);
            }
            else //새로운 room 추가
            {

                using (StreamWriter writer = File.AppendText(_roomMeshPath))
                {
                    string byteString = BitConverter.ToString(imageData);
                    writer.WriteLine(byteString);
                }
            }

        }
        else
        {
            string byteString = BitConverter.ToString(imageData);
            File.WriteAllText(_roomMeshPath, byteString + "\n");
        }
    }

    private void SetRoomUIPositionRotation()
    {
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(3.829f, 1.4f, 1.75f), new Vector3(0f, 270f, 0f)));
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(3.829f, 1.4f, -1.75f), new Vector3(0f, 270f, 0f)));
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(3.829f, 1.4f, -1.768f), new Vector3(0f, 270f, 0f)));
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(2.266f, 1.4f, -3f), new Vector3(0f, 0f, 0f)));
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(0f, 1.4f, -3f), new Vector3(0f, 0f, 0f)));
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(-2.266f, 1.4f, -3f), new Vector3(0f, 0f, 0f)));
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(-3.89f, 1.4f, -1.745f), new Vector3(0f, 90f, 0f)));
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(-3.89f, 1.4f, 0f), new Vector3(0f, 180f, 0f)));
        roomUiDataList.Add(new RoomUiPositionDataClass(new Vector3(-3.89f, 1.4f, 1.745f), new Vector3(0f, 180f, 0f)));
        
    }

    private void SetRoomSelectUIPosition()
    {
        roomSelectUiPosition.Add(new Vector3(20,-0.7f,0));
        roomSelectUiPosition.Add(new Vector3(40,-0.7f,0));
        roomSelectUiPosition.Add(new Vector3(60,-0.7f,0));
        roomSelectUiPosition.Add(new Vector3(20,-0.7f,20));
        roomSelectUiPosition.Add(new Vector3(40,-0.7f,20));
        roomSelectUiPosition.Add(new Vector3(60,-0.7f,20));
    }
}
