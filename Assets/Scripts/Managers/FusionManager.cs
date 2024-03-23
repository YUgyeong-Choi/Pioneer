using System.Collections;
using System.Collections.Generic;
using Chiligames.MetaFusionTemplate;
using UnityEngine;

public class FusionManager
{
    private GameObject NetworkCoreInstance;
    private AvatarSpawner avatarSpawner;
    
    
    public void Init()
    {
        // NetworkCoreInstance = Managers.Resource.Instantiate("NetworkCore");
        //
        // avatarSpawner = NetworkCoreInstance.GetComponent<AvatarSpawner>();
        // avatarSpawner.cameraRig = GameObject.Find("OVRCameraRigNetworkInteraction").transform;
        // avatarSpawner.centerEyeAnchor = avatarSpawner.cameraRig.GetChild(0).Find("CenterEyeAnchor");
    }
    
}
