using System.Collections.Generic;
using System.IO;
using PaintCore;
using PaintIn3D;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomsManager : MonoBehaviour
{
    public Transform roomsCollection;
    DontDestroyOnLoad dontDestroyObject;

    public GameObject smallCube;
    public GameObject middleCube;
    public GameObject largeCube;
    public GameObject smallCylinder;
    public GameObject middleCylinder;
    public GameObject largeCylinder;
    
    private GameObject newRoom = null;
    private int roomIndex;
    
    private string _roomDataPath;
    private List<RoomData> roomDataList = new List<RoomData>();

    public Mesh paintCubeMesh;
    public Mesh paintSmallCylinderMesh;
    public Mesh paintMidleCylinderMesh;
    public Mesh paintLargeCylinderMesh;

    private CwPaintableMeshTexture meshTexture = null;

    void Start()
    {
        _roomDataPath = Path.Combine(Application.persistentDataPath, "RoomData.json");
        dontDestroyObject = FindObjectOfType<DontDestroyOnLoad>();
        roomDataList = ReadRoomDataFromFile();
        roomIndex = dontDestroyObject.roomIndex;
        
        // 예외가 발생하면 인덱스가 올바르지 않음을 의미하므로 새로운 방을 생성하고 추가합니다.
        string roomType = roomDataList[roomIndex].roomType;
        string roomSize = roomDataList[roomIndex].roomSize;
        string roomTypeName = roomSize + roomType;

        switch(roomTypeName)
        { 
            case "smallCube": 
                newRoom = Instantiate(smallCube, new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0));
                newRoom.GetComponent<MeshFilter>().mesh = paintCubeMesh;
                break;
            case "middleCube": 
                newRoom = Instantiate(middleCube, new Vector3(0, 1, 0),Quaternion.Euler(90, 0, 0));
                newRoom.GetComponent<MeshFilter>().mesh = paintCubeMesh;
                break;
            case "largeCube":
                newRoom = Instantiate(largeCube, new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0)); 
                newRoom.GetComponent<MeshFilter>().mesh = paintCubeMesh;
                break;
            case "smallCylinder": 
                newRoom = Instantiate(smallCylinder, new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0));
                newRoom.GetComponent<MeshFilter>().mesh = paintSmallCylinderMesh;
                break;
            case "middleCylinder": 
                newRoom = Instantiate(middleCylinder, new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0)); 
                newRoom.GetComponent<MeshFilter>().mesh = paintMidleCylinderMesh;
                break;
            case "largeCylinder": 
                newRoom = Instantiate(largeCylinder, new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0));
                newRoom.GetComponent<MeshFilter>().mesh = paintLargeCylinderMesh;
                break;
            default: 
                Debug.LogError("Invalid room type"); 
                break;
        }
        
        newRoom.transform.SetParent(roomsCollection);
        CwPaintableMesh mesh = newRoom.AddComponent<CwPaintableMesh>();
        mesh.Activation = CwPaintableMesh.ActivationType.Awake;
        meshTexture = newRoom.AddComponent<CwPaintableMeshTexture>();
        meshTexture.Slot = new CwSlot(0, "_BaseMap");
        meshTexture.Width = 13000;
        meshTexture.Height = 13000;
    }
    
    List<RoomData> ReadRoomDataFromFile()
    {
        List<RoomData> roomDataList = new List<RoomData>();

        if (File.Exists(_roomDataPath))
        {
            string[] lines = File.ReadAllLines(_roomDataPath);

            foreach (string line in lines)
            {
                RoomData fileRoomData = JsonUtility.FromJson<RoomData>(line);
                roomDataList.Add(fileRoomData);
            }
        }
        else
        {
            Debug.LogError("File not found: " + _roomDataPath);
        }

        //Debug.Log("Count:"+roomDataList.Count);
        return roomDataList;
    }

    public void ExitButton()
    {
        meshTexture.GetPngData();
        Application.Quit();
    }
}
