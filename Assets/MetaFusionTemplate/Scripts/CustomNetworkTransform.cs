using Fusion;
using UnityEngine;

namespace Chiligames.MetaFusionTemplate
{
    public class CustomNetworkTransform : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(PositionChanged), OnChangedTargets = OnChangedTargets.Proxies)]
        private Vector3 NetworkPosition { get; set; } = Vector3.zero;
        [Networked(OnChanged = nameof(RotationChanged), OnChangedTargets = OnChangedTargets.Proxies)]
        private Quaternion NetworkRotation { get; set; } = Quaternion.identity;

        private Vector3 targetPosition;
        private Quaternion targetRotation;
        [SerializeField] private float interpolationSpeed = 30f;

        public override void Spawned()
        {
            base.Spawned();
            targetPosition = transform.position;
            targetRotation = transform.rotation;
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                if (transform.position != NetworkPosition)
                {
                    NetworkPosition = transform.position;
                }
                if (transform.rotation != NetworkRotation)
                {
                    NetworkRotation = transform.rotation;
                }

                targetPosition = transform.position;
                targetRotation = transform.rotation;
            }
            if (!HasStateAuthority)
            {
                if (transform.position != targetPosition && targetPosition != Vector3.zero)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * interpolationSpeed);
                }
                if (transform.rotation != targetRotation && targetRotation != Quaternion.identity)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * interpolationSpeed);
                }
            }
        }

        public static void PositionChanged(Changed<CustomNetworkTransform> changed)
        {
            changed.Behaviour.targetPosition = changed.Behaviour.NetworkPosition;
        }
        public static void RotationChanged(Changed<CustomNetworkTransform> changed)
        {
            changed.Behaviour.targetRotation = changed.Behaviour.NetworkRotation;
        }
    }
}