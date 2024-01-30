using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RandomTips : MonoBehaviour
{
    private string[] tips = { "tips1", "tips2", "tips3", "tips4", "tips5", "tips6", "tips7", "tips8", "tips9", "tips10" };
    public TextMeshProUGUI Tip;

    void Start()
    {
        SetRandomTip();
        Invoke("LoadNextScene", 10f); //여기에 서버 연동이 완료되면
    }

    void SetRandomTip()
    {
        int randomIndex = Random.Range(0, tips.Length);
        Tip.text = tips[randomIndex];
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(2);
    }
}