using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUiPositionDataClass 
{
    public Vector3 position;
    public Vector3 rotation;

    public RoomUiPositionDataClass(Vector3 pos, Vector3 rat)
    {
        position = pos;
        rotation = rat;
    }
}
