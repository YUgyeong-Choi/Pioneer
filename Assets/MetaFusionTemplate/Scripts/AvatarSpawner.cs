using System.Collections;
using UnityEngine;
using Fusion;
using System;
using Oculus.Platform;
using Oculus.Avatar2;
using Fusion.Sockets;
using System.Collections.Generic;

namespace Chiligames.MetaFusionTemplate
{
    public class AvatarSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        public enum Entitlement{ NotChecked, Failed, Succeeded}
        public Entitlement entitlement = Entitlement.NotChecked;

        public AvatarNetworkBehaviour avatarPrefab;
        public NetworkObject speakerPrefab;

        private ulong userID = 0;
        [SerializeField] bool forceNoEntitlement;

        [HideInInspector] public NetworkRunner _runner;
        [SerializeField] NetworkRunnerHandler runnerHandler;
        [SerializeField] Transform cameraRig;
        [SerializeField] Transform centerEyeAnchor;
        [SerializeField] bool rememberAvatarPresetSelection;

        public event Action<FusionMetaAvatar> OnLocalAvatarLoaded;
        public event Action<Entitlement> OnEtitlementChecked;

        private bool runnerInitialized = false;
        private bool sceneLoaded = false;

        void Awake()
        {
            runnerHandler.OnNetworkRunnerInitialized += () => runnerInitialized = true;

            if (runnerHandler.networkRunner != null)
            {
                if (runnerHandler.networkRunner.IsRunning)
                {
                    runnerInitialized = true;
                }
            }

            if (forceNoEntitlement)
            {
                entitlement = Entitlement.Failed;
                CreateAvatarEntity();
                OnEtitlementChecked?.Invoke(entitlement);
                return;
            }
            //Initialize the oculus platform
            try
            {
                Core.AsyncInitialize();
                Entitlements.IsUserEntitledToApplication().OnComplete(EntitlementCallback);
            }
            catch (UnityException e)
            {           
                Debug.LogError("Platform failed to initialize due to exception.");
                Debug.LogException(e);
            }
        }

        void EntitlementCallback(Message msg)
        {
            if (msg.IsError)
            {
                Debug.LogError("You are NOT entitled to use this app. Please check if you added the correct ID's and credentials in Oculus>Platform");
                //If not entitled, create a default avatar
                entitlement = Entitlement.Failed;
                OnEtitlementChecked?.Invoke(entitlement);
                CreateAvatarEntity();
            }
            else
            {
                Debug.Log("You are entitled to use this app.");
                GetToken();
            }
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            _runner = runner;
            Debug.Log("OnConnectedToServer");
            SetPositionInSpawnpoint(runner);
        }

