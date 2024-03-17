using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Avatar2;
using Fusion;
using System;

namespace Chiligames.MetaFusionTemplate
{
    public class FusionMetaAvatar : OvrAvatarEntity
    {
        [SerializeField] StreamLOD streamLOD = StreamLOD.Medium;
        private NetworkObject networkObject;
        private AvatarSpawner avatarSpawner;

        [SerializeField] float _intervalToSendData = 0.08f;
        private float _cycleStartTime = 0;

        public bool skeletonLoaded = false;
        public AvatarNetworkBehaviour networkBehaviour;

        public event Action<ulong> OnNewAvatarSelected;

        protected override void Awake()
        {
        }

        private void Start()
        {
            networkObject = GetComponent<NetworkObject>();
            ConfigureAvatar();
            base.Awake();
            //After entity is created, we can set the remote avatar to be third person (and have a head!)
            if (!networkObject.HasInputAuthority)
            {
                SetActiveView(CAPI.ovrAvatar2EntityViewFlags.ThirdPerson);
            }

            StartCoroutine(TryToLoadUser());
        }

        public void SetAvatarSpawner(AvatarSpawner spawner)
        {
            avatarSpawner = spawner;
        }

        //Procedurally set the avatar creation features, this needs to be done before base.Awake() to be effective.
        void ConfigureAvatar()
        {
            if (networkObject.HasInputAuthority)
            {
                SetIsLocal(true);
                _creationInfo.features = CAPI.ovrAvatar2EntityFeatures.Preset_Default;
                SampleInputManager sampleInputManager = OvrAvatarManager.Instance.gameObject.GetComponent<SampleInputManager>();
                SetBodyTracking(sampleInputManager);
                OvrAvatarLipSyncContext lipSyncInput = FindObjectOfType<OvrAvatarLipSyncContext>();
                SetLipSync(lipSyncInput);
                gameObject.name = "LocalAvatar";
            }
            else
            {
                SetIsLocal(false);
                _creationInfo.features = CAPI.ovrAvatar2EntityFeatures.Preset_Remote;
                gameObject.name = "RemoteAvatar";
            }
        }

        IEnumerator TryToLoadUser()
        {
            //We wait until the oculusID is set and the app token has been set (only if entitlement didn't fail)
            while((int)networkBehaviour.CurrentAvatar == 0 || networkBehaviour.CurrentAvatar == null  || (!OvrAvatarEntitlement.AccessTokenIsValid() && avatarSpawner.entitlement != AvatarSpawner.Entitlement.Failed))
            {
                yield return null;
            }

            //If current avatar is less than 100, means it's one of the preset avatars
            if(networkBehaviour.CurrentAvatar < 100)
            {
                LoadPresetAvatarByIndex((int)networkBehaviour.CurrentAvatar);
            }
            //If not, means is an oculus id, so load personal avatar
            else
            {
                _userId = networkBehaviour.OculusID;

                var hasAvatarRequest = OvrAvatarManager.Instance.UserHasAvatarAsync(_userId);
                while (hasAvatarRequest.IsCompleted == false)
                {
                    yield return null;
                }
                OnNewAvatarSelected?.Invoke(_userId);
                LoadUser();
            }
        }

        //Callback to know when the skeleton was loaded
        protected override void OnSkeletonLoaded()
        {
            base.OnSkeletonLoaded();
            skeletonLoaded = true;
        }

        //If the skeleton is already loaded, we can start streaming the avatar state every "_intervalToSendData" seconds
        private void LateUpdate()
        {
            if (!skeletonLoaded) return;
            float elapsedTime = Time.time - _cycleStartTime;
            if (elapsedTime > _intervalToSendData)
            {
                RecordAndSendStreamDataIfHasAuthority();
                _cycleStartTime = Time.time;
            }
        }

        //We "record" our avatar state and send it to other users, only if avatar is local (is ours)
        void RecordAndSendStreamDataIfHasAuthority()
        {        
            if (IsLocal == true)
            {
                byte[] bytes = RecordStreamData(streamLOD);
                networkBehaviour.RPC_RecieveStreamData(bytes);
            }
        }

