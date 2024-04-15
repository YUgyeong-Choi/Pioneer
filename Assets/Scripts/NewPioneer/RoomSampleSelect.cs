using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomSampleSelect : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject self;
    private int selectIndex = 0;
    private List<string> texts = new List<string>() { "Cube\nSmall","Cube\nMiddle","Cube\nLarge","Cylinder\nSmall","Cylinder\nMiddle","Cylinder\nLarge" };

    public void PrevSelectBtn()
    {
        if (selectIndex == 0)
        {
            selectIndex = 5;
        }
        else
        {
            selectIndex--;
        }

        text.text = texts[selectIndex];
        player.transform.position = Managers.Data.roomSelectUiPosition[selectIndex];
        self.transform.position = Managers.Data.roomSelectUiPosition[selectIndex] + new Vector3(0,0.7f,1f);
    }

    public void NextSelectBtn()
    {
        if (selectIndex == 5)
        {
            selectIndex = 0;
        }
        else
        {
            selectIndex++;
        }
        text.text = texts[selectIndex];
        player.transform.position = Managers.Data.roomSelectUiPosition[selectIndex];
        self.transform.position = Managers.Data.roomSelectUiPosition[selectIndex] + new Vector3(0,0.7f,1f);
    }

    public void BackBtn()
    {
        player.transform.position = new Vector3(0, 0.4f, 4.8f);

        selectIndex = 0;
        text.text = texts[selectIndex];
        self.transform.position = Managers.Data.roomSelectUiPosition[selectIndex] + new Vector3(0,0.7f,1f);
    }

}
