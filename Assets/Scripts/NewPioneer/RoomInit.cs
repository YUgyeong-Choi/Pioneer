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
    private CwPaintableMeshTexture meshTexture = null;

    void Start()
    {
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
        Debug.Log("roomCount: " + Managers.Data.roomMeshList.Count);
        if (Managers.Data.roomMeshList.Count > Managers.Data.roomIndex)
        {
            CwPaintableMeshTexture mesh = newRoom.GetComponent<CwPaintableMeshTexture>();
            mesh.LoadFromData(Managers.Data.roomMeshList[Managers.Data.roomIndex]);
        }
        else
        {
            //아직 방에 대한 데이터 없음
            return;
        }
    }

    public void ExitButton()
    {
        CwPaintableMeshTexture mesh = newRoom.GetComponent<CwPaintableMeshTexture>();
        byte[] imageData = mesh.GetPngData();

        Managers.Data.saveRoomData(imageData);
    }
}