        private void SetPositionInSpawnpoint(NetworkRunner runner)
        {
            //Slightly randomize spawn position so we don't spawn on top of another user
            cameraRig.SetPositionAndRotation(cameraRig.position + new Vector3(UnityEngine.Random.Range(-0.2f,0.2f), 0, UnityEngine.Random.Range(-0.2f, 0.2f))
                , cameraRig.rotation);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        //Get Access token and user ID from Oculus Platform
        private void GetToken()
        {
            Users.GetAccessToken().OnComplete(message =>
            {
                if (!message.IsError)
                {
                    OvrAvatarEntitlement.SetAccessToken(message.Data);
                    Users.GetLoggedInUser().OnComplete(message =>
                    {
                        if (!message.IsError)
                        {
                            userID = message.Data.ID;
                            entitlement = Entitlement.Succeeded;
                            OnEtitlementChecked?.Invoke(entitlement);
                            CreateAvatarEntity();
                        }
                        else
                        {
                            var e = message.GetError();
                        }
                    });
                }
                else
                {
                    var e = message.GetError();
                }
            });
        }

        public void CreateAvatarEntity()
        {
            StartCoroutine(WaitForEntitlementCheckAndSpawn());
        }

        //Wait for all the entitlements and the runner to be ready to spawn
        IEnumerator WaitForEntitlementCheckAndSpawn()
        {
            if(this == null)
            {
                Debug.LogError("The NetworkCore GameObject got destroyed, probably you didn't setup the Photon Id correctly, make sure it's for the right version of Fusion that this project is using.");
            }

            AvatarNetworkBehaviour avatar;
            if (entitlement == Entitlement.Failed)
            {
                while (!runnerInitialized || !sceneLoaded)
                {
                    yield return null;
                }
                Debug.Log("Spawning preset avatar");
                avatar = _runner.Spawn(avatarPrefab, cameraRig.position, cameraRig.rotation, _runner.LocalPlayer);
                var obj = _runner.Spawn(speakerPrefab, centerEyeAnchor.position, centerEyeAnchor.rotation, _runner.LocalPlayer);
                obj.transform.SetParent(centerEyeAnchor.transform);
                var lipSync = FindObjectOfType<OvrAvatarLipSyncContext>();
                lipSync.CaptureAudio = true;
                var fusionAvatar = avatar.GetComponent<FusionMetaAvatar>();
                fusionAvatar.SetLipSync(lipSync);
                fusionAvatar.SetAvatarSpawner(this);
            }
            else
            {
                //Oculus entitlement OK
                while (entitlement == Entitlement.NotChecked || !OvrAvatarEntitlement.AccessTokenIsValid() || _runner == null || !runnerInitialized || !sceneLoaded)
                {
                    yield return null;
                }
                avatar = _runner.Spawn(avatarPrefab, cameraRig.position, cameraRig.rotation, _runner.LocalPlayer);
                var obj = _runner.Spawn(speakerPrefab, centerEyeAnchor.position, centerEyeAnchor.rotation, _runner.LocalPlayer);
                obj.transform.SetParent(centerEyeAnchor.transform);
                var lipSync = FindObjectOfType<OvrAvatarLipSyncContext>();
                lipSync.CaptureAudio = true;
                var fusionAvatar = avatar.GetComponent<FusionMetaAvatar>();
                fusionAvatar.SetLipSync(lipSync);
                fusionAvatar.SetAvatarSpawner(this);
            }

            //Avatar spawning
            FusionMetaAvatar avatarEntity = avatar.GetComponentInChildren<FusionMetaAvatar>();
            //Set avatar position and parent
            avatar.transform.SetParent(cameraRig);
            avatar.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            avatarEntity.OnUserAvatarLoadedEvent.AddListener(avatarEntity => OnLocalAvatarLoaded?.Invoke(avatarEntity as FusionMetaAvatar));

            //Set the oculusID and CurrentAvatar in the networkBehaviour so other users can access it to load our avatar
            if (entitlement == Entitlement.Succeeded)
            {
                avatarEntity.networkBehaviour.OculusID = userID;

                //Check If previously selected one of the prefab avatars
                if (PlayerPrefs.GetInt("PresetAvatarIndex", 0) == 0)
                {
                    avatarEntity.networkBehaviour.CurrentAvatar = userID;
                }
                else
                {
                    avatarEntity.networkBehaviour.CurrentAvatar = (ulong)PlayerPrefs.GetInt("PresetAvatarIndex", 0);
                }
            }
            else
            {
                //Check If previously selected one of the prefab avatars
                if (PlayerPrefs.GetInt("PresetAvatarIndex", 0) == 0)
                {
                    avatarEntity.networkBehaviour.CurrentAvatar = (ulong)UnityEngine.Random.Range(1, 32);
                }
                else
                {
                    avatarEntity.networkBehaviour.CurrentAvatar = (ulong)PlayerPrefs.GetInt("PresetAvatarIndex", 1);
                }
            }
        }

        //Reset avatar selection if exiting app
        private void OnApplicationQuit()
        {
            if (!rememberAvatarPresetSelection)
            {
                //Number 0 represents "No preset avatar chosen"
                PlayerPrefs.SetInt("PresetAvatarIndex", 0);
            }
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            sceneLoaded = true;
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }
        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
        }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }
        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}