        public void LoadMyOculusAvatar()
        {
            _userId = networkBehaviour.OculusID;
            networkBehaviour.CurrentAvatar = _userId;
            OnNewAvatarSelected?.Invoke(_userId);
            Teardown();
            CreateEntity();
            LoadUser();
        }

        public void LoadRandomAvatar()
        {
            int r = UnityEngine.Random.Range(1, 33);
            _assets[0] = new AssetData { source = AssetSource.Zip, path = r.ToString() };
            skeletonLoaded = false;
            EntityActive = false;
            Teardown();
            CreateEntity();
            LoadLocalAvatar();
        }

        public void LoadPresetAvatarByIndex(int index)
        {
            //Decrease 1 to the avatar index, as indexes actually go from 0 to 31, but we are using the 0 to express "no avatar chosen", because a [Networked] ulong starts with default value of 0.
            _assets[0] = new AssetData { source = AssetSource.Zip, path = (index - 1).ToString() };
            skeletonLoaded = false;
            EntityActive = false;
            OnNewAvatarSelected?.Invoke((ulong)index);
            networkBehaviour.CurrentAvatar = (ulong)index;
            Teardown();
            CreateEntity();
            LoadLocalAvatar();
        }

        #region Testing
        public enum AssetSource
        {
            Zip,
            StreamingAssets,
        }
        [Serializable]
        private struct AssetData
        {
            public AssetSource source;
            public string path;
        }
        [Header("Testing in editor")]
        [Tooltip("Filename Postfix (WARNING: Typically the postfix is Platform specific, such as \"_rift.glb\")")]
        [SerializeField] private string _overridePostfix = String.Empty;
        [Tooltip("Adds an underscore between the path and the postfix.")]
        [SerializeField] private bool _underscorePostfix = true;
        [Header("Assets")]
        [Tooltip("Asset paths to load, and whether each asset comes from a preloaded zip file or directly from StreamingAssets")]
        [SerializeField] private List<AssetData> _assets = new List<AssetData> { new AssetData { source = AssetSource.Zip, path = "0" } };
        private bool HasLocalAvatarConfigured => _assets.Count > 0;

        public void LoadNewAvatar(string assetPath)
        {
            if (_assets[0].path == assetPath) return;
            networkBehaviour.RPC_LoadNewAvatar(assetPath);
        }

        public void LoadAvatarFromRPC(string assetPath)
        {
            _assets[0] = new AssetData { source = AssetSource.Zip, path = assetPath };
            skeletonLoaded = false;
            EntityActive = false;
            Teardown();
            CreateEntity();
            LoadLocalAvatar();
        }

        private void LoadLocalAvatar()
        {
            if (!HasLocalAvatarConfigured)
            {
                Debug.Log("No local avatar asset configured");
                return;
            }

            // Zip asset paths are relative to the inside of the zip.
            // Zips can be loaded from the OvrAvatarManager at startup or by calling OvrAvatarManager.Instance.AddZipSource
            // Assets can also be loaded individually from Streaming assets
            var path = new string[1];
            foreach (var asset in _assets)
            {
                bool isFromZip = (asset.source == AssetSource.Zip);

                string assetPostfix = (_underscorePostfix ? "_" : "")
                    + OvrAvatarManager.Instance.GetPlatformGLBPostfix(_creationInfo.renderFilters.quality, isFromZip)
                    + OvrAvatarManager.Instance.GetPlatformGLBVersion(_creationInfo.renderFilters.quality, isFromZip)
                    + OvrAvatarManager.Instance.GetPlatformGLBExtension(isFromZip);
                if (!String.IsNullOrEmpty(_overridePostfix))
                {
                    assetPostfix = _overridePostfix;
                }

                path[0] = asset.path + assetPostfix;
                if (isFromZip)
                {
                    LoadAssetsFromZipSource(path);
                }
                else
                {
                    LoadAssetsFromStreamingAssets(path);
                }
            }
        }
        #endregion
    }
}