using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StartUIManager : MonoBehaviour
{
    public TMP_InputField InputRoomName;
    public TextMeshProUGUI reconfirmTitle;
    public GameObject keyboardPage;
    public GameObject reconfirmPage;

    private string roomName;

    public void WritingRoomName()
    {
        if (InputRoomName.text.Length != 0)
        {
            roomName = InputRoomName.text;
            keyboardPage.SetActive(false);
            reconfirmPage.SetActive(true);
            reconfirmTitle.text = roomName;
        }  
    }

    public void PreviousWordClear()
    {
        InputRoomName.text = "";
    }

    public void GoToLoading()
    {
        SceneManager.LoadScene(1);
    }
}
