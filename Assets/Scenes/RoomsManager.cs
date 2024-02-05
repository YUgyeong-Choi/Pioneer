using UnityEngine;

public class RoomsManager : MonoBehaviour
{
    // room 활성화
    void Start()
    {
        string roomName;

        DontDestroyOnLoad dontDestroyObject = FindObjectOfType<DontDestroyOnLoad>();
        
        GameObject.Find("RoomsParent").transform.GetChild(dontDestroyObject.roomIndex).gameObject.SetActive(true);
    }
}
