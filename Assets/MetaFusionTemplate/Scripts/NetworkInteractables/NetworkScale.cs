using Fusion;
using UnityEngine;

namespace Chiligames.MetaFusionTemplate
{
    public class NetworkScale : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(ScaleChanged), OnChangedTargets = OnChangedTargets.Proxies)]
        private Vector3 NetworkedScale { get; set; }

        public override void Spawned()
        {
            base.Spawned();
            NetworkedScale = transform.localScale;
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                if (transform.localScale != NetworkedScale)
                {
                    NetworkedScale = transform.localScale;
                }
            }
        }

        public static void ScaleChanged(Changed<NetworkScale> changed)
        {
            changed.Behaviour.transform.localScale = changed.Behaviour.NetworkedScale;
        }
    }
}