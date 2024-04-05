using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PaintIn3D;
using UnityEngine;

public class RoomInit : MonoBehaviour
{
    public GameObject smallCube;
    public GameObject middleCube;
    public GameObject largeCube;
    public GameObject smallCylinder;
    public GameObject middleCylinder;
    public GameObject largeCylinder;
    
    private GameObject newRoom = null;
    
    private List<byte[]> roomMeshList = new List<byte[]>();

    private CwPaintableMeshTexture meshTexture = null;

    void Start()
    {
        //roomMeshList = ReadRoomMeshDataFromFile();
        
        // 초기 방 생성
        string roomType = Managers.Data.roomDataList[Managers.Data.roomIndex].roomType;
        string roomSize = Managers.Data.roomDataList[Managers.Data.roomIndex].roomSize;
        string roomTypeName = roomSize + roomType;

        switch(roomTypeName)
        { 
            case "smallCube": 
                newRoom = Instantiate(smallCube, new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
                break;
            case "middleCube": 
                newRoom = Instantiate(middleCube, new Vector3(0, 0, 0),Quaternion.Euler(90, 0, 0));
                break;
            case "largeCube":
                newRoom = Instantiate(largeCube, new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)); 
                break;
            case "smallCylinder": 
                newRoom = Instantiate(smallCylinder, new Vector3(0, 2, 0), Quaternion.Euler(90, 0, 0));
                break;
            case "middleCylinder": 
                newRoom = Instantiate(middleCylinder, new Vector3(0, 02, 0), Quaternion.Euler(90, 0, 0)); 
                break;
            case "largeCylinder": 
                newRoom = Instantiate(largeCylinder, new Vector3(0, 02, 0), Quaternion.Euler(90, 0, 0));
                break;
            default: 
                Debug.LogError("Invalid room type"); 
                break;
        }
        
        // 방 데이터 있으면 불러오기
        Debug.Log("roomCount: " + roomMeshList.Count);
        if (roomMeshList.Count > Managers.Data.roomIndex)
        {
            CwPaintableMeshTexture mesh = newRoom.GetComponent<CwPaintableMeshTexture>();
            mesh.LoadFromData(roomMeshList[Managers.Data.roomIndex]);
        }
        else
        {
            //아직 방에 대한 데이터 없음
            return;
        }
    }

    /**
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


    public void ExitButton()
    {
        CwPaintableMeshTexture mesh = newRoom.GetComponent<CwPaintableMeshTexture>();
        byte[] imageData = mesh.GetPngData();

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
        Application.Quit();
    }
    **/
}
