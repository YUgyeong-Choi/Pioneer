using System;
using UnityEngine;
using UnityEngine.Events;
#if XR_ITK
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace VRUIP
{
#if OCULUS_INTEGRATION
    [RequireComponent(typeof(OVRGrabbable))]
#elif XR_ITK
    [RequireComponent(typeof(XRGrabInteractable))]
#endif
    public abstract class A_Grabbable : MonoBehaviour
    {
        private bool IsSetup { get; set; }
        #if OCULUS_INTEGRATION
        private OVRGrabbable ovrGrabbable;
        private OVRGrabber leftHand;
        private OVRGrabber rightHand;
        #elif XR_ITK
        private XRGrabInteractable xrGrabInteractable;
        #endif
        

        // OCULUS INTEGRATION
        // -- Activated means that the object is grabbed and the trigger is pressed at the same time. --
        private readonly UnityEvent _oculusActivated = new();
        private readonly UnityEvent _oculusDeactivated = new();
        private readonly UnityEvent _oculusGrabbed = new();
        private readonly UnityEvent _oculusReleased = new();
        private bool _isActivated;
        private bool _isGrabbed;
        private Transform _parent;

        protected virtual void Start()
        {
            if (!IsSetup) Setup();
        }

        public void Setup()
        {
            if (IsSetup) return;
            IsSetup = true;
            _parent = transform.parent;
            SetupFramework();
        }

        
        protected virtual void Update()
        {
#if OCULUS_INTEGRATION
            // Grabbing section
            var grabbed = ovrGrabbable.isGrabbed;
            if (grabbed && !_isGrabbed)
            {
                _oculusGrabbed.Invoke();
                _isGrabbed = true;
            }
            else if (!grabbed && _isGrabbed)
            {
                _oculusReleased.Invoke();
                _isGrabbed = false;
            }
            
            // Activating section
            bool isRightHandActivated = grabbed && ovrGrabbable.grabbedBy == rightHand && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            bool isLeftHandActivated = grabbed && ovrGrabbable.grabbedBy == leftHand && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
            // Check if the object is grabbed and the trigger is pressed and the event is not already invoked.
            if (!_isActivated && (isRightHandActivated || isLeftHandActivated))
            {
                _oculusActivated.Invoke();
                _isActivated = true;
            }
            else if (_isActivated && !isRightHandActivated && !isLeftHandActivated)
            {
                _oculusDeactivated.Invoke();
                _isActivated = false;
            }
#endif
        }
        

        private void SetupFramework()
        {
#if OCULUS_INTEGRATION
            ovrGrabbable = gameObject.GetComponent<OVRGrabbable>();
            if (VRUIPManager.instance.leftHand != null && VRUIPManager.instance.rightHand != null)
            {
                leftHand = VRUIPManager.instance.leftHand;
                rightHand = VRUIPManager.instance.rightHand;
            }
            else
            {
                Debug.LogWarning("Please assign the left and right hand to the VRUIPManager.");
            }
#elif XR_ITK
            xrGrabInteractable = gameObject.GetComponent<XRGrabInteractable>();
            // Set properties so item doesn't fly away when grabbed.
            xrGrabInteractable.throwOnDetach = false;
            xrGrabInteractable.attachEaseInTime = 0f;
#endif
            RegisterOnRelease(() => transform.parent = _parent);
        }

        protected void RegisterActivated(UnityAction action)
        {
            #if OCULUS_INTEGRATION
            _oculusActivated.AddListener(action);
            #elif XR_ITK
            xrGrabInteractable.activated.AddListener(_ => action.Invoke());
            #endif
        }
        
        protected void RegisterDeactivated(UnityAction action)
        {
            #if OCULUS_INTEGRATION
            _oculusDeactivated.AddListener(action);
            #elif XR_ITK
            xrGrabInteractable.deactivated.AddListener(_ => action.Invoke());
            #endif
        }
        
        protected void RegisterOnGrab(UnityAction action)
        {
            #if OCULUS_INTEGRATION
            _oculusGrabbed.AddListener(action.Invoke);
            #elif XR_ITK
            xrGrabInteractable.selectEntered.AddListener(_ => action.Invoke());
            #endif
        }
        
        protected void RegisterOnRelease(UnityAction action)
        {
            #if OCULUS_INTEGRATION
            _oculusReleased.AddListener(action.Invoke);
            #elif XR_ITK
            xrGrabInteractable.selectExited.AddListener(_ => action.Invoke());
            #endif
        }
    }
}
