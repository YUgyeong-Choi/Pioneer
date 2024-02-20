using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataClass
{
    public string roomType;
    public string roomSize;
    public Vector3 position;

    public RoomDataClass(string type, string size, Vector3 pos)
    {
        roomType = type;
        roomSize = size;
        position = pos;
    }
}
