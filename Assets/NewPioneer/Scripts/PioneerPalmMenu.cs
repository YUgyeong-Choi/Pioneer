using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PioneerPalmMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _offset;
    
    [SerializeField]
    private AudioSource _showMenuAudio;

    [SerializeField]
    private AudioSource _hideMenuAudio;

    [SerializeField] private Transform InivisiblePoint;

    private Vector3 originPos;
    
    void Start()
    {
        originPos = _offset.transform.localPosition;
        SetInvisible(_offset);
    }

    /// <summary>
    /// Show/hide the menu.
    /// </summary>
    public void ToggleMenu()
    {
        if (_offset.transform.localPosition == originPos)
        {
            _hideMenuAudio.Play();
            SetInvisible(_offset);
        }
        else
        {
            _showMenuAudio.Play();
            SetVisible(_offset);
        }
    }

    void SetInvisible(GameObject go)
    {
        go.transform.localPosition = InivisiblePoint.position;
    }

    void SetVisible(GameObject go)
    {
        go.transform.localPosition = originPos;
    }
}
