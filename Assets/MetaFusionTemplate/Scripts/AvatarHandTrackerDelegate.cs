using Oculus.Avatar2;
using Oculus.Interaction;
using Unity.XR.Oculus;
using UnityEngine;
using static Oculus.Avatar2.CAPI;

public class AvatarHandTrackerDelegate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private OvrAvatarInputManager _inputManager;
    public HandVisual leftHand, rightHand;
    private Transform[] mappedBoneRotations;
    SystemHeadset headsetType;

    private void Awake()
    {
        headsetType = Utils.GetSystemHeadsetType();
        mappedBoneRotations = new Transform[34];
    }
    void Start()
    {
        if (headsetType != SystemHeadset.None)
        {
            MapLeftHandBones(mappedBoneRotations, leftHand);
            MapRightHandBones(mappedBoneRotations, rightHand);

            _inputManager.BodyTracking.HandTrackingDelegate = new InteractionSdkHandTracker(_inputManager.BodyTracking.HandTrackingDelegate, mappedBoneRotations);
        }
    }

    private void MapRightHandBones(Transform[] mappedBoneRotations, HandVisual rightHand)
    {
        for (int i = 0; i < 17; ++i)
            mappedBoneRotations[17 + i] = rightHand.Joints[i + 2];
    }

    private void MapLeftHandBones(Transform[] mappedBoneRotations, HandVisual leftHand)
    {
        for (int i = 0; i < 17; ++i)
            mappedBoneRotations[i] = leftHand.Joints[i + 2];
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal class InteractionSdkHandTracker : IOvrAvatarHandTrackingDelegate
    {
        private IOvrAvatarHandTrackingDelegate baseHandTracking;
        private Transform[] mappedBoneRotations;
        internal InteractionSdkHandTracker(IOvrAvatarHandTrackingDelegate handTrackingDelegate, Transform[] mappedBoneRotations)
        {
            this.baseHandTracking = handTrackingDelegate;
            this.mappedBoneRotations = mappedBoneRotations;
        }
        public bool GetHandData(OvrAvatarTrackingHandsState handData)
        {
            bool isHandData = baseHandTracking.GetHandData(handData);

            if (isHandData)
            {
                handData.boneRotations[0] = ConvertQuat(mappedBoneRotations[0].localRotation);
                handData.boneRotations[1] = ConvertQuat(mappedBoneRotations[1].localRotation);
                handData.boneRotations[2] = ConvertQuat(mappedBoneRotations[2].localRotation);
                handData.boneRotations[3] = ConvertQuat(mappedBoneRotations[3].localRotation);
                handData.boneRotations[4] = ConvertQuat(mappedBoneRotations[4].localRotation);
                handData.boneRotations[5] = ConvertQuat(mappedBoneRotations[5].localRotation);
                handData.boneRotations[6] = ConvertQuat(mappedBoneRotations[6].localRotation);
                handData.boneRotations[7] = ConvertQuat(mappedBoneRotations[7].localRotation);
                handData.boneRotations[8] = ConvertQuat(mappedBoneRotations[8].localRotation);
                handData.boneRotations[9] = ConvertQuat(mappedBoneRotations[9].localRotation);
                handData.boneRotations[10] = ConvertQuat(mappedBoneRotations[10].localRotation);
                handData.boneRotations[11] = ConvertQuat(mappedBoneRotations[11].localRotation);
                handData.boneRotations[12] = ConvertQuat(mappedBoneRotations[12].localRotation);
                handData.boneRotations[13] = ConvertQuat(mappedBoneRotations[13].localRotation);
                handData.boneRotations[14] = ConvertQuat(mappedBoneRotations[14].localRotation);
                handData.boneRotations[15] = ConvertQuat(mappedBoneRotations[15].localRotation);
                handData.boneRotations[16] = ConvertQuat(mappedBoneRotations[16].localRotation);
                handData.boneRotations[17] = ConvertQuat(mappedBoneRotations[17].localRotation);
                handData.boneRotations[18] = ConvertQuat(mappedBoneRotations[18].localRotation);
                handData.boneRotations[19] = ConvertQuat(mappedBoneRotations[19].localRotation);
                handData.boneRotations[20] = ConvertQuat(mappedBoneRotations[20].localRotation);
                handData.boneRotations[21] = ConvertQuat(mappedBoneRotations[21].localRotation);
                handData.boneRotations[22] = ConvertQuat(mappedBoneRotations[22].localRotation);
                handData.boneRotations[23] = ConvertQuat(mappedBoneRotations[23].localRotation);
                handData.boneRotations[24] = ConvertQuat(mappedBoneRotations[24].localRotation);
                handData.boneRotations[25] = ConvertQuat(mappedBoneRotations[25].localRotation);
                handData.boneRotations[26] = ConvertQuat(mappedBoneRotations[26].localRotation);
                handData.boneRotations[27] = ConvertQuat(mappedBoneRotations[27].localRotation);
                handData.boneRotations[28] = ConvertQuat(mappedBoneRotations[28].localRotation);
                handData.boneRotations[29] = ConvertQuat(mappedBoneRotations[29].localRotation);
                handData.boneRotations[30] = ConvertQuat(mappedBoneRotations[30].localRotation);
                handData.boneRotations[31] = ConvertQuat(mappedBoneRotations[31].localRotation);
                handData.boneRotations[32] = ConvertQuat(mappedBoneRotations[32].localRotation);
                handData.boneRotations[33] = ConvertQuat(mappedBoneRotations[33].localRotation);
            }

            return isHandData;
        }

        private ovrAvatar2Quatf ConvertQuat(Quaternion q)
        {
            ovrAvatar2Quatf result = new ovrAvatar2Quatf();
            result.x = q.x;
            result.y = -q.y;
            result.z = -q.z;
            result.w = q.w;
            return result;
        }
    }
}