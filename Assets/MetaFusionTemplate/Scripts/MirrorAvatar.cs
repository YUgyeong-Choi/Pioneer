using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Oculus.Avatar2;
using Oculus.Platform;
using UnityEngine;
using CAPI = Oculus.Avatar2.CAPI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Chiligames.MetaFusionTemplate
{
    public class MirrorAvatar : OvrAvatarEntity
    {
        private AvatarSpawner avatarSpawner;
        private FusionMetaAvatar mainAvatar;

        protected override void Awake()
        {
            base.Awake();
            avatarSpawner = FindAnyObjectByType<AvatarSpawner>();
            avatarSpawner.OnLocalAvatarLoaded += AvatarSpawner_OnLocalAvatarLoaded;
            SetBodyTracking(FindObjectOfType<OvrAvatarBodyTrackingBehavior>());
        }

        private void AvatarSpawner_OnLocalAvatarLoaded(FusionMetaAvatar avatar)
        {
            mainAvatar = avatar;
            mainAvatar.OnNewAvatarSelected += MainAvatar_OnNewAvatarSelected;
            LoadNewAvatar(avatar.networkBehaviour.CurrentAvatar);
        }

        private void MainAvatar_OnNewAvatarSelected(ulong id)
        {
            LoadNewAvatar(id);
        }

        private const string logScope = "sampleAvatar";
        public enum AssetSource
        {
            /// Load from one of the preloaded .zip files
            Zip,

            /// Load a loose glb file directly from StreamingAssets
            StreamingAssets,
        }

        [System.Serializable]
        private struct AssetData
        {
            public AssetSource source;
            public string path;
        }

        public bool skeletonLoaded = false;

        [Header("Assets")]
        [Tooltip("Asset paths to load, and whether each asset comes from a preloaded zip file or directly from StreamingAssets. See Preset Asset settings on OvrAvatarManager for how this maps to the real file name.")]
        [SerializeField]
        private List<AssetData> _assets = new List<AssetData> { new AssetData { source = AssetSource.Zip, path = "0" } };

        [Tooltip("Adds an underscore between the path and the postfix.")]
        [SerializeField]
        private bool _underscorePostfix = true;

        [Tooltip("Filename Postfix (WARNING: Typically the postfix is Platform specific, such as \"_rift.glb\")")]
        [SerializeField]
        private string _overridePostfix = String.Empty;

        [Header("CDN")]
        [Tooltip("Automatically retry LoadUser download request on failure")]
        [SerializeField]
        private bool _autoCdnRetry = true;

        [Tooltip("Automatically check for avatar changes")]
        [SerializeField]
        private bool _autoCheckChanges = false;

        [Tooltip("How frequently to check for avatar changes")]
        [SerializeField]
        [Range(4.0f, 320.0f)]
        private float _changeCheckInterval = 8.0f;

        private Stopwatch _loadTime = new Stopwatch();

        private bool HasLocalAvatarConfigured => _assets.Count > 0;

        #region Loading
        private IEnumerator LoadCdnAvatar()
        {
            // Ensure OvrPlatform is Initialized
            if (OvrPlatformInit.status == OvrPlatformInitStatus.NotStarted)
            {
                OvrPlatformInit.InitializeOvrPlatform();
            }

            while (OvrPlatformInit.status != OvrPlatformInitStatus.Succeeded)
            {
                if (OvrPlatformInit.status == OvrPlatformInitStatus.Failed)
                {
                    OvrAvatarLog.LogWarning($"Error initializing OvrPlatform. Falling back to local avatar", logScope);
                    LoadRandomAvatar();
                    yield break;
                }

                yield return null;
            }

            // user ID == 0 means we want to load logged in user avatar from CDN
            if (_userId == 0)
            {
                // Get User ID
                bool getUserIdComplete = false;
                Users.GetLoggedInUser().OnComplete(message =>
                {
                    if (!message.IsError)
                    {
                        _userId = message.Data.ID;
                    }
                    else
                    {
                        var e = message.GetError();
                        OvrAvatarLog.LogError($"Error loading CDN avatar: {e.Message}. Falling back to local avatar", logScope);
                    }

                    getUserIdComplete = true;
                });

                while (!getUserIdComplete) { yield return null; }
            }

            yield return LoadUserAvatar();
        }

        public void LoadRemoteUserCdnAvatar(ulong userId)
        {
            StartLoadTimeCounter();
            _userId = userId;
            StartCoroutine(LoadCdnAvatar());
        }

        public void LoadLoggedInUserCdnAvatar()
        {
            StartLoadTimeCounter();
            _userId = 0;
            StartCoroutine(LoadCdnAvatar());
        }

        private IEnumerator LoadUserAvatar()
        {
            if (_userId == 0)
            {
                LoadLocalAvatar();
                yield break;
            }

            yield return Retry_HasAvatarRequest();
        }

        public void LoadLocalAvatar()
        {
            if (!HasLocalAvatarConfigured)
            {
                OvrAvatarLog.LogInfo("No local avatar asset configured", logScope, this);
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
                    + OvrAvatarManager.Instance.GetPlatformGLBPostfix(CAPI.ovrAvatar2EntityQuality.Standard, isFromZip)
                    + OvrAvatarManager.Instance.GetPlatformGLBVersion(CAPI.ovrAvatar2EntityQuality.Standard, isFromZip)
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

        public void LoadRandomAvatar()
        {
            int r = UnityEngine.Random.Range(0, 32);
            _assets[0] = new AssetData { source = AssetSource.Zip, path = r.ToString() };
            skeletonLoaded = false;
            EntityActive = false;
            Teardown();
            CreateEntity();
            LoadLocalAvatar();
        }

        public void LoadNewAvatar(ulong index)
        {
            if(index > 100)
            {
                skeletonLoaded = false;
                EntityActive = false;
                Teardown();
                CreateEntity();
                LoadRemoteUserCdnAvatar(index);
            }
            else
            {
                //Decrease 1 to the avatar index, as indexes actually go from 0 to 31, but we are using the 0 to express "no avatar chosen", because a [Networked] ulong starts with default value of 0.
                _assets[0] = new AssetData { source = AssetSource.Zip, path = (index - 1).ToString() };
                skeletonLoaded = false;
                EntityActive = false;
                Teardown();
                CreateEntity();
                LoadLocalAvatar();
            }
        }

        public void ReloadAvatarManually(string newAssetPaths, AssetSource newAssetSource)
        {
            string[] tempStringArray = new string[1];
            tempStringArray[0] = newAssetPaths;
            ReloadAvatarManually(tempStringArray, newAssetSource);
        }

        public void ReloadAvatarManually(string[] newAssetPaths, AssetSource newAssetSource)
        {
            Teardown();
            CreateEntity();

            bool isFromZip = (newAssetSource == AssetSource.Zip);
            string assetPostfix = (_underscorePostfix ? "_" : "")
                + OvrAvatarManager.Instance.GetPlatformGLBPostfix(_creationInfo.renderFilters.quality, isFromZip)
                + OvrAvatarManager.Instance.GetPlatformGLBVersion(_creationInfo.renderFilters.quality, isFromZip)
                + OvrAvatarManager.Instance.GetPlatformGLBExtension(isFromZip);

            string[] combinedPaths = new string[newAssetPaths.Length];
            for (var index = 0; index < newAssetPaths.Length; index++)
            {
                combinedPaths[index] = $"{newAssetPaths[index]}{assetPostfix}";
            }

            if (isFromZip)
            {
                LoadAssetsFromZipSource(combinedPaths);
            }
            else
            {
                LoadAssetsFromStreamingAssets(combinedPaths);
            }
        }

        public bool LoadPreset(int preset, string namePrefix = "")
        {
            StartLoadTimeCounter();
            bool isFromZip = true;
            string assetPostfix = (_underscorePostfix ? "_" : "")
                + OvrAvatarManager.Instance.GetPlatformGLBPostfix(_creationInfo.renderFilters.quality, isFromZip)
                + OvrAvatarManager.Instance.GetPlatformGLBVersion(_creationInfo.renderFilters.quality, isFromZip)
                + OvrAvatarManager.Instance.GetPlatformGLBExtension(isFromZip);

            var assetPath = $"{namePrefix}{preset}{assetPostfix}";
            return LoadAssetsFromZipSource(new string[] { assetPath });
        }

        protected override void OnSkeletonLoaded()
        {
            base.OnSkeletonLoaded();
            skeletonLoaded = true;
        }

        #region Unity Transforms

        public Transform GetSkeletonTransform(CAPI.ovrAvatar2JointType jointType)
        {
            if (!_criticalJointTypes.Contains(jointType))
            {
                OvrAvatarLog.LogError($"Can't access joint {jointType} unless it is in critical joint set");
                return null;
            }

            return GetSkeletonTransformByType(jointType);
        }

        public CAPI.ovrAvatar2JointType[] GetCriticalJoints()
        {
            return _criticalJointTypes;
        }

        #endregion

        #region Retry
        private void UserHasNoAvatarFallback()
        {
            OvrAvatarLog.LogError(
                $"Unable to find user avatar with userId {_userId}. Falling back to local avatar.", logScope, this);

            LoadLocalAvatar();
        }

        private IEnumerator Retry_HasAvatarRequest()
        {
            const float HAS_AVATAR_RETRY_WAIT_TIME = 4.0f;
            const int HAS_AVATAR_RETRY_ATTEMPTS = 12;

            int totalAttempts = _autoCdnRetry ? HAS_AVATAR_RETRY_ATTEMPTS : 1;
            bool continueRetries = _autoCdnRetry;
            int retriesRemaining = totalAttempts;
            bool hasFoundAvatar = false;
            bool requestComplete = false;
            do
            {
                var hasAvatarRequest = OvrAvatarManager.Instance.UserHasAvatarAsync(_userId);
                while (!hasAvatarRequest.IsCompleted) { yield return null; }

                switch (hasAvatarRequest.Result)
                {
                    case OvrAvatarManager.HasAvatarRequestResultCode.HasAvatar:
                        hasFoundAvatar = true;
                        requestComplete = true;
                        continueRetries = false;

                        // Now attempt download
                        yield return AutoRetry_LoadUser(true);
                        // End coroutine - do not load default
                        break;

                    case OvrAvatarManager.HasAvatarRequestResultCode.HasNoAvatar:
                        requestComplete = true;
                        continueRetries = false;

                        OvrAvatarLog.LogDebug(
                            "User has no avatar. Falling back to local avatar."
                            , logScope, this);
                        break;

                    case OvrAvatarManager.HasAvatarRequestResultCode.SendFailed:
                        OvrAvatarLog.LogError(
                            "Unable to send avatar status request."
                            , logScope, this);
                        break;

                    case OvrAvatarManager.HasAvatarRequestResultCode.RequestFailed:
                        OvrAvatarLog.LogError(
                            "An error occurred while querying avatar status."
                            , logScope, this);
                        break;

                    case OvrAvatarManager.HasAvatarRequestResultCode.BadParameter:
                        continueRetries = false;

                        OvrAvatarLog.LogError(
                            "Attempted to load invalid userId."
                            , logScope, this);
                        break;

                    case OvrAvatarManager.HasAvatarRequestResultCode.RequestCancelled:
                        continueRetries = false;

                        OvrAvatarLog.LogInfo(
                            "HasAvatar request cancelled."
                            , logScope, this);
                        break;

                    case OvrAvatarManager.HasAvatarRequestResultCode.UnknownError:
                    default:
                        OvrAvatarLog.LogError(
                            $"An unknown error occurred {hasAvatarRequest.Result}. Falling back to local avatar."
                            , logScope, this);
                        break;
                }

                continueRetries &= --retriesRemaining > 0;
                if (continueRetries)
                {
                    yield return new WaitForSecondsRealtime(HAS_AVATAR_RETRY_WAIT_TIME);
                }
            } while (continueRetries);

            if (!requestComplete)
            {
                OvrAvatarLog.LogError(
                    $"Unable to query UserHasAvatar {totalAttempts} attempts"
                    , logScope, this);
            }

            if (!hasFoundAvatar)
            {
                // We cannot find an avatar, use local fallback
                UserHasNoAvatarFallback();
            }

            // Check for changes unless a local asset is configured, user could create one later
            // If a local asset is loaded, it will currently conflict w/ the CDN asset
            if (_autoCheckChanges && (hasFoundAvatar || !HasLocalAvatarConfigured))
            {
                yield return PollForAvatarChange();
            }
        }

        private IEnumerator AutoRetry_LoadUser(bool loadFallbackOnFailure)
        {
            const float LOAD_USER_POLLING_INTERVAL = 4.0f;
            const float LOAD_USER_BACKOFF_FACTOR = 1.618033988f;
            const int CDN_RETRY_ATTEMPTS = 13;

            int totalAttempts = _autoCdnRetry ? CDN_RETRY_ATTEMPTS : 1;
            int remainingAttempts = totalAttempts;
            bool didLoadAvatar = false;
            var currentPollingInterval = LOAD_USER_POLLING_INTERVAL;
            do
            {
                LoadUser();

                CAPI.ovrAvatar2Result status;
                do
                {
                    // Wait for retry interval before taking any action
                    yield return new WaitForSecondsRealtime(currentPollingInterval);

                    //TODO: Cache status
                    status = this.entityStatus;
                    if (status.IsSuccess() || HasNonDefaultAvatar)
                    {
                        didLoadAvatar = true;
                        // Finished downloading - no more retries
                        remainingAttempts = 0;

                        OvrAvatarLog.LogDebug(
                            "Load user retry check found successful download, ending retry routine"
                            , logScope, this);
                        break;
                    }

                    currentPollingInterval *= LOAD_USER_BACKOFF_FACTOR;
                } while (status == CAPI.ovrAvatar2Result.Pending);
            } while (--remainingAttempts > 0);

            if (loadFallbackOnFailure && !didLoadAvatar)
            {
                OvrAvatarLog.LogError(
                    $"Unable to download user after {totalAttempts} retry attempts",
                    logScope, this);

                // We cannot download an avatar, use local fallback
                UserHasNoAvatarFallback();
            }
        }

        private void StartLoadTimeCounter()
        {
            _loadTime.Start();

            OnUserAvatarLoadedEvent.AddListener((OvrAvatarEntity entity) =>
            {
                _loadTime.Stop();
            });
        }

        public long GetLoadTimeMs()
        {
            return _loadTime.ElapsedMilliseconds;
        }

        #endregion // Retry

        #region Change Check

        private IEnumerator PollForAvatarChange()
        {
            var waitForPollInterval = new WaitForSecondsRealtime(_changeCheckInterval);

            bool continueChecking = true;
            do
            {
                yield return waitForPollInterval;

                var checkTask = HasAvatarChangedAsync();
                while (!checkTask.IsCompleted) { yield return null; }

                switch (checkTask.Result)
                {
                    case OvrAvatarManager.HasAvatarChangedRequestResultCode.UnknownError:
                        OvrAvatarLog.LogError(
                            "Check avatar changed unknown error, aborting."
                            , logScope, this);

                        // Stop retrying or we'll just spam this error
                        continueChecking = false;
                        break;
                    case OvrAvatarManager.HasAvatarChangedRequestResultCode.BadParameter:
                        OvrAvatarLog.LogError(
                            "Check avatar changed invalid parameter, aborting."
                            , logScope, this);

                        // Stop retrying or we'll just spam this error
                        continueChecking = false;
                        break;

                    case OvrAvatarManager.HasAvatarChangedRequestResultCode.SendFailed:
                        OvrAvatarLog.LogWarning(
                            "Check avatar changed send failed."
                            , logScope, this);
                        break;

                    case OvrAvatarManager.HasAvatarChangedRequestResultCode.RequestFailed:
                        OvrAvatarLog.LogError(
                            "Check avatar changed request failed."
                            , logScope, this);
                        break;

                    case OvrAvatarManager.HasAvatarChangedRequestResultCode.RequestCancelled:
                        OvrAvatarLog.LogInfo(
                            "Check avatar changed request cancelled."
                            , logScope, this);

                        // Stop retrying, this entity has most likely been destroyed
                        continueChecking = false;
                        break;

                    case OvrAvatarManager.HasAvatarChangedRequestResultCode.AvatarHasNotChanged:
                        OvrAvatarLog.LogVerbose(
                            "Avatar has not changed."
                            , logScope, this);
                        break;

                    case OvrAvatarManager.HasAvatarChangedRequestResultCode.AvatarHasChanged:
                        // Load new avatar!
                        OvrAvatarLog.LogInfo(
                            "Avatar has changed, loading new spec."
                            , logScope, this);

                        yield return AutoRetry_LoadUser(false);
                        break;
                }
            } while (continueChecking);
        }

        #endregion // Change Check
    }

}
