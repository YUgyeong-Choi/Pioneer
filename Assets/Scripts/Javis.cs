using System;
using System.Collections;
using System.Collections.Generic;
using Chiligames.MetaFusionTemplate;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Events;

public class Javis : MonoBehaviour
{
    [SerializeField] AvatarSpawner avatarSpawner;
    
    [SerializeField] private GameObject OVRLeftHandSynthetic;
    [SerializeField] private GameObject OVRRightHandSynthetic;

    private static GameObject localAvatar = null;

    private void Start()
    {
        avatarSpawner.OnLocalAvatarLoaded += GetLocalAvatar;
    }

    void GetLocalAvatar(FusionMetaAvatar metaAvatar)
    {
        localAvatar = metaAvatar.gameObject;

        VisualizeLocalAvatar();
    }
    
    public void VisualizeVirtualHands()
    {
        OVRLeftHandSynthetic.SetActive(true);
        OVRRightHandSynthetic.SetActive(true);
        
        localAvatar.SetActive(false);
    }

    public void VisualizeLocalAvatar()
    {
        OVRLeftHandSynthetic.SetActive(false);
        OVRRightHandSynthetic.SetActive(false);
        
        localAvatar.SetActive(true);
    }
}
