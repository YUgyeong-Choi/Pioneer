using Fusion;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using UnityEngine;

namespace Chiligames.MetaFusionTemplate
{
    public class AuthorityHandler : NetworkBehaviour
    {
        private Grabbable _grabbable;
        private bool wasOriginallyKinematic;
        private Rigidbody rigidBody;
        private HandGrabInteractor currentInteractor;

        private void Awake()
        {
            _grabbable = GetComponent<Grabbable>();
            rigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _grabbable.WhenPointerEventRaised += _grabbable_WhenPointerEventRaised;
        }

        private void _grabbable_WhenPointerEventRaised(PointerEvent evt)
        {
            currentInteractor = evt.Data as HandGrabInteractor;

            switch (evt.Type)
            {
                case PointerEventType.Select:
                    Grabbable_OnGrabBegin();
                    break;
                case PointerEventType.Unselect:
                    Grabbable_OnGrabEnd();
                    break;
                case PointerEventType.Move:
                    break;
            }
        }

        private void Grabbable_OnGrabBegin()
        {
            //If we don't have State Authority over grabbed object, request it.
            if (!Object.HasStateAuthority)
            {
                Object.RequestStateAuthority();
                StartCoroutine(WaitForAuthority());
            }
            //Else, we can directly set the Kinematic State
            else
            {
                ForceReleaseRestRPC();
                SetKinematicRestRPC(true);
                rigidBody.isKinematic = true;
            }
        }

        //We wait for the State authority to be ours (not instant) and then set the [Networked] Kinematic value
        IEnumerator WaitForAuthority()
        {
            ForceReleaseRestRPC();
            while (!Object.HasStateAuthority)
            {
                yield return null;
            }
            SetKinematicRestRPC(true);
        }

        [Rpc(RpcSources.All, RpcTargets.Proxies)]
        private void ForceReleaseRestRPC()
        {
            if(currentInteractor != null)
            {
                currentInteractor.ForceRelease();
                currentInteractor = null;
                rigidBody.isKinematic = true;
                //If force released (meaning another user grabbed the object), set kinematic to true after one frame (after meta's Interaction SDK)
                StartCoroutine(SetKinematicAfterFrame());
            }
        }

        IEnumerator SetKinematicAfterFrame()
        {
            yield return null;
            rigidBody.isKinematic = true;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.Proxies)]
        private void SetKinematicRestRPC(bool kinematic)
        {
            rigidBody.isKinematic = kinematic;
        }

        public override void Spawned()
        {
            wasOriginallyKinematic = rigidBody.isKinematic;
            if (!HasStateAuthority)
            {
                rigidBody.isKinematic = false;
            }
        }

        private void Grabbable_OnGrabEnd()
        {
            rigidBody.isKinematic = wasOriginallyKinematic;
        }

        private void OnDestroy()
        {
            _grabbable.WhenPointerEventRaised -= _grabbable_WhenPointerEventRaised;
        }
    }
}
