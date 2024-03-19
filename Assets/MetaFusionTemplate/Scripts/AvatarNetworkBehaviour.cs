using Fusion;
using System;
using UnityEngine;

namespace Chiligames.MetaFusionTemplate
{
    public class AvatarNetworkBehaviour : NetworkBehaviour
    {
        [SerializeField] FusionMetaAvatar avatar;

        //The [Networked] attribute allows us to easily share the state of a variable across the network just by setting it.
        [Networked(OnChanged = nameof(IdChanged))]
        public ulong OculusID { get; set; }
        [Networked]
        public ulong CurrentAvatar { get; set; } = 0;

        public event Action OnOculusIdLoaded;

        public override void Spawned()
        {
            base.Spawned();
        }

        //RPCs in Fusion must be called from a NetworkBehaviour
        [Rpc(RpcSources.InputAuthority, RpcTargets.Proxies, InvokeLocal = false)]
        public void RPC_RecieveStreamData(byte[] bytes)
        {
            if (avatar.skeletonLoaded)
            {
                avatar.ApplyStreamData(bytes);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
        public void RPC_LoadNewAvatar(string assetPath)
        {
            avatar.LoadAvatarFromRPC(assetPath);
        }

        public static void IdChanged(Changed<AvatarNetworkBehaviour> changed)
        {
            if(changed.Behaviour.OculusID > 0)
            {
                changed.Behaviour.OnOculusIdLoaded?.Invoke();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_LoadMyOculusAvatar()
        {
            avatar.LoadMyOculusAvatar();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_LoadRandomAvatar()
        {
            avatar.LoadRandomAvatar();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_LoadLocalAvatarIndex(int index)
        {
            avatar.LoadPresetAvatarByIndex(index);
        }
    }
}
