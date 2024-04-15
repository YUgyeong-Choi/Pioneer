using UnityEngine;
using System;

public class DontDestroyOnLoad : MonoBehaviour
{
    public string roomName;
    public string currentTime;
    public int roomIndex;
    
    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }

}
